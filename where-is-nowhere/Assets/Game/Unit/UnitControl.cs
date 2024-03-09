using DG.Tweening;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class UnitControl : MonoBehaviour, IPlayerUnitControl, IUnitControl {

        private Tweener _smoothTweener;

        public IMotor Motor { get; private set; }

        public void Init(ICameraController cameraController, IActor actor) {
            Motor = gameObject.GetComponent<Motor>();

            (Motor as IFPSMotor).Init(cameraController, actor.Animator);
        }
        public void Move(Vector2 move) {

            var moveSimple = new Vector2((float)System.Math.Round(move.x, 2), (float)System.Math.Round(move.y, 2));

            if (_smoothTweener != null) {
                _smoothTweener.Kill();
            }

            _smoothTweener = DOVirtual.Vector3((Motor as IFPSMotor).Movement, moveSimple, 0.1f, (Vector3 value) => {
                (Motor as IFPSMotor).Movement = value;
            });
        }
        public void Sprint(bool value) => (Motor as IFPSMotor).Sprint = value;
        public void AnalogControl(bool value) => (Motor as IFPSMotor).AnalogControl = value;
        public void SetUnitState(UnitState unitState) {
            (Motor as IFPSMotor).SetUnitState(unitState);
        }
        public void Teleport(Vector3 position, bool smooth = false) {
            (Motor as IFPSMotor).Teleport(position, onNavMesh: true, smooth);
        }
    }
}