using System;
using System.Collections.Generic;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.Primitives;

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
        public WPos Position;
        public int Direction;//Facing 2 * PI = 256 ;
        public E_AttackType AttackType;
        public int MoveSpeedModifier;

        private AgentOrder() { Type = E_OrderType.E_NONE; }


        public AgentOrder(E_OrderType type) { Type = type; }
    }
}
