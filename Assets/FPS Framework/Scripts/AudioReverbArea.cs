using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace FPSFramework
{
    public class AudioReverbArea : MonoBehaviour
    {
        public AudioReverbPreset preset;

        private AudioFiltersManager AudioFiltersManager;

        private void OnTriggerStay(Collider collider)
        {
            if (collider.GetComponent<ICharacterController>() != null)
            {
                AudioFiltersManager = collider.GetComponent<ICharacterController>().GetCameraManager().audioFiltersManager;
                AudioFiltersManager.SetReverp(preset);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (AudioFiltersManager)
                AudioFiltersManager.ResetReverp();
        }
    }
}