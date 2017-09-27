using System;
using System.Collections.Generic;
using Engine;
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
        
        public int IdleTimer = 0;

        private List<AgentAction> m_ActiveActions = new List<AgentAction>();
        private List<IActionHandler> m_ActionHandlers = new List<IActionHandler>();

       
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

        public int SpeedSmooth = 200;
        public int RotationSmooth = 2;
        public float RotationSmoothInMove = 8.0f;
        public float RollDistance = 4.0f;
        public int MoveSpeedModifier = 100;


        public int Speed = 0;
        public int MoveDir;


        public WPos DesiredPosition;
        public int DesiredDirection;

        public Agent DesiredTarget;
        public E_AttackType DesiredAttackType;

        #region Goal Setting
        public float GOAP_AlertRelevancy = 0.7f;
        public float GOAP_CalmRelevancy = 0.2f;
        public float GOAP_BlockRelevancy = 0.7f;
        public float GOAP_DodgeRelevancy = 0.9f;
        public float GOAP_GoToRelevancy = 0.5f;
        public float GOAP_CombatMoveBackwardRelevancy = 0.7f;
        public float GOAP_CombatMoveForwardRelevancy = 0.75f;
        public float GOAP_CombatMoveLeftRelevancy = 0.6f;
        public float GOAP_CombatMoveRightRelevancy = 0.6f;
        public float GOAP_LookAtTargetRelevancy = 0.7f;
        public float GOAP_KillTargetRelevancy = 0.8f;
        public float GOAP_PlayAnimRelevancy = 0.95f;
        public float GOAP_UseWorlObjectRelevancy = 0.9f;
        public float GOAP_ReactToDamageRelevancy = 1.0f;
        public float GOAP_IdleActionRelevancy = 0.4f;
        public float GOAP_TeleportRelevancy = 0.9f;

        public float GOAP_AlertDelay = 0;
        public float GOAP_CalmDelay = 0;
        public float GOAP_BlockDelay = 2.7f;
        public float GOAP_DodgeDelay = 0;
        public int GOAP_GoToDelay = 0;
        public float GOAP_CombatMoveDelay = 0.5f;
        public float GOAP_CombatMoveLeftDelay = 2.6f;
        public float GOAP_CombatMoveRightDelay = 2.6f;
        public float GOAP_LookAtTargetDelay = 0.4f;
        public float GOAP_KillTargetDelay = 0f;
        public float GOAP_PlayAnimDelay = 0.0f;
        public float GOAP_UseWorlObjectDelay = 0f;
        public float GOAP_CombatMoveBackwardDelay = 3.5f;
        public float GOAP_CombatMoveForwardDelay = 3.5f;
        public int GOAP_IdleActionDelay = 10 * 1000;
        public float GOAP_TeleportDelay = 4;

        #endregion

        public bool _Stop = false;
        /// <summary>
        /// 某些情况下会禁止获取用户输入
        /// </summary>
        public bool Stop { get { return _Stop; } set { _Stop = value; Game.AllUserInput(!_Stop); } }

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
                Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, order.Type);
                switch (order.Type)
                {
                    case AgentOrder.E_OrderType.E_STOPMOVE:
                        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
                        DesiredPosition = Owner.Position;
                        break;
                    case AgentOrder.E_OrderType.E_GOTO:
                        Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, false);
                        DesiredPosition = order.Position;
                        DesiredDirection = order.Direction;
                        MoveSpeedModifier = order.MoveSpeedModifier;
                        break;
                    case AgentOrder.E_OrderType.E_DODGE:
                        //DesiredDirection = order.Direction;
                        //Debug.Log(Time.timeSinceLevelLoad + " order arrived " + order.Type);
                        break;
                  
                }

                //  Debug.Log(Time.timeSinceLevelLoad + " new order arrived " + order.Type);
            }
            else if (order.Type == AgentOrder.E_OrderType.E_ATTACK)
            {
                // Debug.Log(Time.timeSinceLevelLoad +  " " +order.Type + " is nto allowed because " + currentOrder);
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
            IdleTimer += Game.Timestep;

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

        public void ActionAdd(AgentAction action)
        {
            IdleTimer = 0;

            m_ActiveActions.Add(action);

            for (int i = 0; i < m_ActionHandlers.Count; i++)
                m_ActionHandlers[i].HandleAction(action);
        }

        public void Reset()
        {
            m_ActiveActions.Clear();

            Stop = false;
            MotionType = E_MotionType.None;
            WeaponState = E_WeaponState.NotInHands;
            WeaponToSelect = E_WeaponType.None;

            Speed = 0;

            //Health = RealMaxHealth;

            //Rage = RageMin;
            //Dodge = DodgeMin;
            //Fear = FearMin;
            IdleTimer = 0;

            MoveDir = 0;

            DesiredPosition = WPos.Zero;
            DesiredDirection = 0;

            //InteractionObject = null;
            //Interaction = E_InteractionType.None;

            //DesiredAnimation = "";

            DesiredTarget = null;
            DesiredAttackType = E_AttackType.None;

            DontUpdate = false;
        }


    }
}
