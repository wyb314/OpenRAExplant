using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Primitives;

namespace Engine
{
    public class Actor
    {
        public readonly World World;

        public Actor(World world, string name, TypeDictionary initDict)
        {
            this.World = world;

        }

        public void Tick()
        {
        }
    }
}
