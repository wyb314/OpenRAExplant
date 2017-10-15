﻿using System;
using System.Collections.Generic;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;

namespace Engine.ComponentsAI.GOAP.Actions
{
    class GOAPActionWeaponHide : GOAPAction
    {
        private AgentActionWeaponShow Action;

        public GOAPActionWeaponHide(Agent owner) : base(E_GOAPAction.weaponHide, owner) { }


        public override void InitAction()
        {
            WorldEffects.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
            Cost = 1;

            Interruptible = false;
        }


        public override void Activate()
        {
            base.Activate();

            Owner.BlackBoard.WeaponState = E_WeaponState.NotInHands;

            Action = AgentActionFactory.Create(AgentActionFactory.E_Type.E_WEAPON_SHOW) as AgentActionWeaponShow;
            Action.Show = false;
            Owner.BlackBoard.ActionAdd(Action);
        }

        public override void Deactivate()
        {
            base.Deactivate();

            Owner.WorldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
        }

        public override bool IsActionComplete()
        {
            if (Action.IsActive() == false)
                return true;

            return false;
        }
    }
}
