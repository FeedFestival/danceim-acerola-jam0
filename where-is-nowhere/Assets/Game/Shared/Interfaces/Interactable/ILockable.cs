using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface ILockable {
        bool IsLocked { get; }

        void Lock(bool locked = true);
    }
}