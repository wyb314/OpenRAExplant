using System;
using System.Collections.Generic;

namespace Engine.Network.Interfaces
{
    public interface IOrderProcessor<T> where T : IClient
    {
        void ProcessOrder(IOrderManager<T> orderManager, INetWorld world, int clientId, IOrder order);
    }
}
