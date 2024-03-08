using UnityEngine;

namespace Game.Unit {
    [CreateAssetMenu(fileName = "UnitsSettingsSO", menuName = "ScriptableObjects/UnitsSettingsSO", order = 1)]
    public class UnitsSettingsSO : ScriptableObject {
        public GameObject MovementTrigger;  
    }
}