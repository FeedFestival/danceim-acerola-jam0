using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sound {
    public class SoundManager : MonoBehaviour, ISFXSoundManager {

        [Serializable]
        public struct SoundFX {
            public SFXName name;
            public AudioClip audioClip;
        }
        [SerializeField]
        private List<SoundFX> _soundFx;
        private Dictionary<SFXName, AudioClip> _audioCLip;

        protected AudioSource _fxAudioSource;

        public void Init() {

            _fxAudioSource = GetComponent<AudioSource>();

            _audioCLip = new Dictionary<SFXName, AudioClip>();
            foreach (var soundFx in _soundFx) {
                _audioCLip.Add(soundFx.name, soundFx.audioClip);
            }
            _soundFx = null;
        }

        public void PlaySoundFx(SFXName soundName) {
            Debug.Log("PlaySoundFx.soundName: " + soundName);
            _fxAudioSource.clip = GetSoundFxClip(soundName);
            _fxAudioSource.loop = false;
            _fxAudioSource.Play();
        }

        public AudioClip GetSoundFxClip(SFXName soundName) {
            return _audioCLip[soundName];
        }
    }
}