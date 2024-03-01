using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IActor {

        Animator Animator { get; }

        void Init();

        void SetActive(bool active);
    }
}