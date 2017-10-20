using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Engine.ComponentsAI.AStarMachine;

namespace Engine.Physics
{
    public abstract class AbstractPhysicComponent : IAgentComponent
    {

        public Agent agent {private set; get; }


        public void SetAgent(Agent agent)
        {
            if (this.agent == null)
            {
                this.agent = agent;
                return;
            }

            throw new ArgumentException("Can set a agent multi times to a physic component!");
        }

        public T GetComponent<T>() where T : AbstractPhysicComponent
        {
            if (this.agent != null)
            {
                return this.agent.GetComponent<T>();
            }
            return null;
        }

    }
}
