using Game.Shared.Constants;
using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {

    public class NPCControl : MonoBehaviour, INPCControl {

        private NPCMotor _npcMotor;

        public void Init(IActor actor) {

            _npcMotor = GetComponent<NPCMotor>();
        }
        public void Sprint(bool value) {
            
        }
        public void SetUnitState(UnitState unitState) {
            _npcMotor.SetUnitState(unitState);
        }
    }
}
