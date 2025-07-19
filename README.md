# Rosso Games

<table>
  <tr>
    <td><img src="https://github.com/rossogames/Rossoforge-Scenes/blob/main/logo.png?raw=true" alt="Rossoforge" width="64"/></td>
    <td><h2>Rossoforge-Scenes</h2></td>
  </tr>
</table>

**Rossoforge-Scenes** A lightweight and flexible service that centralizes scene loading, unloading, and transitions, ensuring smooth and efficient navigation between scenes. It is designed to simplify and streamline scene management in Unity projects

**Version**: Unity 6 or higher

**Tutorial**: [Pending...]

**Dependencies**
* [Rossoforge-Core](https://github.com/rossogames/Rossoforge-Core.git)
* [Rossoforge-Events](https://github.com/rossogames/Rossoforge-Events.git)
* [Rossoforge-Services](https://github.com/rossogames/Rossoforge-Services.git) (Optional)

#

```csharp
// Setup
ServiceLocator.SetLocator(new DefaultServiceLocator());

var eventService = new EventService();
var sceneService = new SceneService(eventService, _sceneTransitionData);

ServiceLocator.Register<IEventService>(eventService);
ServiceLocator.Register<ISceneService>(sceneService);
ServiceLocator.Initialize();

// Anywhere in your code
var myService = ServiceLocator.Get<ISceneService>();

// Change scene displaying transition scene
_sceneService.ChangeScene(sceneName);
_sceneService.ChangeScene(sceneName, _customSceneTransitionData);

// Load and undload scenes immediately without showing transition effect
_sceneService.UnloadScene(sceneName);
_sceneService.LoadScene(sceneName, LoadSceneMode.Additive);

// Reload the current scene displaying transition scene
_sceneService.RestartScene();
_sceneService.RestartScene(_customSceneTransitionData);

// Load the previous scene displaying transition scene
_sceneService.GoBackScene();
_sceneService.GoBackScene(_customSceneTransitionData);
```

#
This package is part of the **Rossoforge** suite, designed to streamline and enhance Unity development workflows.

Developed by Agustin Rosso
https://www.linkedin.com/in/rossoagustin/
