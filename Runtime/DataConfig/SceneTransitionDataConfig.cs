using UnityEngine;

namespace Rossoforge.Scenes.DataConfig
{
    [CreateAssetMenu(fileName = nameof(SceneTransitionDataConfig), menuName = "Rossoforge/Data Config/Scenes/Transition")]
    public class SceneTransitionDataConfig : ScriptableObject, ISceneTransitionDataConfig
    {
        [field: SerializeField]
        public string TransitionSceneName { get; private set; }
    }
}
