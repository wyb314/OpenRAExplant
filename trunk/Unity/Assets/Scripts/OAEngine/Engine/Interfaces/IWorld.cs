using System;
using System.Collections.Generic;
using Engine.Network.Interfaces;


namespace Engine.Interfaces
{
    public interface IWorld : INetWorld
    {
        IOrderGenerator OrderGenerator { set; get; }

        void LoadComplete(IWorldRenderer worldRenderer);

        void TickRender(IWorldRenderer worldRenderer);

        void ProcessOrder(int clientId, IOrder order);

        void IssureOrder(IOrder order);
    }
}
