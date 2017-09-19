
using System.Collections.Generic;
using Engine.Network.Defaults;
using Engine.Support;

namespace Engine.Interfaces
{
    public interface IOrderGenerator
    {
        IInputsGetter inputer { get; }
        void Tick(IWorld world);
        IEnumerable<Order> Order(World world, byte opCode, byte extCode);
    }
}
