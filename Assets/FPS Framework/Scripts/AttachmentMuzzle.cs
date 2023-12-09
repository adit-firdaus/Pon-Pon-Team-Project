using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    [AddComponentMenu("Weapons/Attachments/Muzzle")]
    public class AttachmentMuzzle : MonoBehaviour
    {
        public AudioProfile fireSFX;
        public ParticleSystem[] muzzleEffects;
    }
}