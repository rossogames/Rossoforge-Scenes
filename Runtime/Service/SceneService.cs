using Rossoforge.Core.Events;
using Rossoforge.Core.Services;
using Rossoforge.Scenes.Data;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;

namespace Rossoforge.Scenes.Service
{
    public class SceneService : ISceneService, IInitializable, IDisposable,
    IEventListener<SceneTransitionActiveEvent>,
    IEventListener<SceneTransitionInactiveEvent>
    {
        private IEventService _eventService;

        private string _previousSceneName;
        private string _nextSceneName;
        private SceneTransitionData _transitionDataDefault;
        private SceneTransitionData _currentTransitionData;

        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool IsLoading { get; private set; }

        public SceneService(IEventService eventService, SceneTransitionData sceneTransitionData)
        {
            _eventService = eventService;
            _transitionDataDefault = sceneTransitionData;
        }
        public void Initialize()
        {
            _eventService.RegisterListener<SceneTransitionActiveEvent>(this);
            _eventService.RegisterListener<SceneTransitionInactiveEvent>(this);
        }
        public void Dispose()
        {
            _eventService.UnregisterListener<SceneTransitionActiveEvent>(this);
            _eventService.UnregisterListener<SceneTransitionInactiveEvent>(this);
        }

        public Awaitable LoadScene(string sceneName)
        {
            return LoadScene(sceneName, _transitionDataDefault);
        }
        public async Awaitable LoadScene(string sceneName, SceneTransitionData sceneTransitionData)
        {
            _currentTransitionData = sceneTransitionData;

            if (IsLoading)
                return;

            _previousSceneName = CurrentSceneName;
            _nextSceneName = sceneName;

            IsLoading = true;

            await SceneManager.LoadSceneAsync(sceneTransitionData.TransitionSceneName, LoadSceneMode.Additive);
        }
        //public async void UnloadScene(string sceneName, SceneTransitionData sceneTransition)
        //{
        //    _transitionDataDefault = sceneTransition;
        //    _previousSceneName = sceneName;
        //    _nextSceneName = null;
        //    await SceneManager.LoadSceneAsync(_transitionDataDefault.TransitionSceneName, LoadSceneMode.Additive);
        //
        //    //_eventService.Raise(new SceneUnloadedEvent(_previousSceneName));
        //}
        //public async void UnloadScene(string sceneName)
        //{
        //    await SceneManager.UnloadSceneAsync(sceneName);
        //}
        //
        //public void GoBack()
        //{
        //    if (!string.IsNullOrWhiteSpace(_previousSceneName))
        //        LoadScene(_previousSceneName);
        //}
        public Awaitable Restart()
        {
            return LoadScene(CurrentSceneName);
        }

        public Awaitable Restart(SceneTransitionData sceneTransitionData)
        {
           return LoadScene(CurrentSceneName, sceneTransitionData);
        }

        public async void OnEventInvoked(SceneTransitionActiveEvent eventArg)
        {
            await ChangeNextScene();
        }
        public async void OnEventInvoked(SceneTransitionInactiveEvent eventArg)
        {
           await UnloadTransitionScene();
        }

        private async Awaitable ChangeNextScene()
        {
            await SceneManager.UnloadSceneAsync(_previousSceneName);

            if (!string.IsNullOrWhiteSpace(_nextSceneName))
            {
                await SceneManager.LoadSceneAsync(_nextSceneName, LoadSceneMode.Additive);
                await Awaitable.NextFrameAsync();
            }

            _eventService.Raise<TargetSceneLoadedCompletedEvent>();
        }

        private async Awaitable UnloadTransitionScene()
        {
            await SceneManager.UnloadSceneAsync(_currentTransitionData.TransitionSceneName);
            IsLoading = false;
        }
    }
}