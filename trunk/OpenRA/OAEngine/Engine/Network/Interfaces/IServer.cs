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
    public interface IServer<T, U> where T : IClient where U : IClientPing
    {
        IPAddress Ip { get; }

        int Port { get; }
        
        List<IServerConnectoin<T, U>> Conns { get; }

        List<IServerConnectoin<T,U>> PreConns { get; }
        
        Session<T, U> LobbyInfo { get; }

        TypeDictionary serverTraits { get; }

        ServerState State { get; }

        bool Dedicated { get; }

        ModData ModData { get; }

        void StartGame();

        void EndGame();

        void DropClient(IServerConnectoin<T, U> toDrop);

        void Shutdown();
        
        T GetClient(IServerConnectoin<T, U> conn);

        void InterpretServerOrder(IServerConnectoin<T, U> conn, IServerOrder so);
    }
}
