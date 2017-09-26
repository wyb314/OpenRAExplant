using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;

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

        public AgentAction ActionGet(int index)
        {
            return m_ActiveActions[index];
        }

        public int ActionCount()
        {
            return m_ActiveActions.Count;
        }

        public void OrderAdd(AgentOrder order)
        {
            if (IsOrderAddPossible(order.Type))
            {
                
            }

            AgentOrderFactory.Return(order);
        }

        public bool IsOrderAddPossible(AgentOrder.E_OrderType orderType)
        {
            AgentOrder.E_OrderType currentOrder = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder();

            if (orderType == AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_USE)
                return true;
            else if (currentOrder != AgentOrder.E_OrderType.E_ATTACK && currentOrder != AgentOrder.E_OrderType.E_DODGE && currentOrder != AgentOrder.E_OrderType.E_USE)
                return true;
            else
                return false;
        }

    }
}
