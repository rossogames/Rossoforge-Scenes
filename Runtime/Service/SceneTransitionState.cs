using UnityEngine;

namespace Rossoforge.Scenes.Service
{
    public enum SceneTransitionState
    {
        Entering,  // Mostr�ndose, animando la entrada
        Active,    // Totalmente visible y funcional
        Exiting,   // Ocult�ndose, animando la salida
        Inactive   // Oculto/inactivo
    }
}
