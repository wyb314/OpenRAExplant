using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;

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
    public class ComponentPlayer
    {
        

        private Agent Owner;

        private AgentActionAttack CurrentAttackAction;

        public int force;

        private Queue<AgentOrder> BufferedOrders = new Queue<AgentOrder>();
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

            
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_DODGE);
            order.Direction = this.force;

            Owner.BlackBoard.OrderAdd(order);

            ClearBufferedOrder();
        }

        void ClearBufferedOrder()
        {
            while (BufferedOrders.Count > 0)
                AgentOrderFactory.Return(BufferedOrders.Dequeue());
        }


        public void CreateOrderGoTo(int angle , int force)
        {
            this.force = force;

            if (CouldAddnewOrder() == false)
                return;
            
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_GOTO);
            order.Direction = angle;
            order.MoveSpeedModifier = force;

            Owner.BlackBoard.OrderAdd(order);
        }

        public void CreateOrderStop()
        {
            this.force = 0;
            AgentOrder order = AgentOrderFactory.Create(AgentOrder.E_OrderType.E_STOPMOVE);
            Owner.BlackBoard.OrderAdd(order);
        }


        public void StopMove(bool stop)
        {
            //if (stop)
            //    Controls.DisableInput();
            //else
            //    Controls.EnableInput();
        }

        public void Update()
        {

        }

    }
}
