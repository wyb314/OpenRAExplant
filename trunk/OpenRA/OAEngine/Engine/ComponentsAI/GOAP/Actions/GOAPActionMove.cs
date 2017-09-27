﻿using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AgentActions;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.Factories;
using Engine.ComponentsAI.GOAP.Core;
using Engine.Primitives;

namespace Engine.ComponentsAI.GOAP.Actions
{
    class GOAPActionMove : GOAPAction
    {
        private AgentActionMove Action;
        private WPos FinalPos;

        public GOAPActionMove(Agent owner) : base(E_GOAPAction.move, owner) { }


        public override void InitAction()
        {
            WorldEffects.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
            Cost = 5;
            Precedence = 30;
        }

        public override void Update()
        {
            if (WorldEffects.GetWSProperty(E_PropKey.E_AT_TARGET_POS).GetBool() == true)
            {
                Action = AgentActionFactory.Create(AgentActionFactory.E_Type.E_MOVE) as AgentActionMove;
                Action.MoveType = AgentActionMove.E_MoveType.E_MT_FORWARD;
                Owner.BlackBoard.ActionAdd(Action);
            }
            else
            {
                //  Debug.Log("CreateOrderStop");
                AgentActionIdle a = AgentActionFactory.Create(AgentActionFactory.E_Type.E_IDLE) as AgentActionIdle;
                Owner.BlackBoard.ActionAdd(a);
            }
        }

        public override void Activate()
        {
            base.Activate();

            Action = AgentActionFactory.Create(AgentActionFactory.E_Type.E_MOVE) as AgentActionMove;
            Action.MoveType = AgentActionMove.E_MoveType.E_MT_FORWARD;
            Owner.BlackBoard.ActionAdd(Action);

        }

        public override void Deactivate()
        {
            base.Deactivate();

            Owner.WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);

            //   Debug.Log("CreateOrderStop");
            AgentActionIdle a = AgentActionFactory.Create(AgentActionFactory.E_Type.E_IDLE) as AgentActionIdle;
            Owner.BlackBoard.ActionAdd(a);

        }

        public override bool IsActionComplete()
        {
            if (Action != null && Action.IsActive() == false)
            {
                //UnityEngine.Debug.Log(this.ToString() + " complete !");
                return true;
            }

            return false;
        }

        public override bool ValidateAction()
        {

            if (Action != null && Action.IsFailed() == true)
            {
                //UnityEngine.Debug.Log(this.ToString() + " not valid anymore !" + FinalPos.ToString());
                return false;
            }

            return true;
        }

        /*private void ActionRotate(Vector3 direction)
        {
            RotateAction = AgentActionFactory.Create(AgentActionFactory.E_Type.Rotate) as AgentActionRotate;
            RotateAction.Direction = direction;
            Owner.BlackBoard.ActionAdd(RotateAction);
        }*/
    }
}
