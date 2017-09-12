using System;
using System.Collections.Generic;

namespace Engine.Network.Interfaces
{
    public interface IOrderProcessor
    {
        void ProcessOrder(IOrderManager orderManager, INetWorld world, int clientId, IOrder order);
    }
}
