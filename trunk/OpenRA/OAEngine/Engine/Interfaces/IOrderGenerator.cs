
using System.Collections.Generic;
using Engine.Inputs;
using Engine.Network.Defaults;
using Engine.Support;

namespace Engine.Interfaces
{
    public interface IOrderGenerator
    {
        void Tick(IWorld world);
        IEnumerable<Order> Order(World world, byte opCode, byte extCode);
    }
}
