using Game.Shared.Constants;
using Game.Shared.Interfaces;

namespace Game.Unit {
    public class NPCUnit : Unit, INPCUnit {

        public IUnitControl UnitControl => _npcControl;
        private INPCControl _npcControl;

        public void Init(ITrigger movementTargetTrigger) {
            base.initEntityId();
            base.tryInitActor();

            movementTargetTrigger.Init(ID);

            _npcControl = GetComponent<INPCControl>();
            _npcControl.Init(ID, Actor, movementTargetTrigger);

            SetUnitState(UnitState.FreePlaying);
        }

        public override void SetUnitState(UnitState unitState) {

            if (_unitState == unitState) { return; }
            setUnitState(unitState);

            _npcControl.SetUnitState(_unitState);
        }
    }
}
