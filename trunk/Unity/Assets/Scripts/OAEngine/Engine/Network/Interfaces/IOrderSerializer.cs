using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Engine.Network.Interfaces
{
    public interface IOrderSerializer
    {
        IOrder Deserialize(INetWorld world, BinaryReader r);
        
        List<IOrder> Deserialize(INetWorld world, byte[] bytes);

        byte[] Serialize(IOrder order);
    }
}
