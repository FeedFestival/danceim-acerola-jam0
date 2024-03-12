using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actors {
    public class DetashedHands : MonoBehaviour {

        [SerializeField]
        private GameObject _rightHand;

        internal void ShowRightHand(bool show = true) {
            _rightHand.SetActive(show);
        }
    }
}