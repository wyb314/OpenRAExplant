using System;
using System.Collections.Generic;
using Engine.ComponentsAI.Factories;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionPlayIdleAnim : AgentAction
    {
        public AgentActionPlayIdleAnim() : base(AgentActionFactory.E_Type.E_PLAY_IDLE_ANIM) { }
    }
}
