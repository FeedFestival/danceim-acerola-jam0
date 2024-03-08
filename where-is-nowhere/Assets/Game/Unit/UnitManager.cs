using Game.Shared.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class UnitManager : MonoBehaviour, IUnitManager {

        [SerializeField]
        private UnitsSettingsSO _unitsSettings;

        public Dictionary<int, IUnit> Units { get; private set; }

        public void Init() {
            Units = new Dictionary<int, IUnit>();

            var movementIndicatorT = createMovementIndicatorsParen();

            foreach (Transform t in transform) {
                var unit = t.GetComponent<INPCUnit>();

                if (unit == null) { continue; }

                var movementTargetTrigger = createMovementTarget(_unitsSettings.MovementTrigger, movementIndicatorT);
                unit?.Init(movementTargetTrigger);
                Units.Add(unit.ID, unit);
            }
        }

        private Transform createMovementIndicatorsParen() {
            var go = new GameObject("MovementIndicators");
            go.transform.SetParent(transform);
            return go.transform;
        }

        private ITrigger createMovementTarget(GameObject movementTargetSignal, Transform movementIndicator = null) {
            var go = Instantiate(movementTargetSignal);

            if (movementIndicator != null) {
                go.transform.SetParent(movementIndicator.transform);
            }

            return go.GetComponent<ITrigger>();
        }
    }
}
