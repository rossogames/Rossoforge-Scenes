using Rossoforge.Core.Events;
using Rossoforge.Scenes.Events;
using UnityEngine;

namespace Rossoforge.Scenes
{
    public abstract class SceneTransition : MonoBehaviour,
        IEventListener<TargetSceneLoadedCompletedEvent>
    {
        protected IEventService _eventService;

        protected virtual void Start()
        {
            _eventService.RegisterListener<TargetSceneLoadedCompletedEvent>(this);
        }
        protected virtual void OnDestroy()
        {
            _eventService.UnregisterListener<TargetSceneLoadedCompletedEvent>(this);
        }

        public virtual void OnTransitionEntering()
        {
            _eventService.Raise<SceneTransitionEnteringEvent>();
        }

        public virtual void OnTransitionActive()
        {
            _eventService.Raise<SceneTransitionActiveEvent>();
        }

        public virtual void OnTransitionExiting()
        {
            _eventService.Raise<SceneTransitionExitingEvent>();
        }

        public virtual void OnTransitionInactive()
        {
            _eventService.Raise<SceneTransitionInactiveEvent>();
        }

        public void OnEventInvoked(TargetSceneLoadedCompletedEvent eventArg)
        {
            OnTargetSceneLoadedCompletedEvent();
        }

        protected abstract void OnTargetSceneLoadedCompletedEvent();
    }
}
