using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Zone {
    public class Zone : MonoBehaviour, IZone {
        public int ID { get; private set; }
        public Action Entered { get; set; }
        public Action Exited { get; set; }

        public void Init() {
            var entity = gameObject.GetComponent<IEntityId>();
            ID = entity.Id;
            Debug.Log("ID: " + ID);
            entity.DestroyComponent();
        }

        public void EmitEntered() {
            Entered?.Invoke();
        }

        public void EmitExited() {
            Exited?.Invoke();
        }
    }
}