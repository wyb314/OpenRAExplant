using System;
using System.Collections.Generic;

namespace Engine.Physics
{
    public class PhysicsManager
    {
        public enum PhysicsType { W_2D, W_3D };

        public static IPhysicsManager instance;
    }
}
