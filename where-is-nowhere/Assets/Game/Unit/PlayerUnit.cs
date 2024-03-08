using Game.Shared.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class PlayerUnit : Unit, IPlayerUnit {
        [Header("Player Unit")]
        [SerializeField]
        private Transform _spineToOrientate;


        public Transform SpineToOrientate { get => _spineToOrientate; }
    }
}
