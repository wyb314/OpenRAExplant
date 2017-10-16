using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.Primitives;
using TrueSync;
using Engine.ComponentsAI.AStarMachine;

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
        public TSVector2 Position;
        public TSVector2 Direction;
        public FP Facing;
        public Agent Target;
        public E_AttackType AttackType;
        public FP MoveSpeedModifier;
        public AnimAttackData AnimAttackData;

        private AgentOrder() { Type = E_OrderType.E_NONE; }


        public AgentOrder(E_OrderType type) { Type = type; }
    }
}
