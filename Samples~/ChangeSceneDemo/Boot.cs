using Rossoforge.Core.Events;
using Rossoforge.Core.Scenes;
using Rossoforge.Events.Service;
using Rossoforge.Scenes.Service;
using Rossoforge.Services;
using UnityEngine;

namespace Rossoforge.Scenes.Samples.ChangeSceneDemo
{
    public class Boot : MonoBehaviour
    {
        [SerializeField]
        private SceneServiceData _sceneServiceData;

        private void Awake()
        {
            // Setup
            ServiceLocator.SetLocator(new DefaultServiceLocator());

            var eventService = new EventService();
            var sceneService = new SceneService(eventService, _sceneServiceData);

            ServiceLocator.Register<IEventService>(eventService);
            ServiceLocator.Register<ISceneService>(sceneService);
            ServiceLocator.Initialize();

            sceneService.ChangeScene("SceneTransitionA");
        }

    }
}