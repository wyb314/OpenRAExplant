using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using TrueSync;

namespace Engine.ComponentsAI.GOAP.Goals
{
    class GOAPGoalOrderAttack : GOAPGoal
    {
        public GOAPGoalOrderAttack(Agent owner) : base(E_GOAPGoals.E_ORDER_ATTACK, owner) { }

        public override void InitGoal()
        {

        }

        public override FP GetMaxRelevancy()
        {
            return Owner.BlackBoard.GOAP_KillTargetRelevancy;
        }
        public override void CalculateGoalRelevancy()
        {
            //   E_EventTypes eventType = Owner.WorldState.GetWSProperty(E_PropKey.E_EVENT).GetEvent();
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER);

            if (prop != null && prop.GetOrder() == AgentOrder.E_OrderType.E_ATTACK)
                GoalRelevancy = Owner.BlackBoard.GOAP_KillTargetRelevancy;
            else
                GoalRelevancy = 0;
        }

        public override void SetDisableTime() { NextEvaluationTime = Owner.BlackBoard.GOAP_KillTargetDelay + Game.WorldTime; }

        public override void SetWSSatisfactionForPlanning(WorldState worldState)
        {
            worldState.SetWSProperty(E_PropKey.E_ATTACK_TARGET, true);
        }

        public override bool IsWSSatisfiedForPlanning(WorldState worldState)
        {
            WorldStateProp prop = worldState.GetWSProperty(E_PropKey.E_ATTACK_TARGET);

            if (prop != null && prop.GetBool() == true)
                return true;

            return false;
        }

        public override bool ReplanRequired()
        {
            if (IsPlanFinished() && Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER).GetOrder() == AgentOrder.E_OrderType.E_ATTACK)
                return true;

            return false;
        }

        public override bool IsSatisfied()
        {
            return IsPlanFinished();
        }

        public override void Deactivate()
        {
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_ORDER);
            if (prop.GetOrder() == AgentOrder.E_OrderType.E_ATTACK)
                Owner.WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

            base.Deactivate();
        }
    }
}
