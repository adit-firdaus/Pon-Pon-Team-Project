using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class ToggleObjects : MonoBehaviour
    {
        public bool isOn { get; set; }

        public KeyCode activationKey = KeyCode.F1;
        public List<GameObject> objects;

        private void Start()
        {
            SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(activationKey))
            {
                isOn = !isOn;
                SetActive(isOn);
            }
        }

        /// <summary>
        /// enables or disables all objects in the list (Must be called once not in update)
        /// </summary>
        public void SetActive(bool state)
        {
            isOn = state;
            foreach (GameObject _object in objects) _object.SetActive(isOn);
        }
    }
}