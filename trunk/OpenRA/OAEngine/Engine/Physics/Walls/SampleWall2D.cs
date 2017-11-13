using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using OAEngine.Engine.Physics;
using TrueSync;
using TrueSyncPhysics;

namespace Engine.Physics.Walls
{
    public class SampleWall2D
    {
        public TSTransform2D tran { private set; get; }

        public TSBoxCollider2D collider { private set; get; }

        private Dictionary<Type, IAgentComponent> components = new Dictionary<Type, IAgentComponent>();

        public IRegidbodyWrapObject RendererObject
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public SampleWall2D(TSVector2 pos , TSVector2 size)
        {
            this.tran = new TSTransform2D();
           
            this.tran.Init();
            this.collider = new TSBoxCollider2D();
            this.tran.tsCollider = this.collider;
            this.collider.Init();
            this.collider.tsTransform = this.tran;

            this.tran.position = pos;
            this.collider.size = size;
        }

        public void AddToPhysicWorld()
        {
            PhysicsManager.instance.AddBody(this.collider);
        }


    }
}
