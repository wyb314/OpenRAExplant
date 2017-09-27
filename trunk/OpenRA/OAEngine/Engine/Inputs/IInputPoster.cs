using System;
using System.Collections.Generic;

namespace Engine.Inputs
{
    public interface IInputPoster
    {
        bool AllowAcceptInput { set; get; }

        IInputEvtDispatcher dispatcher { get; }
        void PostInput(E_OpType type, int val = 0);
    }
}
