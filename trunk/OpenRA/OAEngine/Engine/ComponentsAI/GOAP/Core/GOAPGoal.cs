using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using TrueSync;

namespace Engine.ComponentsAI.GOAP.Core
{
    public abstract class GOAPGoal
    {
        public Agent Owner;// { get { return Ai; } private set { Ai = value; } }
        public FP GoalRelevancy;//{ get { return GoalRelevancy; } protected set { GoalRelevancy = value; } }
        public E_GOAPGoals GoalType;// { get { return GoalType; } private set { GoalType = value; } }

        public bool Active = false;// { get { return Active; } private set { Active = value; } }
        public bool Critical = false; // Its top most goal, if it set to higher value it could terminate other goal !!!

        private GOAPPlan Plan;
        protected FP NextEvaluationTime;// { get { return NextEvaluationTime; } protected set { NextEvaluationTime = value; } }

        static int id;
        public int UID;

        public abstract void SetWSSatisfactionForPlanning(WorldState worldState);
        public abstract bool IsWSSatisfiedForPlanning(WorldState worldState);



        public abstract FP GetMaxRelevancy();
        public abstract void CalculateGoalRelevancy(); // how important is this goal !!!
        public void ClearGoalRelevancy() { GoalRelevancy = 0; }

        public virtual void SetDisableTime() { NextEvaluationTime = Game.WorldTime + this.Owner.Random.Next(0.1f,0.2f); }

        public virtual bool ReplanRequired() { return false; }// if goal need to be replanned !!!!
        public abstract bool IsSatisfied();
        public virtual bool IsDisabled() { return NextEvaluationTime > Game.WorldTime; }

        protected GOAPGoal(E_GOAPGoals type, Agent ai)
        {
            GoalType = type;
            Owner = ai;
        }

        public abstract void InitGoal();

        /**
        * Updates the goal. This involves getting the plan, checking if the current step(i.e. action is complete), if so advance the plan
        */
        public bool UpdateGoal()
        {
            //Check if plan exists, if not quit
            if (Plan == null)
                return false;

            Plan.Update();

            //Check if the plan step is complete, if so advance if not do nothing :)
            if (Plan.IsPlanStepComplete())
                return Plan.AdvancePlan();

            return true;
        }

        public virtual bool Activate(GOAPPlan plan)
        {
            UID = ++id;

            Active = true;
            Plan = plan;

            return Plan.Activate(Owner, this);
        }

        public virtual void ReplanReset()
        {
            Active = false;
            if (Plan != null)
                Plan.Deactivate();

            Plan = null;

            //if (Owner.debugGOAP) Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " - replan Reset");
        }

        public virtual void Reset()
        {
            Active = false;
            if (Plan != null)
                Plan.Deactivate();

            Plan = null;
            ClearGoalRelevancy();

            //if (Owner.debugGOAP) Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " - Reset");
        }
        /* 
         * do some cleaning shit here
         */
        public virtual void Deactivate()
        {
            Active = false;
            if (Plan != null)
                Plan.Deactivate();

            Plan = null;
            ClearGoalRelevancy();
            SetDisableTime();

            //if (Owner.debugGOAP) Debug.Log(Time.timeSinceLevelLoad + " " + this.ToString() + " - Deactivated");
        }

        /**
        * Checks whether the plan is interruptible or not
        * @return true if the plan is interruptible, false otherwise
        */

        public bool IsPlanInterruptible()
        {
            return Plan == null ? true : Plan.IsPlanStepInterruptible();
        }

        /**
        * Checks whether the plan is valid
        * @return true if the plan is valid, false otherwise
        */
        public virtual bool IsPlanValid()
        {
            return Plan == null ? false : Plan.IsPlanValid();
        }


        public bool IsPlanFinished()
        {
            return Plan == null ? true : Plan.IsDone();
        }



        //If a plan fails to be built, clear the relevancy and try again
        public virtual void HandlePlanBuildFailure()
        {
            ClearGoalRelevancy();
        }
        
        public override string ToString()
        {
            return base.ToString() + " Releavancy: " + GoalRelevancy + (Active ? "Active " : " Deactive ") + (IsDisabled() ? " Disabled " : " Enabled ");
        }
    }
}
