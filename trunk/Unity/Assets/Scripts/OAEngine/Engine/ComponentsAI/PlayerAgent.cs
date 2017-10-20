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
using Engine.Physics;
using Engine.Primitives;
using Engine.Support;
using OAEngine.Engine.ComponentsAI;
using TrueSyncPhysics;
using TrueSync;

namespace Engine.ComponentsAI
{
    public class PlayerAgent : Agent
    {
        public override TSVector2 Position
        {
            set
            {
                this.Transform2D.position = value;
                //this.self.Pos = value;
            }
            get
            {
                return this.Transform2D.position;
                //return this.self.Pos;
            }
        }

        public override FP Facing
        {
            get
            {
                return this.Transform2D.rotation;
                //return this.self.Facing;
            }
            set
            {
                this.Transform2D.rotation = value;
                //this.self.Facing = value;
            }
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

        public override MersenneTwister Random
        {
            get { return this.self.Random; }
        }

        public Actor self { private set; get; }

        public IRender rendererProxy { private set; get; }

        public override E_ComboLevel[] ComboLevel
        {
            get { return this.self.ComboLevel; }
        }

        public override E_SwordLevel SwordLevel
        {
            get { return this.self.SwordLevel; }
        }

        public override TSVector2 Forward
        {
            get { return MathUtils.FacingToTSVector2(this.Facing); }
        }

        public override World world
        {
            get { return this.self.World; }
        }

        #region
        public TSTransform2D Transform2D { private set; get; }

        private TSRigidBody2D rigidBody2D;

        public override TSRigidBody2D RigidBody2D
        {
            get { return this.rigidBody2D; }
        }

        public TSCircleCollider2D CircleCollider2D { private set; get; }


        private IRegidbodyWrapObject rendererObject;
        public override IRegidbodyWrapObject RendererObject
        {
            get { return this.rendererObject; }
        }
        #endregion

        public PlayerAgent(Actor self, IRender render,AnimSet animSet)
        {
            this.self = self;
            this.rendererProxy = render;
            this.rendererObject = new PlayerRegidbodyWarper(this);
            this.AnimSet = animSet;
        }

        public void Init()
        {
            this.ApplyPhysics();
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


        private void ApplyPhysics()
        {
            this.Transform2D = new TSTransform2D();
            
            this.AddComponent(Transform2D);

            this.rigidBody2D = new TSRigidBody2D();
            
            this.AddComponent(RigidBody2D);

            this.CircleCollider2D = new TSCircleCollider2D();

            this.AddComponent(CircleCollider2D);

            this.Transform2D.Init();
            this.CircleCollider2D.Init();
            this.Transform2D.position = this.self.Pos;
            this.Transform2D.rotation = this.self.Facing;
            this.RigidBody2D.isKinematic = true;

            PhysicsManager.instance.AddBody(this.CircleCollider2D);
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

            //this.Transform2D.Update();
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
