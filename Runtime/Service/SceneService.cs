using Rossoforge.Events.Bus;
using Rossoforge.Events.Service;
using Rossoforge.Scenes.DataConfig;
using Rossoforge.Scenes.Events;
using Rossoforge.Services.Locator;
using Rossoforge.Services.Service;
using Rossoforge.Utils.Logger;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rossoforge.Scenes.Service
{
    public class SceneService : ISceneService, IInitializable, IDisposable,
        IEventListener<SceneTransitionActiveEvent>,
        IEventListener<SceneTransitionInactiveEvent>
    {
        private IEventService _eventService;
        private SceneDataService _dataService;

        private string _previousSceneName;
        private ISceneTransitionDataConfig _currentSceneTransition;
        private AwaitableCompletionSource _transitionEffectCompletionSource;

        /// <summary>
        /// Gets the name of the currently active scene in Unity's <see cref="SceneManager"/>.
        /// </summary>
        public string CurrentSceneName => SceneManager.GetActiveScene().name;

        /// <summary>
        /// Gets a value indicating whether a scene transition sequence is currently in progress.
        /// </summary>
        public bool IsTransitionRunning { get; private set; }

        public SceneService(SceneDataService dataService)
        {
            _dataService = dataService;
        }

        public void Initialize()
        {
            _eventService = ServiceLocator.Get<IEventService>();

            _eventService.RegisterListener<SceneTransitionActiveEvent>(this);
            _eventService.RegisterListener<SceneTransitionInactiveEvent>(this);
        }

        public void Dispose()
        {
            _eventService.UnregisterListener<SceneTransitionActiveEvent>(this);
            _eventService.UnregisterListener<SceneTransitionInactiveEvent>(this);
        }

        /// <summary>
        /// Changes to the specified destination scene using the default transition configuration.
        /// </summary>
        /// <param name="sceneName">The name of the destination scene to load.</param>
        /// <param name="onScreenCoveredAsync">Optional asynchronous callback executed while the transition screen fully covers the view, prior to unloading the current scene.</param>
        public Awaitable ChangeScene(string sceneName, Func<Awaitable> onScreenCoveredAsync = null)
        {
            return ChangeScene(sceneName, _dataService.DefaultSceneTransition, onScreenCoveredAsync);
        }

        /// <summary>
        /// Changes to the specified destination scene using a custom transition configuration and optional cleanup task.
        /// </summary>
        /// <param name="sceneName">The name of the destination scene to load.</param>
        /// <param name="sceneTransitionData">Data specifying which transition scene and visual effect settings to use.</param>
        /// <param name="onScreenCoveredAsync">Optional asynchronous callback executed while the transition screen fully covers the view, prior to unloading the current scene.</param>
        public async Awaitable ChangeScene(string sceneName, ISceneTransitionDataConfig sceneTransitionData, Func<Awaitable> onScreenCoveredAsync = null)
        {
            if (IsTransitionRunning)
            {
                RossoLogger.Warning("A scene transition is already running.");
                return;
            }

            await ShowTransitionScene(sceneTransitionData);

            // Execute custom cleanup/saving logic while screen is black/covered
            if (onScreenCoveredAsync != null)
            {
                try
                {
                    await onScreenCoveredAsync.Invoke();
                }
                catch (Exception ex)
                {
                    RossoLogger.Error($"Error executing cleanup task during scene transition: {ex.Message}\n{ex.StackTrace}");
                }
            }

            _previousSceneName = CurrentSceneName;
            await UnloadSceneAsync(CurrentSceneName);

            await LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Ensure the newly loaded additive scene becomes the active scene
            Scene targetScene = SceneManager.GetSceneByName(sceneName);
            if (targetScene.IsValid())
            {
                SceneManager.SetActiveScene(targetScene);
            }

            await HideTransitionScene();
        }

        /// <summary>
        /// Navigates back to the previously loaded scene using the default transition configuration.
        /// </summary>
        /// <param name="onScreenCoveredAsync">Optional asynchronous callback executed while the transition screen fully covers the view.</param>
        public Awaitable GoBackScene(Func<Awaitable> onScreenCoveredAsync = null)
        {
            return GoBackScene(_dataService.DefaultSceneTransition, onScreenCoveredAsync);
        }

        /// <summary>
        /// Navigates back to the previously loaded scene using a custom transition configuration.
        /// </summary>
        /// <param name="sceneTransitionData">Data specifying which transition scene to display during navigation.</param>
        /// <param name="onScreenCoveredAsync">Optional asynchronous callback executed while the transition screen fully covers the view.</param>
        public async Awaitable GoBackScene(ISceneTransitionDataConfig sceneTransitionData, Func<Awaitable> onScreenCoveredAsync = null)
        {
            if (!string.IsNullOrWhiteSpace(_previousSceneName))
                await ChangeScene(_previousSceneName, sceneTransitionData, onScreenCoveredAsync);
        }

        /// <summary>
        /// Restarts the currently active scene using the default transition configuration.
        /// </summary>
        /// <param name="onScreenCoveredAsync">Optional asynchronous callback executed while the transition screen fully covers the view.</param>
        public Awaitable RestartScene(Func<Awaitable> onScreenCoveredAsync = null)
        {
            return RestartScene(_dataService.DefaultSceneTransition, onScreenCoveredAsync);
        }

        /// <summary>
        /// Restarts the currently active scene using a custom transition configuration.
        /// </summary>
        /// <param name="sceneTransitionData">Data specifying which transition scene to display during restart.</param>
        /// <param name="onScreenCoveredAsync">Optional asynchronous callback executed while the transition screen fully covers the view.</param>
        public Awaitable RestartScene(ISceneTransitionDataConfig sceneTransitionData, Func<Awaitable> onScreenCoveredAsync = null)
        {
            return ChangeScene(CurrentSceneName, sceneTransitionData, onScreenCoveredAsync);
        }

        /// <summary>
        /// Asynchronously loads a scene by name using Unity's <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="mode">The loading mode (Additive or Single).</param>
        public async Awaitable LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, mode);
            if (asyncOp == null)
            {
                RossoLogger.Error($"Failed to load scene {sceneName}");
                return;
            }

            await asyncOp;
        }

        /// <summary>
        /// Asynchronously unloads a scene by name using Unity's <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        public async Awaitable UnloadSceneAsync(string sceneName)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(sceneName);
            if (asyncOp == null)
            {
                RossoLogger.Error($"Failed to unload scene {sceneName}");
                return;
            }

            await asyncOp;
        }

        /// <summary>
        /// Loads the transition scene additively and waits for its activation visual effect to finish.
        /// </summary>
        /// <param name="sceneTransitionData">Data specifying which transition scene to load.</param>
        private async Awaitable ShowTransitionScene(ISceneTransitionDataConfig sceneTransitionData)
        {
            if (IsTransitionRunning)
                return;

            _currentSceneTransition = sceneTransitionData;
            _previousSceneName = CurrentSceneName;

            IsTransitionRunning = true;

            _transitionEffectCompletionSource = new AwaitableCompletionSource();
            await LoadSceneAsync(sceneTransitionData.TransitionSceneName, LoadSceneMode.Additive);
            await _transitionEffectCompletionSource.Awaitable;
        }

        /// <summary>
        /// Notifies the transition scene to begin its deactivation visual effect, waits for it to finish, and unloads it.
        /// </summary>
        private async Awaitable HideTransitionScene()
        {
            if (!IsTransitionRunning)
                return;

            _transitionEffectCompletionSource = new AwaitableCompletionSource();

            // Notify the transition scene that target loading is complete so it can start its fade-out/custom animation
            _eventService.Raise<TargetSceneLoadedCompletedEvent>();

            // Wait for SceneTransitionInactiveEvent to signal that deactivation effect finished
            await _transitionEffectCompletionSource.Awaitable;

            await UnloadSceneAsync(_currentSceneTransition.TransitionSceneName);
            IsTransitionRunning = false;
        }

        /// <summary>
        /// Handles the event emitted when the transition scene finishes its activation visual effect (screen fully covered).
        /// </summary>
        /// <param name="eventArg">The event payload.</param>
        public void OnEventInvoked(SceneTransitionActiveEvent eventArg)
        {
            _transitionEffectCompletionSource?.TrySetResult();
        }

        /// <summary>
        /// Handles the event emitted when the transition scene finishes its deactivation visual effect (screen revealed).
        /// </summary>
        /// <param name="eventArg">The event payload.</param>
        public void OnEventInvoked(SceneTransitionInactiveEvent eventArg)
        {
            _transitionEffectCompletionSource?.TrySetResult();
        }
    }
}