using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class PossiblePositions : MonoBehaviour {
        private Bounds[] _boxBounds;

        public void Init() {
            var boxBoundsList = new List<Bounds>();
            foreach (Transform child in transform) {
                var boxCollider = child.GetComponent<BoxCollider>();
                boxBoundsList.Add(boxCollider.bounds);
            }

            _boxBounds = boxBoundsList.ToArray();
        }

        public Vector3 GetRandomPoint() {
            var index = Random.Range(0, _boxBounds.Length);
            return new Vector3(
                Random.Range(_boxBounds[index].min.x, _boxBounds[index].max.x),
                Random.Range(_boxBounds[index].min.y, _boxBounds[index].max.y),
                Random.Range(_boxBounds[index].min.z, _boxBounds[index].max.z)
            );
        }
    }
}