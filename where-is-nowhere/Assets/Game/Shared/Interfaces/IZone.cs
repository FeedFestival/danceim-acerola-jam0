using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Shared.Interfaces {
    public interface IZone {
        int ID { get; }
        public Action Entered { get; set; }
        public Action Exited { get; set; }

        void Init();
        void EmitEntered();
        void EmitExited();
    }
}