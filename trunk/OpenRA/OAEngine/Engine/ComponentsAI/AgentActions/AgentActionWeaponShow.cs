using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.Factories;

namespace Engine.ComponentsAI.AgentActions
{
    public class AgentActionWeaponShow : AgentAction
    {
        public bool Show = true;

        public AgentActionWeaponShow() : base(AgentActionFactory.E_Type.E_WEAPON_SHOW) { }
    }
}
