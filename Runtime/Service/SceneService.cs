using Rossoforge.Core.Events;
using Rossoforge.Core.Scenes;
using Rossoforge.Core.Services;
using Rossoforge.Scenes.Events;
using Rossoforge.Services;
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
        private SceneServiceData _serviceData;

        private string _previousSceneName;
        private ISceneTransitionData _currentTransitionData;
        private AwaitableCompletionSource _transitionEffectCompletionSource;

        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool _isTransitionRuning;

        public SceneService(SceneServiceData serviceData)
        {
            _serviceData = serviceData;
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
        /// Loads the transition scene and unloads the current active scene.
        /// </summary>
        /// <param name="sceneTransitionData">Data specifying which transition scene to use.</param>
        public async Awaitable UnloadCurrentScene(ISceneTransitionData sceneTransitionData)
        {
            await ShowTransitionScene(sceneTransitionData);

            _previousSceneName = CurrentSceneName;
            await UnloadSceneAsync(CurrentSceneName);
        }

        /// <summary>
        /// Loads the target scene and unloads the current transition scene.
        /// </summary>
        /// <param name="sceneName">The name of the target scene to load.</param>
        public async Awaitable LoadScene(string sceneName)
        {
            await LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            await HideTransitionScene();
        }

        /// <summary>
        /// Changes to the specified scene using the default transition scene.
        /// </summary>
        /// <param name="sceneName">The name of the destination scene.</param>
        public Awaitable ChangeScene(string sceneName)
        {
            return ChangeScene(sceneName, _serviceData.DefaultSceneTransitionData);
        }

        /// <summary>
        /// Changes to the specified scene using a specific transition scene.
        /// </summary>
        /// <param name="sceneName">The name of the destination scene.</param>
        /// <param name="sceneTransitionData">Data specifying which transition scene to display during the switch.</param>
        public async Awaitable ChangeScene(string sceneName, ISceneTransitionData sceneTransitionData)
        {
            await UnloadCurrentScene(sceneTransitionData);
            await LoadScene(sceneName);
        }

        /// <summary>
        /// Navigates back to the previously loaded scene using the default transition scene.
        /// </summary>
        public Awaitable GoBackScene()
        {
            return GoBackScene(_serviceData.DefaultSceneTransitionData);
        }

        /// <summary>
        /// Navigates back to the previously loaded scene using a specific transition scene.
        /// </summary>
        /// <param name="sceneTransitionData">Data specifying which transition scene to display during the switch.</param>
        public async Awaitable GoBackScene(ISceneTransitionData sceneTransitionData)
        {
            if (!string.IsNullOrWhiteSpace(_previousSceneName))
                await ChangeScene(_previousSceneName, sceneTransitionData);
        }

        /// <summary>
        /// Restarts the currently active scene using the default transition scene.
        /// </summary>
        public Awaitable RestartScene()
        {
            return RestartScene(_serviceData.DefaultSceneTransitionData);
        }

        /// <summary>
        /// Restarts the currently active scene using a specific transition scene.
        /// </summary>
        /// <param name="sceneTransitionData">Data specifying which transition scene to display during the restart.</param>
        public Awaitable RestartScene(ISceneTransitionData sceneTransitionData)
        {
            return ChangeScene(CurrentSceneName, sceneTransitionData);
        }

        /// <summary>
        /// Loads the specified transition scene additively and waits for its active effect event to complete.
        /// </summary>
        /// <param name="sceneTransitionData">Data identifying the transition scene to load.</param>
        private async Awaitable ShowTransitionScene(ISceneTransitionData sceneTransitionData)
        {
            if (_isTransitionRuning)
                return;

            _currentTransitionData = sceneTransitionData;
            _previousSceneName = CurrentSceneName;

            _isTransitionRuning = true;

            _transitionEffectCompletionSource = new AwaitableCompletionSource();
            await LoadSceneAsync(sceneTransitionData.TransitionSceneName, LoadSceneMode.Additive);
            await _transitionEffectCompletionSource.Awaitable;

            _isTransitionRuning = false;
        }

        /// <summary>
        /// Unloads the active transition scene and waits for its inactive effect event to complete.
        /// </summary>
        private async Awaitable HideTransitionScene()
        {
            if (_isTransitionRuning)
                return;

            _isTransitionRuning = true;

            _transitionEffectCompletionSource = new AwaitableCompletionSource();
            _eventService.Raise<TargetSceneLoadedCompletedEvent>(); // Notify that the target scene has finished loading, allowing the transition scene to start its deactivation effect.
            await _transitionEffectCompletionSource.Awaitable; // Wait for the transition scene to finish its deactivation effect before unloading it.

            await UnloadSceneAsync(_currentTransitionData.TransitionSceneName);
            _isTransitionRuning = false;
        }

        /// <summary>
        /// Asynchronously loads a scene using Unity's <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="mode">The load mode (Additive or Single).</param>
        private async Awaitable LoadSceneAsync(string sceneName, LoadSceneMode mode)
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
        /// Asynchronously unloads a scene using Unity's <see cref="SceneManager"/>.
        /// </summary>
        /// <param name="sceneName">The name of the scene to unload.</param>
        private async Awaitable UnloadSceneAsync(string sceneName)
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
        /// Handles the event emitted when the transition scene finishes its activation visual effect.
        /// </summary>
        /// <param name="eventArg">The event payload.</param>
        public async void OnEventInvoked(SceneTransitionActiveEvent eventArg)
        {
            _transitionEffectCompletionSource.SetResult();
        }

        /// <summary>
        /// Handles the event emitted when the transition scene finishes its deactivation visual effect.
        /// </summary>
        /// <param name="eventArg">The event payload.</param>
        public async void OnEventInvoked(SceneTransitionInactiveEvent eventArg)
        {
            _transitionEffectCompletionSource.SetResult();
        }
    }
}