using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class Unit : MonoBehaviour, IUnit {

        [SerializeField]
        private GameObject _actorGo;
        private IUnitControl _unitControl;
        [SerializeField]
        protected UnitState _unitState;

        public int ID { get; private set; }
        public string Name { get => gameObject.name; }
        public Transform Transform { get => transform; }
        public IUnitControl UnitControl => _unitControl;
        public IActor Actor { get; private set; }

        public virtual void Init() {
            initEntityId();
            tryInitActor();

            _unitControl = GetComponent<IUnitControl>();
        }

        public virtual void SetUnitState(UnitState unitState) {

            if (_unitState == unitState) { return; }
            setUnitState(unitState);

            _unitControl.SetUnitState(_unitState);
        }

        protected void initEntityId() {
            var entity = gameObject.GetComponent<IEntityId>();
            ID = entity.Id;
            Debug.Log("ID: " + ID);
            entity.DestroyComponent();
        }

        protected void tryInitActor() {
            if (_actorGo != null) {
                Actor = _actorGo.GetComponent<IActor>();
                _actorGo = null;
                if (Actor != null) {
                    Actor.Init();
                }
            }
        }

        protected void setUnitState(UnitState unitState) {
            _unitState = unitState;
            Debug.Log("_unitState: " + _unitState);

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