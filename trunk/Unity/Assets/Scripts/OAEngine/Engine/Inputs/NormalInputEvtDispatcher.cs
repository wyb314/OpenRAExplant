using System;
using System.Collections.Generic;
using Engine.Interfaces;
using ORDER = Engine.Network.Defaults.Order;

namespace Engine.Inputs
{
    public class NormalInputEvtDispatcher : IInputEvtDispatcher
    {
        private ORDER curJoystickOrder = null;

        private bool joystickEnd = true;

        public void HandInput(ControllerEvt evt)
        {

            switch (evt.evtType)
            {
                case E_OpType.Joystick:
                    if (evt.val == 0 && !joystickEnd)
                    {
                        joystickEnd = true;
                        Game.OrderManager.IssueOrder(ORDER.JoystickMotion((byte)evt.evtType, (ushort)0));
                    }
                    else if (evt.val > 0)
                    {
                        joystickEnd = false;
                        Game.OrderManager.IssueOrder(ORDER.JoystickMotion((byte)evt.evtType, (ushort)(evt.val & 0xffff)));
                    }
                    break;
                default:
                    Game.OrderManager.IssueOrder(ORDER.ButtonDown((byte)evt.evtType));
                    break;
            }
        }
    }
}
