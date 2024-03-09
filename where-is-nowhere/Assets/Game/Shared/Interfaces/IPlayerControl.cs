using System;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IPlayerControl {
        Action FirePerformed { get; set; }
    }
}