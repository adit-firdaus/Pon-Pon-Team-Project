using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace FPSFramework
{
    public class Interactable : MonoBehaviour, IInteractable
    {
        public string interactionName = "Interact";
        public KeyCode InteractionKey = KeyCode.F;
        public UnityEvent OnInteract;

        public KeyCode GetInteractionKey()
        {
            return InteractionKey;
        }

        public string GetInteractionName()
        {
            return interactionName;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void Interact(InteractablesManager source)
        {
            OnInteract?.Invoke();
        }
    }
}