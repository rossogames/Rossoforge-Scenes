using Rossoforge.Scenes.DataConfig;
using UnityEngine;

namespace Rossoforge.Scenes.Service
{
    [CreateAssetMenu(fileName = nameof(SceneDataService), menuName = "Rossoforge/Data Service/Scenes")]
    public class SceneDataService : ScriptableObject
    {
        [field: SerializeField]
        public SceneTransitionDataConfig DefaultSceneTransition { get; private set; }
    }
}
