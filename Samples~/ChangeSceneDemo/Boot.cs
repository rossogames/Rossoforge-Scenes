using Rossoforge.Core.Events;
using Rossoforge.Events.Service;
using Rossoforge.Scenes.Data;
using Rossoforge.Scenes.Service;
using Rossoforge.Services;
using UnityEngine;

namespace Rossoforge.Scenes.Samples.ChangeSceneDemo
{
    public class Boot : MonoBehaviour
    {
        [SerializeField]
        private SceneTransitionData _sceneTransitionData;

        private void Awake()
        {
            // Setup
            ServiceLocator.SetLocator(new DefaultServiceLocator());

            var eventService = new EventService();
            var sceneService = new SceneService(eventService, _sceneTransitionData);

            ServiceLocator.Register<IEventService>(eventService);
            ServiceLocator.Register<ISceneService>(sceneService);
            ServiceLocator.Initialize();

            sceneService.ChangeScene("SceneTransitionA");
        }

    }
}