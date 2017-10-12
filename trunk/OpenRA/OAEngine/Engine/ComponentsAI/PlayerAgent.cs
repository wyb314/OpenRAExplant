using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentAnim;
using Engine.ComponentAnim.Core;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.GOAP;
using Engine.ComponentsAI.GOAP.Core;
using Engine.ComponentsAI.WorkingMemory;
using Engine.Interfaces;
using Engine.Primitives;
using OAEngine.Engine.ComponentsAI;
using TrueSync;

namespace Engine.ComponentsAI
{
    public class PlayerAgent : Agent
    {
        public override TSVector2 Position
        {
            set { this.self.Pos = value; }
            get { return this.self.Pos; }
        }

        public override TSQuaternion Rotation
        {
            get { return this.self.Facing; }
            set { this.self.Facing = value; }
        }

        public override FP TurnSpeed
        {
            get { return this.self.TurnSpeed; }
            set { this.self.TurnSpeed = value; }
        }

        public override TSVector2 CurJoystickDir
        {
            get { return this.self.CurJoystickDir; }
        }

        public Actor self { private set; get; }

        public IRender rendererProxy { private set; get; }

        public PlayerAgent(Actor self, IRender render,AnimSet animSet)
        {
            this.self = self;
            this.rendererProxy = render;
            this.AnimSet = animSet;
        }

        public void Init()
        {
            this.WorldState = new WorldState();
            this.m_GoalManager = new GOAPManager(this);
            this.Memory = new Memory();
            this.BlackBoard = new BlackBoard();
            this.BlackBoard.Owner = this;

            this.ResetAgent();

            WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

            WorldState.SetWSProperty(E_PropKey.E_IDLING, true);
            WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
            WorldState.SetWSProperty(E_PropKey.E_ATTACK_TARGET, false);
            WorldState.SetWSProperty(E_PropKey.E_LOOKING_AT_TARGET, false);
            WorldState.SetWSProperty(E_PropKey.E_USE_WORLD_OBJECT, false);
            WorldState.SetWSProperty(E_PropKey.E_PLAY_ANIM, false);

            WorldState.SetWSProperty(E_PropKey.E_IN_DODGE, false);
            WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);
            WorldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
            WorldState.SetWSProperty(E_PropKey.E_IN_BLOCK, false);
            WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
            WorldState.SetWSProperty(E_PropKey.E_IN_COMBAT_RANGE, false);
            WorldState.SetWSProperty(E_PropKey.E_AHEAD_OF_ENEMY, false);
            WorldState.SetWSProperty(E_PropKey.E_BEHIND_ENEMY, false);
            WorldState.SetWSProperty(E_PropKey.MoveToRight, false);
            WorldState.SetWSProperty(E_PropKey.MoveToLeft, false);
            WorldState.SetWSProperty(E_PropKey.E_TELEPORT, false);

            WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);

            this.BlackBoard.DontUpdate = false;
        }



        public void Tick()
        {
            this.UpdateAgent();
        }

        void UpdateAgent()
        {
            if (BlackBoard.DontUpdate == true)
                return;

            //update blackboard
            BlackBoard.Update();

            m_GoalManager.UpdateCurrentGoal();

            //Manage the list of goals we have
            m_GoalManager.ManageGoals();

            //Update the working memory.Cleans up facts marked for deletion
            Memory.Tick();
        }


        private void ResetAgent()
        {
            WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);

            //StopAllCoroutines();
            BlackBoard.Reset();
            WorldState.Reset();
            Memory.Reset();
            m_GoalManager.Reset();
        }




    }
}
