using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class Unit : MonoBehaviour, IUnit {

        [SerializeField]
        private GameObject _actorGo;
        private UnitControl _unitControl;
        [SerializeField]
        private UnitState _unitState;
        [SerializeField]
        private Transform _spineToOrientate;

        public int ID { get; private set; }
        public string Name { get => gameObject.name; }
        public Transform Transform { get => transform; }
        public IUnitControl UnitControl => _unitControl;
        public IActor Actor { get; private set; }
        public Transform SpineToOrientate { get => _spineToOrientate; }

        public void Init() {
            var entity = gameObject.GetComponent<IEntityId>();
            ID = entity.Id;
            entity.DestroyComponent();

            _unitControl = GetComponent<UnitControl>();

            if (_actorGo != null) {
                Actor = _actorGo.GetComponent<IActor>();
                _actorGo = null;
                if (Actor != null) {
                    Actor.Init();
                }
            }
        }

        public void SetUnitState(UnitState unitState) {

            if (_unitState == unitState) { return; }
            _unitState = unitState;

            Debug.Log("Unit -> SetUnitState " + _unitState);
            _unitControl.SetUnitState(_unitState);

            switch (_unitState) {
                case UnitState.FreePlaying:
                case UnitState.Interacting:

                    Actor.SetActive(true);

                    break;
                case UnitState.Hidden:
                default:

                    Actor.SetActive(false);

                    break;
            }
        }
    }
}