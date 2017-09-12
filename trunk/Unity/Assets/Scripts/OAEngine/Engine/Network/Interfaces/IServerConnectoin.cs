using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Engine.Network.Interfaces
{
    public interface IServerConnectoin<T, U> where T : IClient where U : IClientPing
    {
        int PlayerIndex { get; }

        int MostRecentFrame { get; }

        Socket Socket { get; }

        void ReadData(IServer<T, U> server);
    }
}
