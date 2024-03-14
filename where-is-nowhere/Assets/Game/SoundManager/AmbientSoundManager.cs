using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Sound {
    public class AmbientSoundManager : MonoBehaviour, IAmbientSoundManager {

        [Serializable]
        public struct SoundAmbient {
            public AmbientSFXName name;
            public AudioClip audioClip;
        }
        [SerializeField]
        private List<SoundAmbient> _soundFx;
        private Dictionary<AmbientSFXName, AudioClip> _ambientAudioCLip;

        protected AudioSource _fxAudioSource;

        public void Init() {

            Debug.Log("IAmbientSoundManager.Init: ");

            _fxAudioSource = GetComponent<AudioSource>();

            _ambientAudioCLip = new Dictionary<AmbientSFXName, AudioClip>();
            foreach (var soundFx in _soundFx) {
                _ambientAudioCLip.Add(soundFx.name, soundFx.audioClip);
            }
            _soundFx = null;
        }

        public void PlayAmbient(AmbientSFXName soundName) {
            Debug.Log("PlayAmbient.soundName: " + soundName);
            _fxAudioSource.clip = _ambientAudioCLip[soundName];
            //_fxAudioSource.loop = false;
            _fxAudioSource.Play();
        }
    }
}