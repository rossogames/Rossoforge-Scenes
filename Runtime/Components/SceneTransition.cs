using Rossoforge.Core.Events;
using UnityEngine;

namespace Rossoforge.Scenes.Components
{ 
    [RequireComponent(typeof(Animator))]
    public class SceneTransition : MonoBehaviour
        //IEventListener<SceneTransitionCloseEvent>
    {
        [HideInInspector] public Animator Animator;

        private IEventService _eventService;

        private void Awake()
        {
            //_eventService = ServiceLocator.Current.Get<IEventService>();
            Animator = GetComponent<Animator>();

            //_eventService.RegisterListener(this);
        }
        private void OnDestroy()
        {
            //_eventService.UnregisterListener(this);
        }

        public void OnTransitionEntering()
        {
            Debug.LogWarning("Entering");
            //_eventService.Raise<SceneTransitionStandByEvent>();
        }

        public void OnTransitionActive()
        {
            Debug.LogWarning("Active");
            //_eventService.Raise<SceneTransitionCompletedEvent>();
        }

        public void OnTransitionExiting()
        {
            Debug.LogWarning("Exiting");
            //_eventService.Raise<SceneTransitionStartingEvent>();
        }

        public void OnTransitionInactive()
        {
            Debug.LogWarning("Inactive");
            //_eventService.Raise<SceneTransitionEndingEvent>();
        }

        //public void OnEventInvoked(SceneTransitionCloseEvent eventArg)
        //{
        //    Animator.SetTrigger("Close");
        //}
    }
}