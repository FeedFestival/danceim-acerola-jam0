using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class Actor : MonoBehaviour, IActor {

        public Animator Animator { get; private set; }

        public void Init() {
            Animator = GetComponent<Animator>();
        }

        public void SetActive(bool active) {

        }
    }
}