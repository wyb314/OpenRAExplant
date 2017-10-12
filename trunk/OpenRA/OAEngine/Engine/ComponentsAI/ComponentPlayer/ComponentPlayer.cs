using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;
using TrueSync;

namespace Engine.ComponentsAI.ComponentPlayer
{
    public enum E_AttackType
    {
        None = -1,
        X = 0,
        O = 1,
        BossBash = 2,
        Fatality = 3,
        Counter = 4,
        Berserk = 5,
        Max = 6,
    }
    public class ComponentPlayer: IActionHandler
    {
        

        private Agent Owner;

        private AgentActionAttack CurrentAttackAction;

        private Agent LastAttacketTarget;

        public int force;

        private Queue<AgentOrder> BufferedOrders = new Queue<AgentOrder>();

        public ComponentPlayer(Agent Owner)
        {
            this.Owner = Owner;
        }


        public void Init()
        {
            this.Owner.AddGOAPAction(E_GOAPAction.gotoPos);
            this.Owner.AddGOAPAction(E_GOAPAction.move);
            this.Owner.AddGOAPAction(E_GOAPAction.gotoMeleeRange);
            this.Owner.AddGOAPAction(E_GOAPAction.weaponShow);
            this.Owner.AddGOAPAction(E_GOAPAction.weaponHide);
            this.Owner.AddGOAPAction(E_GOAPAction.orderAttack);
            //Agent.AddGOAPAction(E_GOAPAction.orderAttackJump);
            this.Owner.AddGOAPAction(E_GOAPAction.orderDodge);
            this.Owner.AddGOAPAction(E_GOAPAction.rollToTarget);
            this.Owner.AddGOAPAction(E_GOAPAction.useLever);
            this.Owner.AddGOAPAction(E_GOAPAction.playAnim);
            this.Owner.AddGOAPAction(E_GOAPAction.teleport);
            this.Owner.AddGOAPAction(E_GOAPAction.injury);
            this.Owner.AddGOAPAction(E_GOAPAction.death);

            this.Owner.AddGOAPGoal(E_GOAPGoals.E_GOTO);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ORDER_ATTACK);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ORDER_DODGE);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ORDER_USE);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_ALERT);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_CALM);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_USE_WORLD_OBJECT);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_PLAY_ANIM);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_TELEPORT);
            this.Owner.AddGOAPGoal(E_GOAPGoals.E_REACT_TO_DAMAGE);

            this.Owner.InitializeGOAP();

            Owner.BlackBoard.ActionHandlerAdd(this);
            Owner.BlackBoard.ActionHandlerAdd(this);
        }



        public bool CouldBufferNewOrder()
        {
            return BufferedOrders.Count <= 0 && CurrentAttackAction != null;
        }

        public bool CouldAddnewOrder()
        {
            AgentOrder.E_OrderType order = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder();

            if (order == AgentOrder.E_OrderType.E_DODGE || order == AgentOrder.E_OrderType.E_ATTACK || order == AgentOrder.E_OrderType.E_USE)
                return false;

            AgentAction action;

            //for (int i = 0; i < Owner.BlackBoard.ActionCount(); i++)
            //{
            //    action = Owner.BlackBoard.ActionGet(i);
            //    if (action is AgentActionAttack && (action as AgentActionAttack).AttackPhaseDone == false)
            //        return false;
            //    else if (action is AgentActionRoll)
            //        return false;
            //    else if (action is AgentActionUseLever)
            //        return false;
            //    else if (action is AgentActionGoTo && (action as AgentActionGoTo).Motion == E_MotionType.Sprint)
            //        return false;
            //}
            return true;
        }

        public void CreateOrderAttack(E_AttackType type)
        {
            if (CouldBufferNewOrder() == false && CouldAddnewOrder() == false)
            {
                return;
            }
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_ATTACK);

            //if (Controls.Joystick.Direction != Vector3.zero)
            //    order.Direction = Controls.Joystick.Direction;
            //else
            //    order.Direction = Transform.forward;


            //order.AnimAttackData = ProcessCombo(type);

            //order.Target = GetBestTarget(false);

            if (CouldAddnewOrder())
            {
                Owner.BlackBoard.OrderAdd(order);
            }
            else
            {
                BufferedOrders.Enqueue(order);
            }
        }


        public void CreateOrderDodge()
        {
            if (Owner.BlackBoard.IsOrderAddPossible(AgentOrder.E_OrderType.E_DODGE) == false)
                return;

            TSVector2 rollDir;

            if (this.Owner.CurJoystickDir != TSVector2.zero)
            {
                rollDir = this.Owner.CurJoystickDir;
            }
            else
            {
                rollDir = TSVector2.up;
            }
               
            rollDir.Normalize();
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_DODGE);
            order.Direction = rollDir;

            Owner.BlackBoard.OrderAdd(order);

            ClearBufferedOrder();
        }

        void ClearBufferedOrder()
        {
            while (BufferedOrders.Count > 0)
                AgentOrderFactory.Return(BufferedOrders.Dequeue());
        }


        public void CreateOrderGoTo(FP MoveSpeedModifier)
        {
            this.force = force;

            if (CouldAddnewOrder() == false)
                return;
            
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_GOTO);
            order.Direction = this.Owner.CurJoystickDir;
            order.MoveSpeedModifier = MoveSpeedModifier;

            Owner.BlackBoard.OrderAdd(order);
        }

        public void CreateOrderStop()
        {
            this.force = 0;
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_STOPMOVE);
            Owner.BlackBoard.OrderAdd(order);
        }

        

        public void Update()
        {
            if (Owner.BlackBoard.Stop)
            {
                LastAttacketTarget = null;
                //ComboProgress.Clear();
                ClearBufferedOrder();
                CreateOrderStop();
                return;
            }

            if (BufferedOrders.Count > 0)
            {
                if (CouldAddnewOrder())
                    Owner.BlackBoard.OrderAdd(BufferedOrders.Dequeue());
            }
        }

        public void HandleAction(AgentAction a)
        {
            if (a is AgentActionAttack)
            {
                CurrentAttackAction = a as AgentActionAttack;
                Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, true);
            }
            //else if (a is AgentActionInjury)
            //{
            //    Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);
            //    ComboProgress.Clear();
            //    ClearBufferedOrder();
            //    GuiManager.Instance.ShowComboProgress(ComboProgress);
            //    Game.Instance.Hits = 0;
            //    Game.Instance.NumberOfInjuries++;

            //}
            //else if (a is AgentActionDeath)
            //{
            //    Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);
            //    ComboProgress.Clear();
            //    ClearBufferedOrder();
            //    GuiManager.Instance.ShowComboProgress(ComboProgress);
            //    Game.Instance.Hits = 0;
            //    Game.Instance.NumberOfDeath++;
            //    //Game.Instance.Score -= 50;
            //    Mission.Instance.EndOfMission(false);
            //    // of	unlockAchievement
            //    if (Game.Instance.NumberOfDeath >= 100)
            //    {
            //        Achievements.UnlockAchievement(2);
            //    }
            //    else if (Game.Instance.NumberOfDeath >= 50)
            //    {
            //        Achievements.UnlockAchievement(1);
            //    }
            //}
        }
    }
}
