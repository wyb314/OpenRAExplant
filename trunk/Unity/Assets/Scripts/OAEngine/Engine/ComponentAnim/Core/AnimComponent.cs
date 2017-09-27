using System;
using System.Collections.Generic;
using Engine.ComponentAnim._FSMs;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.Primitives;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;

namespace Engine.ComponentAnim.Core
{
    public class AnimComponent : IActionHandler
    {
        private AnimFSM FSM;

        private Animation Animation;
        private Agent Owner;


        public AnimComponent(Agent owner,Animation animation)
        {
            this.Owner = owner;
            this.Animation = animation;
            this.FSM = new AnimFSMPlayer(this.Animation,owner);
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
