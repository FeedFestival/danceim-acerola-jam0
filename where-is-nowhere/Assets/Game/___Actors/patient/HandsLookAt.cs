using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Actors {
    public class HandsLookAt : MonoBehaviour {

        [SerializeField]
        private Transform _lookAt;

        [SerializeField]
        private Transform _copyPosition;

        [SerializeField]
        private Vector3 _eulerAnglesOffset;

        void LateUpdate() {
            if (_lookAt == null) {
                return;
            }
            transform.LookAt(_lookAt.position);
            transform.localEulerAngles = transform.localEulerAngles + _eulerAnglesOffset;
            //transform.position = _copyPosition.position;
        }
    }
}