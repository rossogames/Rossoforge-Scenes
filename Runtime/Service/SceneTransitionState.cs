using UnityEngine;

namespace Rossoforge.Scenes.Service
{
    public enum SceneTransitionState
    {
        Entering,  // Mostrándose, animando la entrada
        Active,    // Totalmente visible y funcional
        Exiting,   // Ocultándose, animando la salida
        Inactive   // Oculto/inactivo
    }
}
