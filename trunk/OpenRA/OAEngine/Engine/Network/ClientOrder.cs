using System;
using System.Collections.Generic;
using Engine.Network.Interfaces;

namespace Engine.Network
{
    public struct ClientOrder
    {
        public int Client;
        public IOrder Order;

        public override string ToString()
        {
            return "ClientId: {0} {1}".F(Client, Order);
        }
    }
}
