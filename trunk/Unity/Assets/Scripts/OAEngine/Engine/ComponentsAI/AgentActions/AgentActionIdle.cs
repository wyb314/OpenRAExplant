using System;
using System.Collections.Generic;
using Engine.ComponentsAI.Factories;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionIdle : AgentAction
    {
        public AgentActionIdle() : base(AgentActionFactory.E_Type.E_IDLE) { }
    }
}
