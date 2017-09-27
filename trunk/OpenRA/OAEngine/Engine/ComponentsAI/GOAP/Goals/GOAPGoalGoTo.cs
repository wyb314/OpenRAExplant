using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentsAI.GOAP.Goals
{
    class GOAPGoalGoTo : GOAPGoal
    {
        public GOAPGoalGoTo(Agent owner) : base(E_GOAPGoals.E_GOTO, owner) { }

        public override void InitGoal()
        {
            Log.Write("wyb", "Enter GOAPGoalGoTo goal!");
        }

        public override float GetMaxRelevancy()
        {
            return Owner.BlackBoard.GOAP_GoToRelevancy;
        }

        public override void CalculateGoalRelevancy()
        {
            //AgentOrder order = Ai.BlackBoard.OrderGet();
            //if(order != null && order.Type == AgentOrder.E_OrderType.E_GOTO)

            if (Owner.BlackBoard.MotionType != E_MotionType.None)
            {
                GoalRelevancy = 0;
            }
            else
            {
                WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_AT_TARGET_POS);
                if (prop != null && prop.GetBool() == false)
                    GoalRelevancy = Owner.BlackBoard.GOAP_GoToRelevancy;
                else
                    GoalRelevancy = 0;
            }
        }

        public override void SetDisableTime() { NextEvaluationTime = Owner.BlackBoard.GOAP_GoToDelay + Game.LocalTick * Game.Timestep; }

        public override void SetWSSatisfactionForPlanning(WorldState worldState)
        {
            worldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
        }

        public override bool IsWSSatisfiedForPlanning(WorldState worldState)
        {
            WorldStateProp prop = worldState.GetWSProperty(E_PropKey.E_AT_TARGET_POS);

            if (prop != null && prop.GetBool() == true)
                return true;

            return false;
        }

        public override bool IsSatisfied()
        {
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_AT_TARGET_POS);

            if (prop != null && prop.GetBool() == true)
                return true;

            return false;
        }
    }
}
