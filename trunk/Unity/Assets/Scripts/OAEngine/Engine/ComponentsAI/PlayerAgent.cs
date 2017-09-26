using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.GOAP.Core;
using Engine.ComponentsAI.WorkingMemory;

namespace Engine.ComponentsAI
{
    public class PlayerAgent : Agent
    {

        public PlayerAgent()
        {
            this.WorldState = new WorldState();
            this.m_GoalManager = new GOAPManager(this);
            this.Memory = new Memory();


            this.ResetAgent();
        }


        private void ResetAgent()
        {
        }


    }
}
