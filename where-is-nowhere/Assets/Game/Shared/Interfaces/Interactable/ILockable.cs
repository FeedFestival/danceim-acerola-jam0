using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ILockable {

        void Lock(bool locked = true);
    }
}