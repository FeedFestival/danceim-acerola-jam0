using Game.Interactable;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Interactables {
    public class LockedGateInteractable : BasicDoorInteractable, ILockable {

        [Header("Locked Gate")]
        [SerializeField]
        private Transform _lockingPipe;
        [SerializeField]
        private Vector3 _lockedPos;
        [SerializeField]
        private Vector3 _unlockedPos;
        [SerializeField]
        private bool _locked;

        public override void Init() {
            base.initEntityId();
            base.initFocusTrigger();

            lockDoor(_locked, true);
        }

        public override void DoDefaultInteraction(IPlayer player) {

            if (_locked) { return; }

            base.DoDefaultInteraction(player);
        }

        public void Lock(bool locked = true) {
            _locked = locked;
            lockDoor(_locked, true);
        }

        //---------------------------------------------------------------------------------------

        private void lockDoor(bool locked = true, bool instant = false) {

            if (instant) {
                if (locked) {
                    _lockingPipe.localPosition = _lockedPos;
                } else {
                    _lockingPipe.localPosition = _unlockedPos;
                }
            } else {
                // do it tweened
            }
        }
    }
}