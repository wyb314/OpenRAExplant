
using System.Collections.Generic;
using Engine.Network.Defaults;

namespace Engine.Interfaces
{
    public interface IOrderGenerator
    {
        void Tick(IWorld world);
        IEnumerable<Order> Order(World world, byte opCode, byte extCode);
    }
}
