using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Support;
using TrueSync;

namespace Engine.ComponentsAI.GOAP.Goals
{
    class GOAPGoalIdle : GOAPGoal
    {
        public GOAPGoalIdle(Agent owner) : base(E_GOAPGoals.E_IDLE_ANIM, owner) { }

        public override void InitGoal()
        {
            Log.Write("wyb", "Enter GOAPGoalIdle goal!");
        }

        public override FP GetMaxRelevancy()
        {
            return Owner.BlackBoard.GOAP_IdleActionRelevancy;
        }

        public override void CalculateGoalRelevancy()
        {
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_IDLING);
            WorldStateProp prop2 = Owner.WorldState.GetWSProperty(E_PropKey.E_WEAPON_IN_HANDS);

            if (prop != null && prop.GetBool() == true && prop2.GetBool() == true && Owner.BlackBoard.IdleTimer > 5)
                GoalRelevancy = Owner.BlackBoard.GOAP_IdleActionRelevancy;
            else
                GoalRelevancy = 0;
        }

        public override void SetDisableTime() { NextEvaluationTime = Owner.BlackBoard.GOAP_IdleActionDelay + Game.Time; }

        public override void SetWSSatisfactionForPlanning(WorldState worldState)
        {
            worldState.SetWSProperty(E_PropKey.E_IDLING, false);
        }

        public override bool IsWSSatisfiedForPlanning(WorldState worldState)
        {
            WorldStateProp prop = worldState.GetWSProperty(E_PropKey.E_IDLING);

            if (prop.GetBool() == false)
                return true;

            return false;
        }

        public override bool IsSatisfied()
        {
            return IsPlanFinished();
        }
    }
}
