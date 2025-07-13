using Rossoforge.Core.Events;
using Rossoforge.Services;
using UnityEngine;

namespace Rossoforge.Scenes.Samples.SceneTransitionSideLeftToRight
{
    [RequireComponent(typeof(Animator))]
    public class TransitionSideLeftToRight : SceneTransition
    {
        [HideInInspector] public Animator Animator;

        private void Awake()
        {
            _eventService = ServiceLocator.Get<IEventService>();
            Animator = GetComponent<Animator>();
        }

        override protected void OnTargetSceneLoadedCompletedEvent()
        {
            Animator.SetTrigger("Close");
        }
    }
}