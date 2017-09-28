using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface IOrderManager<T> : IDisposable where T : IClient
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

        int FramesAhead { set;get; }
        
        IConnection Connection { get; }

        List<IOrder> localOrders { get; }

        IFrameData<T> frameData { get; }

        IOrderSerializer orderSerializer { get; }

        IOrderProcessor<T> orderProcessor { get; }

        ISyncReport syncReport { get; }

        INetWorld World { set; get; }

        T LocalClient { get; }

        bool GameStarted { get; }

        bool IsReadyForNextFrame { get; }

        Session<T> LobbyInfo { set;get; }

        string ServerError { set; get; }

        bool AuthenticationFailed { set; get; }

        void StartGame();


        void IssueOrders(IOrder[] orders);


        void IssueOrder(IOrder order);

        void TickImmediate();
        
        void Tick();
    }
}
