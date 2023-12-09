using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework.Animation
{
    [AddComponentMenu("FPS Engine/Animation/Procedural Animation Trigger")]
    public class ProceduralAnimationTrigger : MonoBehaviour
    {
        public TriggerType type = TriggerType.None;
        public KeyCode triggerKey = KeyCode.None;


        public ProceduralAnimation Animation { get; private set; }

        private bool isTriggered;

        private void Start()
        {
            Animation = GetComponent<ProceduralAnimation>();
        }

        private void Update()
        {
            switch(type)
            {
                case TriggerType.Toggle:
                    if (Input.GetKeyDown(triggerKey)) isTriggered = !isTriggered;
                    break;

                case TriggerType.Hold:
                    isTriggered = Input.GetKey(triggerKey);
                    break;
            }

            Animation?.SetTrigger(isTriggered);
        }

        public enum TriggerType
        {
            None = 0,
            Toggle = 1,
            Hold = 2,
        }
    }
}