using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Engine.Network.Interfaces
{
    public interface IServerConnectoin<T> where T : IClient
    {
        int PlayerIndex { get; }

        int MostRecentFrame { get; }

        long TimeSinceLastResponse { get; }

        bool TimeoutMessageShown { set;get; }

        Socket Socket { get; }

        void ReadData(IServer<T> server);
    }
}
