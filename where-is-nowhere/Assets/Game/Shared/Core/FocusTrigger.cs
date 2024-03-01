using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Shared.Core {
    public class FocusTrigger : MonoBehaviour, IFocusTrigger {

        public int ID { get; private set; }
        public void Init(int id) {
            ID = id;
        }


    }
}