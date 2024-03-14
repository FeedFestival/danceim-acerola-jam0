using Game.Shared.Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ISoundManager {
        void Init();   
    }
    public interface IAmbientSoundManager: ISoundManager {
        void PlayAmbient(AmbientSFXName soundName);
    }
    public interface ISFXSoundManager: ISoundManager {
        void PlaySoundFx(SFXName soundName);
        AudioClip GetSoundFxClip(SFXName soundName);
    }
}