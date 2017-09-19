using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Network.Defaults;

namespace Engine.OrderGenerators
{
    public sealed class PlayerControllerOrderGenerator : IOrderGenerator
    {
        public IEnumerable<Order> Order(World world, byte opCode, byte extCode)
        {

            yield break;
        }

        public void Tick(IWorld world)
        {
            
        }
    }
}
