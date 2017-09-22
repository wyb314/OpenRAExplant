using System;
using System.Collections.Generic;

namespace Engine.Inputs
{
    public struct ControllerEvt
    {
        public E_OpType evtType;

        public int val;

        public ControllerEvt(E_OpType evtType , int val)
        {
            this.evtType = evtType;
            this.val = val;
        }

    }
}
