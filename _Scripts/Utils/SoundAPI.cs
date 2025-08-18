using DragonLi.Core;
using UnityEngine;

namespace Game
{
    public abstract class SoundAPI
    {
        public static AudioSource PlaySound(AudioClip clip)
        {
            return clip == null ? null : SpawnSound2D(AudioInstance.Instance.Settings.SoundPrefab, clip);
        }
        
        private static AudioSource SpawnSound2D(GameObject soundContainer, AudioClip sound)
        {
            var soundObj = SpawnManager.Instance.GetObjectFromPool(soundContainer);
            var audioClip = soundObj.GetGameObject().GetComponent<AudioSource>();
            audioClip.clip = sound;
            audioClip.Play();
            return audioClip;
        }
    }
}