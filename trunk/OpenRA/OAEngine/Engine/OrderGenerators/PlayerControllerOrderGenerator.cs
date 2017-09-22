using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Inputs;
using Engine.Interfaces;
using Engine.Network.Defaults;
using Engine.Support;
using ORDER = Engine.Network.Defaults.Order;

namespace Engine.OrderGenerators
{
    public sealed class PlayerControllerOrderGenerator : IOrderGenerator
    {
        
        public PlayerControllerOrderGenerator()
        {
        }

        public IEnumerable<Order> Order(World world, byte opCode, byte extCode)
        {

            yield break;
        }

        public void Tick(IWorld world)
        {
            
        }
        

    }
}
