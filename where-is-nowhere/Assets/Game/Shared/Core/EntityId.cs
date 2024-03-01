using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Shared.Core {
    public class EntityId : MonoBehaviour, IEntityId {
        [SerializeField]
        private int _id;

        public int Id { get => _id; }

        public void DestroyComponent() {
            Destroy(this);
        }
    }
}