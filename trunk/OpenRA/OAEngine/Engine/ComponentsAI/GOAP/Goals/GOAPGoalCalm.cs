using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using TrueSync;

namespace Engine.ComponentsAI.GOAP.Goals
{
    class GOAPGoalCalm : GOAPGoal
    {
        public GOAPGoalCalm(Agent owner) : base(E_GOAPGoals.E_CALM, owner) { }

        public override void InitGoal()
        {

        }

        public override FP GetMaxRelevancy()
        {
            return Owner.BlackBoard.GOAP_CalmRelevancy;
        }

        public override void CalculateGoalRelevancy()
        {
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_WEAPON_IN_HANDS);
            WorldStateProp prop2 = Owner.WorldState.GetWSProperty(E_PropKey.E_ALERTED);
            if (prop != null && prop2 != null && prop.GetBool() == true && prop2.GetBool() == false && Owner.BlackBoard.IdleTimer > 1.5f && this.Owner.Random.Next(0, 100) < 5)
                GoalRelevancy = Owner.BlackBoard.GOAP_CalmRelevancy;
            else
                GoalRelevancy = 0;

            SetDisableTime();
        }

        public override void SetDisableTime() { NextEvaluationTime = Owner.BlackBoard.GOAP_CalmDelay + Game.WorldTime; }

        public override void SetWSSatisfactionForPlanning(WorldState worldState)
        {
            worldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
        }

        public override bool IsWSSatisfiedForPlanning(WorldState worldState)
        {
            WorldStateProp prop = worldState.GetWSProperty(E_PropKey.E_WEAPON_IN_HANDS);

            if (prop.GetBool() == false)
                return true;

            return false;
        }

        public override bool IsSatisfied()
        {
            WorldStateProp prop = Owner.WorldState.GetWSProperty(E_PropKey.E_WEAPON_IN_HANDS);

            if (prop.GetBool() == false)
                return true;

            return false;
        }


        public override bool ReplanRequired()
        {
            return false;
        }
    }
}
