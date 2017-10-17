using System;
using System.Collections.Generic;
using Physics;
using TrueSync;
using TrueSync.Physics2D;

namespace Engine.Physics
{
    public class Physics2DWorldManager : IPhysicsManager
    {
        public static Physics2DWorldManager instance;

        private World world;

        Dictionary<IBody, IRandererObject> gameObjectMap;

        //Dictionary<Body, Dictionary<Body, TSCollision2D>> collisionInfo;

        public TSVector Gravity
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public FP LockedTimeStep
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool SpeculativeContacts
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddBody(ICollider iCollider)
        {
            throw new NotImplementedException();
        }

        public int GetBodyLayer(IBody rigidBody)
        {
            throw new NotImplementedException();
        }

        public IRandererObject GetGameObject(IBody rigidBody)
        {
            throw new NotImplementedException();
        }

        public IWorld GetWorld()
        {
            throw new NotImplementedException();
        }

        public IWorldClone GetWorldClone()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public bool IsCollisionEnabled(IBody rigidBody1, IBody rigidBody2)
        {
            throw new NotImplementedException();
        }

        public void OnRemoveBody(Action<IBody> OnRemoveBody)
        {
            throw new NotImplementedException();
        }

        public void RemoveBody(IBody iBody)
        {
            throw new NotImplementedException();
        }

        public void UpdateStep()
        {
            throw new NotImplementedException();
        }
    }
}
