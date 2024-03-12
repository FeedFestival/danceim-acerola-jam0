using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IFireable {

        Action FireAnimationCompleted { get; set; }

        void FireInDirection(Vector3 origin, Vector3 direction);
    }
}