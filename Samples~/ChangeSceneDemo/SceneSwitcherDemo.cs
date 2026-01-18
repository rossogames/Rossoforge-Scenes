using Rossoforge.Core.Events;
using Rossoforge.Core.Scenes;
using Rossoforge.Scenes.Data;
using Rossoforge.Scenes.Events;
using Rossoforge.Services;
using Rossoforge.Utils.Logger;
using UnityEngine;

namespace Rossoforge.Scenes.Samples.ChangeSceneDemo
{
    public class SceneSwitcherDemo : MonoBehaviour,
        IEventListener<SceneTransitionEnteringEvent>,
        IEventListener<SceneTransitionActiveEvent>,
        IEventListener<SceneTransitionExitingEvent>,
        IEventListener<SceneTransitionInactiveEvent>
    {
        [SerializeField]
        private string sceneName;

        [SerializeField]
        private SceneTransitionData _customSceneTransitionData;

        private ISceneService _sceneService;
        private IEventService _eventService;

        private void Awake()
        {
            _sceneService = ServiceLocator.Get<ISceneService>();
            _eventService = ServiceLocator.Get<IEventService>();

            _eventService.RegisterListener<SceneTransitionEnteringEvent>(this);
            _eventService.RegisterListener<SceneTransitionActiveEvent>(this);
            _eventService.RegisterListener<SceneTransitionExitingEvent>(this);
            _eventService.RegisterListener<SceneTransitionInactiveEvent>(this);
        }

        private void OnDestroy()
        {
            _eventService.UnregisterListener<SceneTransitionEnteringEvent>(this);
            _eventService.UnregisterListener<SceneTransitionActiveEvent>(this);
            _eventService.UnregisterListener<SceneTransitionExitingEvent>(this);
            _eventService.UnregisterListener<SceneTransitionInactiveEvent>(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _sceneService.RestartScene();

            if (Input.GetKeyDown(KeyCode.T))
                _sceneService.RestartScene(_customSceneTransitionData);

            if (Input.GetKeyDown(KeyCode.B))
                _sceneService.GoBackScene();

            if (Input.GetKeyDown(KeyCode.V))
                _sceneService.GoBackScene(_customSceneTransitionData);
        }

        public void OnButtonSwitchDefaultClick()
        {
            _sceneService.ChangeScene(sceneName);
        }
        public void OnButtonSwitchCustomClick()
        {
            _sceneService.ChangeScene(sceneName, _customSceneTransitionData);
        }

        public void OnEventInvoked(SceneTransitionEnteringEvent eventArg)
        {
            // Do something when the scene transition is entering.
            RossoLogger.Info("SceneTransition - Entering ");
        }

        public void OnEventInvoked(SceneTransitionActiveEvent eventArg)
        {
            // Do something when the scene transition is active.
            RossoLogger.Info("SceneTransition - Active");
        }

        public void OnEventInvoked(SceneTransitionExitingEvent eventArg)
        {
            // Do something when the scene transition is exiting.
            RossoLogger.Info("SceneTransition - Exiting");
        }

        public void OnEventInvoked(SceneTransitionInactiveEvent eventArg)
        {
            // Do something when the scene transition is inactive.
            RossoLogger.Info("SceneTransition - Inactive");
        }
    }
}