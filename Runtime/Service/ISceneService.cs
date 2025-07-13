using Rossoforge.Core.Services;
using Rossoforge.Scenes.Data;
using UnityEngine;

namespace Rossoforge.Scenes.Service
{
    public interface ISceneService : IService
    {
        bool IsLoading { get; }
        string CurrentSceneName { get; }
        Awaitable LoadScene(string sceneName);
        Awaitable LoadScene(string sceneName, SceneTransitionData sceneTransitionData);
        Awaitable GoBack();
        Awaitable GoBack(SceneTransitionData sceneTransitionData);
        Awaitable Restart();
        Awaitable Restart(SceneTransitionData sceneTransitionData);
        /*
        void UnloadScene(string sceneName, SceneTransitionProfile sceneTransition);
        void UnloadScene(string sceneName);
        void GoBack();
        */
    }
}