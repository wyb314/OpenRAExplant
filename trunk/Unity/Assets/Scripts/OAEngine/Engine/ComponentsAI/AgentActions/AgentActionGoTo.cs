using System;
using System.Collections.Generic;
using Engine.ComponentsAI.Factories;
using Engine.Primitives;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionGoTo : AgentAction
    {
        public enum E_MoveType
        {
            E_MT_FORWARD,
            E_MT_BACKWARD,
            E_STRAFE_LEFT,
            E_STRAFE_RIGHT,
        }

        public WPos FinalPosition;
        public E_MoveType MoveType;
        public E_MotionType Motion;

        public AgentActionGoTo() : base(AgentActionFactory.E_Type.E_GOTO)
        {
        }
    }
}
