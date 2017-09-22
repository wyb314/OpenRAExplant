using System;
using System.Collections.Generic;
using Engine.Interfaces;
using ORDER = Engine.Network.Defaults.Order;

namespace Engine.Inputs
{
    public class NormalInputEvtDispatcher : IInputEvtDispatcher
    {
        public void HandInput(ControllerEvt evt)
        {
            switch (evt.evtType)
            {
                    case E_OpType.Joystick:
                    Game.OrderManager.IssueOrder(ORDER.ButtonDown((byte)evt.evtType, (byte)(evt.val & 0xff)));
                    break;
                default:
                    Game.OrderManager.IssueOrder(ORDER.ButtonDown((byte)evt.evtType, 0));
                    break;
            }
            
        }
    }
}
