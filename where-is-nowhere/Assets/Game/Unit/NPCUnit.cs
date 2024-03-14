using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {
    public class NPCUnit : Unit, INPCUnit, IDefaultInteraction {

        public override IUnitControl UnitControl => _npcControl;
        private INPCControl _npcControl;

        public virtual void Init(ITrigger movementTargetTrigger) {
            base.initEntityId();
            base.tryLoadActor();
            base.tryInitActor();

            movementTargetTrigger.Init(ID);

            _npcControl = GetComponent<INPCControl>();
            _npcControl.Init(ID, Actor, movementTargetTrigger);

            SetUnitState(UnitState.FreePlaying);
        }

        public override void SetUnitState(UnitState unitState) {
            if (_unitState == unitState) { return; }
            setUnitState(unitState);
        }

        public void DoDefaultInteraction(IPlayer player) {
            (player.Unit as IPlayerUnit).Inventory.AddToInventory(InventoryItem.RightHand);

            SetUnitState(UnitState.Hidden);
        }

        public void DoDefaultInteraction(IUnit unit) {

        }
    }
}
