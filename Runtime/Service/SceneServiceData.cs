using Rossoforge.Scenes.Data;
using UnityEngine;

namespace Rossoforge.Scenes.Service
{
    [CreateAssetMenu(fileName = nameof(SceneServiceData), menuName = "Rossoforge/Scenes/Service Data")]
    public class SceneServiceData : ScriptableObject
    {
        [field: SerializeField]
        public SceneTransitionData DefaultSceneTransitionData { get; private set; }
    }
}
