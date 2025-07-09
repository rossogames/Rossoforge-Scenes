using UnityEngine;

namespace Rossoforge.Scenes.Data
{
    [CreateAssetMenu(fileName = nameof(SceneTransitionData), menuName = "RossoForge/Scenes/TransitionData")]
    public class SceneTransitionData : ScriptableObject
    {
        [field: SerializeField]
        public string TransitionSceneName { get; private set; }
    }
}
