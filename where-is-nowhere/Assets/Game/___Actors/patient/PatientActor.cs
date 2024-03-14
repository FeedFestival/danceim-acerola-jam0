using DG.Tweening;
using Game.Shared.Components;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System;
using UnityEngine;

namespace Game.Actors {
    public class PatientActor : Actor, IPatientActor, IFireable {

        [Header("Patient Actor")]
        [SerializeField]
        private DetashedHands _detashedHands;
        [SerializeField]
        private Animator _tongueAnimator;
        [SerializeField]
        private Vector3 _tonguePosOffset = new Vector3(0, -0.15f, -0.1f);
        [SerializeField]
        private Vector3 _tongueRotOffset = new Vector3(14, 0, 0);

        public Action FireAnimationCompleted { get; set; }

        public void Init(IInventory inventory) {
            
            _detashedHands.ShowRightHand(false);

            inventory.InventoryItemAdded += inventoryItemAdded;
            inventory.InventoryItemRemoved += inventoryItemRemoved;

            _tongueAnimator.gameObject.SetActive(false);
        }

        private void inventoryItemRemoved(InventoryItem inventoryItem) {

            if (inventoryItem == InventoryItem.RightHand) {
                _detashedHands.ShowRightHand(false);
            }
        }

        private void inventoryItemAdded(InventoryItem inventoryItem) {
            if (inventoryItem == InventoryItem.RightHand) {
                _detashedHands.ShowRightHand();
            }
        }

        public void FireInDirection(Vector3 origin, Vector3 direction) {
            var fireAnimLength = 1.625f;
            var fireSpeed = UnityEngine.Random.Range(1.7f, 3f);
            var duration = fireAnimLength / fireSpeed;

            _tongueAnimator.transform.position = origin;
            _tongueAnimator.transform.localPosition
                = _tongueAnimator.transform.localPosition + _tonguePosOffset;
            var target = origin - direction;
            _tongueAnimator.transform.LookAt(target);
            _tongueAnimator.transform.localEulerAngles
                = _tongueAnimator.transform.localEulerAngles + _tongueRotOffset;

            _tongueAnimator.gameObject.SetActive(true);
            _tongueAnimator.Play("Fire");
            _tongueAnimator.SetFloat("FireSpeed", fireSpeed);

            DOTween.Sequence()
                .SetDelay(duration)
                .AppendCallback(() => {
                    _tongueAnimator.Play("Empty");
                    _tongueAnimator.gameObject.SetActive(false);
                    FireAnimationCompleted?.Invoke();
                });
        }
    }
}