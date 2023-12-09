using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


namespace FPSFramework
{
    [CreateAssetMenu(fileName = "New Audio Profile")]
    public class AudioProfile : ScriptableObject
    {
        public AudioClip clip;
        public AudioMixerGroup output;
        public bool mute;
        public bool bypassEffects;
        public bool bypassListenerEffects;
        public bool bypassReverbZones;
        public bool playOnAwake = false;
        public bool loop;

        [Space]
        [Range(0, 256)] public int priority = 128;
        [Range(0, 1)] public float volume = 1;
        [Range(-3, 3)] public float pitch = 1;
        [Range(-1, 1)] public float stereoPan = 0;
        [Range(0, 1)] public float spatialBlend = 1;
        [Range(0, 1.1f)] public float reverbZoneMix = 1;

        [Header("3D Sound Settings")]
        [Range(0, 5)] public float dopplerLevel = 1;
        [Range(0, 360)] public float spread = 0;
        public AudioRolloffMode volumeRolloff = AudioRolloffMode.Custom;
        public float minDistance = 1;
        public float maxDistance = 500;

        [Space]
        public bool dymaicPitch = true;
        public float pitchFactor = 0.2f;

        /// <summary>
        /// audio source for this sound
        /// </summary>
        public AudioSource AudioSource { get; set; }

        /// <summary>
        /// updates pitch with its defaults
        /// </summary>
        public void Update()
        {
            if (!AudioSource) return;
            AudioSource.clip = clip;
            AudioSource.outputAudioMixerGroup = output;
            AudioSource.mute = mute;
            AudioSource.bypassEffects = bypassEffects;
            AudioSource.bypassListenerEffects = bypassListenerEffects;
            AudioSource.bypassReverbZones = bypassReverbZones;
            AudioSource.playOnAwake = playOnAwake;
            AudioSource.loop = loop;

            AudioSource.priority = priority;
            AudioSource.volume = volume;
            AudioSource.pitch = pitch;
            AudioSource.panStereo = stereoPan;
            AudioSource.spatialBlend = spatialBlend;
            AudioSource.reverbZoneMix = reverbZoneMix;

            AudioSource.dopplerLevel = dopplerLevel;
            AudioSource.spread = spread;
            AudioSource.rolloffMode = volumeRolloff;
            AudioSource.minDistance = minDistance;
            AudioSource.maxDistance = maxDistance;
        }

        public void UpdatePitch()
        {
            AudioSource.pitch = Time.timeScale * pitch;
        }

        /// <summary>
        /// sets sound pitch to random in order add some sounds variety without using too much audio clips 
        /// </summary>
        public void RandomizePitch()
        {
            AudioSource.pitch = RandomizedPitch();
        }

        /// <summary>
        /// returns random value bettwen using time scale
        /// </summary>
        /// <returns></returns>
        public float RandomizedPitch()
        {
            if (dymaicPitch) return Random.Range((Time.timeScale * pitch), (Time.timeScale + pitchFactor) * pitch);
            return pitch;
        }
    }
}