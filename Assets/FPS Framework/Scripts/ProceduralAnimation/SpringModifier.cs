using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace FPSFramework.Animation
{
    [AddComponentMenu("FPS Engine/Animation/Modifiers/Spring Modifier"), RequireComponent(typeof(ProceduralAnimation))]
    public class SpringModifier : ProceduralAnimationModifier
    {
        public BouncyVector3 position = new BouncyVector3();
        public BouncyVector3 rotation = new BouncyVector3();

        [Space]
        public UnityEvent OnTrigger;

        private void Update()
        {
            position.Update();
            rotation.Update();

            positionOffset = position.result;
            rotationOffset = rotation.result;
        }

        public override void Trigger()
        {
            position.Start();
            rotation.Start();
            OnTrigger?.Invoke();
        }
    }
}