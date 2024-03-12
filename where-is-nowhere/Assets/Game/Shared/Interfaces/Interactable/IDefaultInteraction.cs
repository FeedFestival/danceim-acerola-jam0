using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IDefaultInteraction {
        void DoDefaultInteraction(IPlayer player);
        void DoDefaultInteraction(IUnit unit);
    }
}