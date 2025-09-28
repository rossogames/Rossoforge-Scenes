using Rossoforge.Core.Scenes;
using UnityEngine;

namespace Rossoforge.Scenes.Data
{
    [CreateAssetMenu(fileName = nameof(SceneTransitionData), menuName = "Rossoforge/Scenes/Transition Data")]
    public class SceneTransitionData : ScriptableObject, ISceneTransitionData
    {
        [field: SerializeField]
        public string TransitionSceneName { get; private set; }
    }
}
