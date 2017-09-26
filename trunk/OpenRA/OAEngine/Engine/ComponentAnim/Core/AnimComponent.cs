using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.Primitives;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim.Core
{
    public class AnimComponent : IActionHandler
    {
        private AnimFSM FSM;

        private Animation Animation;
        private Agent Owner;


        public AnimComponent(Agent owner,AnimFSM fsm)
        {
            this.Owner = owner;
            this.FSM = fsm;
        }

        public void Init()
        {
            FSM.Initialize();
            Owner.BlackBoard.ActionHandlerAdd(this);
        }

        public void Update()
        {
            FSM.UpdateAnimStates();
        }


        public void HandleAction(AgentAction action)
        {
            if (action.IsFailed())
                return;

            FSM.DoAction(action);
        }

        public void Activate()
        {
            Animation.Stop();
            Animation.Rewind();
            FSM.Initialize();
        }

        public void Deactivate()
        {
            FSM.Reset();
        }

    }
}
