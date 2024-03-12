using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IActor {
        int ID { get; }
        Animator Animator { get; }

        void Init(int id);

        void SetActive(bool active = true);
    }

    //

    public interface IPatientActor: IActor {

        void Init(IInventory inventory);
    }
}