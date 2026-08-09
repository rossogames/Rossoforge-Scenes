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

        public async Awaitable UnloadCurrentScene(ISceneTransitionData sceneTransitionData)
        {
            await LoadTransitionScene(sceneTransitionData);

            _previousSceneName = CurrentSceneName;
            await UnloadSceneAsync(CurrentSceneName);
        }
        public async Awaitable LoadScene(string sceneName, ISceneTransitionData sceneTransitionData)
        {
            await UnloadTransitionScene();
            await LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        public Awaitable ChangeScene(string sceneName)
        {
            return ChangeScene(sceneName, _serviceData.DefaultSceneTransitionData);
        }
        public async Awaitable ChangeScene(string sceneName, ISceneTransitionData sceneTransitionData)
        {
            await UnloadCurrentScene(sceneTransitionData);
            await LoadScene(sceneName, sceneTransitionData);
        }
        public Awaitable GoBackScene()
        {
            return GoBackScene(_serviceData.DefaultSceneTransitionData);
        }
        public async Awaitable GoBackScene(ISceneTransitionData sceneTransitionData)
        {
            if (!string.IsNullOrWhiteSpace(_previousSceneName))
                await ChangeScene(_previousSceneName, sceneTransitionData);
        }
        public Awaitable RestartScene()
        {
            return RestartScene(_serviceData.DefaultSceneTransitionData);
        }
        public Awaitable RestartScene(ISceneTransitionData sceneTransitionData)
        {
            return ChangeScene(CurrentSceneName, sceneTransitionData);
        }

        private async Awaitable LoadTransitionScene(ISceneTransitionData sceneTransitionData)
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
        private async Awaitable UnloadTransitionScene()
        {
            if (_isTransitionRuning)
                return;

            _isTransitionRuning = true;

            _transitionEffectCompletionSource = new AwaitableCompletionSource();
            await UnloadSceneAsync(_currentTransitionData.TransitionSceneName);
            await _transitionEffectCompletionSource.Awaitable;

            _isTransitionRuning = false;
        }

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

        public async void OnEventInvoked(SceneTransitionActiveEvent eventArg)
        {
            _transitionEffectCompletionSource.SetResult();
            // This line is used to signal that the transition effect has completed, allowing the scene change to proceed.
        }
        public async void OnEventInvoked(SceneTransitionInactiveEvent eventArg)
        {
            _transitionEffectCompletionSource.SetResult();
            // This line is used to signal that the transition effect has completed, allowing the scene change to proceed.
        }
    }
}