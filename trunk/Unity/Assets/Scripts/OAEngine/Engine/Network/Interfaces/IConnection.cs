using System;
using System.Collections.Generic;

namespace Engine.Network.Interfaces
{
    public interface IConnection : IDisposable
    {
        int LocalClientId { get; }
        ConnectionState ConnectionState { get; }
        void Send(int frame, List<byte[]> orders);
        void SendImmediate(List<byte[]> orders);
        void SendSync(int frame, byte[] syncData);
        void Receive(Action<int, byte[]> packetFn);
    }
}
