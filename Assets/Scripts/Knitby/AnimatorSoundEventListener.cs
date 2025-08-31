using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animation
{
    [RequireComponent(typeof(AudioClip))]
    public class AnimatorSoundEventListener : MonoBehaviour
    {
        [SerializeField] private AudioClip sound;
        private AudioSource _audioSource;

        private void Awake() => _audioSource = GetComponent<AudioSource>();
        public void OnPlaySound() => _audioSource.PlayOneShot(sound);
    }
}
