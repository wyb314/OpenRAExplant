using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface IOrderManager : IDisposable
    {
        /// <summary>
        /// IP
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Port
        /// </summary>
        int Port { get; }

        string Password { get; }
        
        int NetFrameNumber { get; }

        int LocalFrameNumber { set;get; }

        int FramesAhead { get; }
        
        IConnection Connection { get; }

        List<IOrder> localOrders { get; }

        IFrameData frameData { get; }

        IOrderSerializer orderSerializer { get; }

        IOrderProcessor orderProcessor { get; }

        ISyncReport syncReport { get; }

        INetWorld World { get; }

        bool IsReadyForNextFrame { get; }
        
        void StartGame();


        void IssueOrders(IOrder[] orders);


        void IssueOrder(IOrder order);

        void TickImmediate();
        
        void Tick();
    }
}
