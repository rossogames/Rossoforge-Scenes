using Rossoforge.Core.Services;
using Rossoforge.Scenes.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rossoforge.Scenes.Service
{
    public interface ISceneService : IService
    {
        bool IsLoading { get; }
        string CurrentSceneName { get; }
        Awaitable ChangeScene(string sceneName);
        Awaitable ChangeScene(string sceneName, SceneTransitionData sceneTransitionData);
        Awaitable LoadScene(string sceneName, LoadSceneMode loadSceneMode);
        Awaitable UnloadScene(string sceneName);
        Awaitable GoBackScene();
        Awaitable GoBackScene(SceneTransitionData sceneTransitionData);
        Awaitable RestartScene();
        Awaitable RestartScene(SceneTransitionData sceneTransitionData);
    }
}