using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class UnitControl : MonoBehaviour, IUnitControl {

        private Tweener _smoothTweener;
        protected Motor _motor;

        public void Init(ICameraController cameraController, IActor actor) {
            _motor = gameObject.GetComponent<Motor>();

            _motor.Init(cameraController, actor.Animator);
        }
        public void Move(Vector2 move) {

            var moveSimple = new Vector2((float)System.Math.Round(move.x, 2), (float)System.Math.Round(move.y, 2));

            if (_smoothTweener != null) {
                _smoothTweener.Kill();
            }

            _smoothTweener = DOVirtual.Vector3(_motor.Movement, moveSimple, 0.1f, (Vector3 value) => {
                _motor.Movement = value;
            });
        }
        public void Sprint(bool value) => _motor.Sprint = value;
        public void AnalogControl(bool value) => _motor.AnalogControl = value;
        public void SetUnitState(UnitState unitState) {
            Debug.Log("UnitStateControl -> SetUnitState ");
            _motor.SetUnitState(unitState);
        }
        public void Teleport(Vector3 position, bool smooth = false) {
            _motor.Teleport(position, onNavMesh: true, smooth);
        }
    }
}