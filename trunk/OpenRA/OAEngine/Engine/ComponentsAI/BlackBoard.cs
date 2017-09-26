using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Primitives;

namespace OAEngine.Engine.ComponentsAI
{
    public interface IActionHandler
    {
        void HandleAction(AgentAction a);
    }

    public enum E_MotionType
    {
        None,
        Walk,
        Run,
        Sprint,
        Roll,
        Attack,
        Block,
        BlockingAttack,
        Injury,
        Knockdown,
        Death,
        AnimationDrive,
    }

    public class BlackBoard
    {
        public bool DontUpdate = true;

        private List<AgentAction> m_ActiveActions = new List<AgentAction>();
        private List<IActionHandler> m_ActionHandlers = new List<IActionHandler>();

        public Agent DesiredTarget;

        public E_MotionType MotionType = E_MotionType.None;

        public E_WeaponState WeaponState = E_WeaponState.NotInHands;
        public E_WeaponType WeaponSelected = E_WeaponType.Katana;
        public E_WeaponType WeaponToSelect = E_WeaponType.None;

        public E_LookType LookType;

        public int MaxSprintSpeed = 8 * 1024;
        public int MaxRunSpeed = 4 * 1024;
        public int MaxWalkSpeed = 1536;
        public int MaxCombatMoveSpeed = 1024;
        //public int MaxHealth = 100;
        public int MaxKnockdownTime = 4000;

        public int Speed = 0;
        public WVec MoveDir;

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


        public void Update()
        {
            for (int i = 0; i < m_ActiveActions.Count; i++)
            {
                if (m_ActiveActions[i].IsActive())
                    continue;

                ActionDone(m_ActiveActions[i]);
                m_ActiveActions.RemoveAt(i);

                return;
            }
            
        }

        private void ActionDone(AgentAction action)
        {
            AgentActionFactory.Return(action);
        }

        public void ActionHandlerAdd(IActionHandler handler)
        {
            for (int i = 0; i < m_ActionHandlers.Count; i++)
                if (m_ActionHandlers[i] == handler)
                    return;

            m_ActionHandlers.Add(handler);
        }
        public void Reset()
        {
            m_ActiveActions.Clear();

            //Stop = false;
            //MotionType = E_MotionType.None;
            //WeaponState = E_WeaponState.NotInHands;
            //WeaponToSelect = E_WeaponType.None;

            //Speed = 0;

            //Health = RealMaxHealth;

            //Rage = RageMin;
            //Dodge = DodgeMin;
            //Fear = FearMin;
            //IdleTimer = 0;

            //MoveDir = Vector3.zero;

            //DesiredPosition = Vector3.zero;
            //DesiredDirection = Vector3.zero;

            //InteractionObject = null;
            //Interaction = E_InteractionType.None;

            //DesiredAnimation = "";

            //DesiredTarget = null;
            //DesiredAttackType = E_AttackType.None;

            DontUpdate = false;
        }


    }
}
