using Game.Shared.Components;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Actors {
    public class Actor_RightHand : Actor {

        public override void Init() {
            base.Init();

            Debug.Log("Actor_RightHand -> Animator: " + Animator);

            Animator.Play("Idle");
        }
    }
}
