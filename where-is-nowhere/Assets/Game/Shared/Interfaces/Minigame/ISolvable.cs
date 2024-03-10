using System;

namespace Game.Shared.Interfaces {
    public interface ISolvable {
        Action OnSolved { get; set; }
    }
}