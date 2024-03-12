using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class Unit : MonoBehaviour, IUnit {

        [SerializeField]
        private GameObject _actorGo;
        [SerializeField]
        protected UnitState _unitState;

        public int ID { get; private set; }
        public string Name { get => gameObject.name; }
        public Transform Transform { get => transform; }
        public virtual IUnitControl UnitControl { get; protected set; }
        public IActor Actor { get; protected set; }

        public virtual void Init() {
            initEntityId();
            tryLoadActor();
            tryInitActor();

            UnitControl = GetComponent<IUnitControl>();
        }

        public virtual void SetUnitState(UnitState unitState) {
            if (_unitState == unitState) { return; }
            setUnitState(unitState);
        }

        protected void initEntityId() {
            var entity = gameObject.GetComponent<IEntityId>();
            ID = entity.Id;
            entity.DestroyComponent();
        }

        protected void tryLoadActor() {
            if (_actorGo != null) {
                Actor = _actorGo.GetComponent<IActor>();
            }
        }

        protected void tryInitActor() {
            _actorGo = null;
            if (Actor != null) {
                Actor.Init(ID);
            }
        }

        protected void setUnitState(UnitState unitState) {
            _unitState = unitState;

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