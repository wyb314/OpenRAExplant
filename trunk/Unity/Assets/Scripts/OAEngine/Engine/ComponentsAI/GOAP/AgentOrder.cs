using System;
using System.Collections.Generic;

namespace Engine.ComponentsAI.GOAP
{
    public class AgentOrder
    {
        public enum E_OrderType
        {
            E_NONE,
            E_GOTO,
            E_ATTACK,
            E_DODGE,
            E_USE,
            E_STOPMOVE,
        }

        public E_OrderType Type;

        private AgentOrder() { Type = E_OrderType.E_NONE; }


        public AgentOrder(E_OrderType type) { Type = type; }
    }
}
