using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Shared.Components {
    public class Actor : MonoBehaviour, IActor {
        public int ID { get; private set; }
        public Animator Animator { get; private set; }

        public virtual void Init(int id) {
            ID = id;
            Animator = GetComponent<Animator>();
        }

        public void SetActive(bool active = true) {
            gameObject.SetActive(active);
        }
    }
}