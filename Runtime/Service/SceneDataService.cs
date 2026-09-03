using Rossoforge.Scenes.Data;
using UnityEngine;

namespace Rossoforge.Scenes.Service
{
    [CreateAssetMenu(fileName = nameof(SceneDataService), menuName = "Rossoforge/Data Service/Scenes")]
    public class SceneDataService : ScriptableObject
    {
        [field: SerializeField]
        public SceneTransitionData DefaultSceneTransitionData { get; private set; }
    }
}
