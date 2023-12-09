using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPSFramework
{
    public static class AudioManager
    {
        /// <summary>
        /// sets up the sound with profile settings this is recommended to be in OnEnable function
        /// </summary>
        /// <param name="target">target source of sound</param>
        /// <param name="profile">target sound with settings</param>
        public static void EquipAudio(GameObject target, AudioProfile profile)
        {
            profile.AudioSource = target.AddComponent<AudioSource>();
            profile.Update();
        }

        /// <summary>
        /// plays a sound with it's profile settings
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        public static void Play(AudioProfile profile)
        {
            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.Play();
        }

        /// <summary>
        /// plays a sound with it's profile settings and if sound isn't equiped equips it
        /// </summary>
        /// <param name="target">target source of sound</param>
        /// <param name="profile">target sound with settings</param>
        public static void Play(GameObject target, AudioProfile profile)
        {
            if (!profile.AudioSource) EquipAudio(target, profile);

            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.Play();
        }

        /// <summary>
        /// plays a sound with it's profile settings
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        /// <param name="clip">target audio clip to play</param>
        public static void Play(AudioProfile profile, AudioClip clip)
        {
            AudioClip prevClip = profile.clip;

            profile.clip = clip;
            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.Play();

            profile.clip = prevClip;
        }

        /// <summary>
        /// plays a sound with it's profile settings and if sound isn't equiped equips it
        /// </summary>
        /// <param name="target">target source of sound</param>
        /// <param name="profile">target sound with settings</param>
        /// <param name="clip">target audio clip to play</param>
        public static void Play(GameObject target, AudioProfile profile, AudioClip clip)
        {
            if (!profile.AudioSource) EquipAudio(target, profile);

            AudioClip prevClip = profile.clip;

            profile.clip = clip;
            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.Play();

            profile.clip = prevClip;
        }

        /// <summary>
        /// plays a sound with it's profile settings
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        public static void PlayOneShot(AudioProfile profile)
        {
            profile.Update();
            profile.RandomizePitch();

            if (!profile.clip)
            {
                return;
            }

            profile.AudioSource.PlayOneShot(profile.clip);
        }

        /// <summary>
        /// plays a sound with it's profile settings and if sound isn't equiped equips it
        /// </summary>
        /// <param name="target">target source of sound</param>
        /// <param name="profile">target sound with settings</param>
        public static void PlayOneShot(GameObject target, AudioProfile profile)
        {
            if (!profile.AudioSource) EquipAudio(target, profile);

            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.PlayOneShot(profile.clip);
        }

        /// <summary>
        /// plays a sound with it's profile settings
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        /// <param name="clip">target audio clip to play</param>
        public static void PlayOneShot(AudioProfile profile, AudioClip clip)
        {
            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// plays a sound with it's profile settings and if sound isn't equiped equips it
        /// </summary>
        /// <param name="target">target source of sound</param>
        /// <param name="profile">target sound with settings</param>
        /// <param name="clip">target audio clip to play</param>
        public static void PlayOneShot(GameObject target, AudioProfile profile, AudioClip clip)
        {
            if (!profile.AudioSource) EquipAudio(target, profile);

            profile.Update();
            profile.RandomizePitch();
            profile.AudioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// pauses playing target sound
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        public static void Pause(AudioProfile profile)
        {
            profile.AudioSource.Pause();
        }

        /// <summary>
        /// unpauses target sound
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        public static void Unpause(AudioProfile profile)
        {
            profile.AudioSource.UnPause();
        }

        /// <summary>
        /// stops playing target audio
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        public static void Stop(AudioProfile profile)
        {
            profile.AudioSource.Stop();
        }

        /// <summary>
        /// updates sound with its defaults
        /// </summary>
        /// <param name="profile"></param>
        public static void Update(AudioProfile profile)
        {
            profile.Update();
        }

        /// <summary>
        /// updates sound pitch with time scale 
        /// </summary>
        /// <param name="profile"></param>
        public static void UpdatePitch(AudioProfile profile)
        {
            profile.UpdatePitch();
        }

        /// <summary>
        /// returns audio source used to play audio with the profile settings
        /// </summary>
        /// <param name="profile">target sound with settings</param>
        /// <returns></returns>
        public static AudioSource GetAudioSource(AudioProfile profile)
        {
            return profile.AudioSource;
        }
    }
}