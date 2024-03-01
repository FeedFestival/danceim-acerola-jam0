using Game.Shared.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class UnitManager : MonoBehaviour, IUnitManager {
        public Dictionary<int, IUnit> Units { get; private set; }

        public void Init() {
            Units = new Dictionary<int, IUnit>();

            foreach (Transform ct in transform) {
                var unit = ct.GetComponent<IUnit>();
                unit?.Init();
                Units.Add(unit.ID, unit);
            }
        }
    }
}
