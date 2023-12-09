using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework.Animation
{
    [System.Serializable]
    public class BouncyVector3
    {
        public Vector3 value;
        public float speed = 10;
        public float fadeOutTime = 1;
        [Range(0, 1)] public float weight = 1;

        public float time { get; set; }
        public float progress { get; set; }
        public Vector3 result { get; set; }
        private float velocity;

        public void Update()
        {
            time += Time.deltaTime * speed;

            float pos = 0;

            pos += Mathf.Sin(time) * progress;

            progress = Mathf.SmoothDamp(progress, 0, ref velocity, fadeOutTime);

            result = new Vector3(pos * value.x, pos * value.y, pos * value.z) * progress;
        }

        public void Start()
        {
            progress = 1 * weight;
            time = 0;
        }
    }
}