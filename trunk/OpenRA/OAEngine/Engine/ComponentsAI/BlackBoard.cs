using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;

namespace OAEngine.Engine.ComponentsAI
{
    public interface IActionHandler
    {
        void HandleAction(AgentAction a);
    }

    public class BlackBoard
    {
        private List<AgentAction> m_ActiveActions = new List<AgentAction>();
        private List<IActionHandler> m_ActionHandlers = new List<IActionHandler>();

        public Agent Owner;



    }
}
