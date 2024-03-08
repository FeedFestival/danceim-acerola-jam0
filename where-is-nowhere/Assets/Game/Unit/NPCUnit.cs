using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class NPCUnit : Unit, INPCUnit {

        private INPCControl _npcControl;

        public override void Init() {
            base.initEntityId();
            base.tryInitActor();

            _npcControl = GetComponent<INPCControl>();
            _npcControl.Init(Actor);

            SetUnitState(UnitState.FreePlaying);
        }

        public override void SetUnitState(UnitState unitState) {

            if (_unitState == unitState) { return; }
            setUnitState(unitState);

            _npcControl.SetUnitState(_unitState);
        }
    }
}
