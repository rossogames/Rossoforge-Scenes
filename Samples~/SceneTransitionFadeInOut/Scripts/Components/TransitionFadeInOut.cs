using Rossoforge.Events.Service;
using Rossoforge.Scenes.Components;
using Rossoforge.Services.Locator;
using UnityEngine;

namespace Rossoforge.Scenes.Samples.SceneTransitionFadeInOut
{
    [RequireComponent(typeof(Animator))]
    public class TransitionFadeInOut : SceneTransition
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