using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.Factories;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionMove : AgentAction
    {
        public enum E_MoveType
        {
            E_MT_FORWARD,
            E_MT_BACKWARD,
            E_STRAFE_LEFT,
            E_STRAFE_RIGHT,
        }

        public E_MoveType MoveType = E_MoveType.E_MT_FORWARD;

        public AgentActionMove() : base(AgentActionFactory.E_Type.E_MOVE) { }
    }
}
