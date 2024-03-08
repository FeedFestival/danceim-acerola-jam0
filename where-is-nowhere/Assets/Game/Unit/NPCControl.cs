using Game.Shared.Constants;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Unit {

    public class NPCControl : MonoBehaviour, INPCControl {

        public IMotor Motor { get => _npcMotor; }

        private NPCMotor _npcMotor;

        public void Init(int id, IActor actor, ITrigger movementTargetTrigger) {
            _npcMotor = GetComponent<NPCMotor>();
            _npcMotor.Init(id, actor.Animator, movementTargetTrigger);
        }
        public void Sprint(bool value) {

        }
        public void SetUnitState(UnitState unitState) {
            _npcMotor.SetUnitState(unitState);
        }

        public void MoveTo(Vector3 pos) {
            _npcMotor.MovementTargetChanged(pos);
        }

        //-----------------------------------------------------------------------------------

    }
}
