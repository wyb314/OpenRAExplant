using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Engine.Primitives;
using Engine.Support;
using Engine.Network.Enums;

namespace Engine.Network.Interfaces
{
    public interface IServer<T> : IDisposable  where T : IClient
    {
        IPAddress Ip { get; }

        int Port { get; }
        
        List<IServerConnectoin<T>> Conns { get; }

        List<IServerConnectoin<T>> PreConns { get; }
        
        Session<T> LobbyInfo { get; }

        TypeDictionary serverTraits { get; }

        ServerState State { get; }

        bool Dedicated { get; }

        ModData ModData { get; }

        void StartGame();

        void EndGame();

        void DropClient(IServerConnectoin<T> toDrop);

        void Shutdown();
        
        T GetClient(IServerConnectoin<T> conn);

        void InterpretServerOrder(IServerConnectoin<T> conn, IServerOrder so);
    }
}
