using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Shared.Components {
    public class Actor : MonoBehaviour, IActor {

        public Animator Animator { get; private set; }

        public virtual void Init() {
            Animator = GetComponent<Animator>();
        }

        public void SetActive(bool active) {

        }
    }
}