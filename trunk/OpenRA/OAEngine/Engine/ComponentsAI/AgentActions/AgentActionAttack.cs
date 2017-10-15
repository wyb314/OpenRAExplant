using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.ComponentPlayer;
using Engine.ComponentsAI.Factories;
using Engine.Primitives;
using TrueSync;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionAttack : AgentAction
    {
        public Agent Target;
        public AnimAttackData Data;
        public E_AttackType AttackType;
        public TSVector2 AttackDir;

        public bool Hit;
        public bool AttackPhaseDone;

        public AgentActionAttack() : base(AgentActionFactory.E_Type.E_ATTACK)
        {
        }

        public override void Reset()
        {
            base.Reset();
            Target = null;
            Hit = false;
            AttackPhaseDone = false;
            //Data = null;
            AttackType = E_AttackType.None;
        }
        public override string ToString() { return "HumanActionAttack " + (Target != null ? Target.name : "no target") + " " + AttackType.ToString() + " " + Status.ToString(); }
    }
}
