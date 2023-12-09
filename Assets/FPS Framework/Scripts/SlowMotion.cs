using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public class SlowMotion : MonoBehaviour
    {
        public KeyCode slowMoKey = KeyCode.CapsLock;
        public CanvasGroup slowMotionHUD;
        public float timeScale = 0.5f;
        public float fixedDeltaTime = 0.02f;
        public float smoothness = 5;
        public bool isOn;

        private void Update()
        {
            if (Input.GetKeyDown(slowMoKey))
            {
                isOn = !isOn;
            }

            if (isOn)
            {
                Time.timeScale = Mathf.SmoothStep(Time.timeScale, timeScale, Time.deltaTime * smoothness);
                Time.fixedDeltaTime = Mathf.SmoothStep(Time.fixedDeltaTime, fixedDeltaTime, Time.deltaTime * smoothness);
                if (slowMotionHUD) slowMotionHUD.alpha = Mathf.SmoothStep(slowMotionHUD.alpha, 1, Time.deltaTime * smoothness);
            }
            else
            {
                Time.timeScale = Mathf.SmoothStep(Time.timeScale, 1, Time.deltaTime * smoothness);
                Time.fixedDeltaTime = Mathf.SmoothStep(Time.fixedDeltaTime, 0.02f, Time.deltaTime * smoothness);
                if (slowMotionHUD) slowMotionHUD.alpha = Mathf.SmoothStep(slowMotionHUD.alpha, 0, Time.deltaTime * smoothness);
            }
        }
    }
}