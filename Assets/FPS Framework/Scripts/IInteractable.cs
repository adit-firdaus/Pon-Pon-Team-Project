using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public interface IInteractable
    {
        void Interact(InteractablesManager source);
        Transform transform { get; }
        string GetInteractionName();
        KeyCode GetInteractionKey();
    }
}