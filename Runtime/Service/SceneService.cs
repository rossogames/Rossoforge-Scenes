using Rossoforge.Core.Events;
using Rossoforge.Core.Services;
using Rossoforge.Scenes.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rossoforge.Scenes.Service
{
    public class SceneService : ISceneService
    //IEventListener<SceneTransitionStandByEvent>,
    //IEventListener<SceneTransitionCompletedEvent>
    {
        private IEventService _eventService;

        private string _previousSceneName;
        private string _nextSceneName;
        private SceneTransitionData _transitionDataDefault;

        public string CurrentSceneName => SceneManager.GetActiveScene().name;
        public bool IsLoading { get; private set; }

        public SceneService(IEventService eventService, SceneTransitionData sceneTransitionData)
        {
            _eventService = eventService;
            _transitionDataDefault = sceneTransitionData;
        }
        public void Initialize()
        {
            //_eventService.RegisterListener<SceneTransitionStandByEvent>(this);
            //_eventService.RegisterListener<SceneTransitionCompletedEvent>(this);
        }
        public void Dispose()
        {
            //_eventService.UnregisterListener<SceneTransitionStandByEvent>(this);
            //_eventService.UnregisterListener<SceneTransitionCompletedEvent>(this);
        }

        //public async void OnEventInvoked(SceneTransitionStandByEvent eventArg)
        //{
        //    await SceneManager.UnloadSceneAsync(_previousSceneName);
        //
        //    if (!string.IsNullOrWhiteSpace(_nextSceneName))
        //    {
        //        await SceneManager.LoadSceneAsync(_nextSceneName, LoadSceneMode.Additive);
        //        await Awaitable.NextFrameAsync();
        //    }
        //
        //    _eventService.Raise<SceneTransitionCloseEvent>();
        //}
        //public async void OnEventInvoked(SceneTransitionCompletedEvent eventArg)
        //{
        //    await SceneManager.UnloadSceneAsync(_transitionData.TransitionSceneName);
        //    IsLoading = false;
        //}
        public Awaitable LoadScene(string sceneName)
        {
            return LoadScene(sceneName, _transitionDataDefault);
        }
        public async Awaitable LoadScene(string sceneName, SceneTransitionData sceneTransitionData)
        {
            if (IsLoading)
                return;

            _previousSceneName = CurrentSceneName;
            _nextSceneName = sceneName;

            IsLoading = true;

            await SceneManager.LoadSceneAsync(sceneTransitionData.TransitionSceneName, LoadSceneMode.Additive);
        }
        public async void UnloadScene(string sceneName, SceneTransitionData sceneTransition)
        {
            _transitionDataDefault = sceneTransition;
            _previousSceneName = sceneName;
            _nextSceneName = null;
            await SceneManager.LoadSceneAsync(_transitionDataDefault.TransitionSceneName, LoadSceneMode.Additive);

            //_eventService.Raise(new SceneUnloadedEvent(_previousSceneName));
        }
        public async void UnloadScene(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }

        public void GoBack()
        {
            if (!string.IsNullOrWhiteSpace(_previousSceneName))
                LoadScene(_previousSceneName);
        }
        public void Restart()
        {
            LoadScene(CurrentSceneName);
        }
    }
}