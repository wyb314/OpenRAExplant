using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using TrueSync;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionRoll : AgentAction
    {
        public TSVector2 Direction;
        public Agent ToTarget;

        public AgentActionRoll() : base(AgentActionFactory.E_Type.E_ROLL) { }

        public override void Reset()
        {
            ToTarget = null;
        }
    }
}
