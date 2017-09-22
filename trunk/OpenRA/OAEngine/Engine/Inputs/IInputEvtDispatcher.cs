using System;
using System.Collections.Generic;
using Engine.Inputs;

namespace Engine.Inputs
{
    public interface IInputEvtDispatcher
    {
        void HandInput(ControllerEvt evt);
    }
}
