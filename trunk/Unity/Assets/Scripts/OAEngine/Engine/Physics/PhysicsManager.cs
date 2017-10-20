using System;
using System.Collections.Generic;
using TrueSync;

namespace Engine.Physics
{
    public class PhysicsManager : IPhysicsManagerBase
    {
        public enum PhysicsType { W_2D, W_3D };

        public static IPhysicsManager instance;


        public static IPhysicsManager New()
        {
            instance = new Physics2DWorldManager();
            instance.Gravity = TSVector.zero;
            instance.SpeculativeContacts = false;

            return instance;
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void UpdateStep()
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

        public void RemoveBody(IBody iBody)
        {
            throw new NotImplementedException();
        }
    }
}
