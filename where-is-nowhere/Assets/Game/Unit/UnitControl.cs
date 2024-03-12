using DG.Tweening;
using Game.Shared.Bus;
using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class UnitControl : MonoBehaviour, IPlayerUnitControl, IUnitControl {

        private Tweener _smoothTweener;
        private ICameraController _cameraController;
        public IMotor Motor { get; private set; }
        public bool CanFire { get; set; }

        private IActor _actor;

        private bool _firing;   

        public void Init(ICameraController cameraController, IActor actor) {
            _cameraController = cameraController;
            Motor = gameObject.GetComponent<Motor>();

            _actor = actor;

            (Motor as IFPSMotor).Init(cameraController, actor.Animator);

            (_actor as IFireable).FireAnimationCompleted += fireAnimationCompleted;
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
        public void Fire() {
            if (CanFire && _firing == false) {

                _firing = true;
                var unitHit = _cameraController.GetAimHitUnit();

                (_actor as IFireable).FireInDirection(unitHit.origin, unitHit.direction);
            }
        }
        public void Sprint(bool value) => (Motor as IFPSMotor).Sprint = value;
        public void AnalogControl(bool value) => (Motor as IFPSMotor).AnalogControl = value;
        public void Teleport(Vector3 position, bool smooth = false) {
            Motor.Teleport(position, onNavMesh: true, smooth);
        }

        //----------------------------------------------------------------------------------------

        void OnDestroy() {
            (_actor as IFireable).FireAnimationCompleted -= fireAnimationCompleted;
        }

        private void fireAnimationCompleted() {
            _firing = false;

            var unitHit = _cameraController.GetAimHitUnit();

            __.GameBus.Emit(GameEvt.PLAYER_ATTACKED_WITH_UNIT, unitHit.id.Value);
        }
    }
}