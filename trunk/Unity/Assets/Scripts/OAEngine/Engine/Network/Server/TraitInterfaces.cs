using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;

namespace Engine.Network.Server
{
    public interface IInterpretCommand<T, U> where T : IClient where U : IClientPing
    {
        bool InterpretCommand(IServer<T,U> server, IServerConnectoin<T, U> conn, T client, string cmd);
    }

    public interface INotifySyncLobbyInfo<T, U> where T : IClient where U : IClientPing
    {
        void LobbyInfoSynced(IServer<T, U> server);
    }

    public interface INotifyServerStart<T, U> where T : IClient where U : IClientPing
    {
        void ServerStarted(IServer<T, U> server);
    }

    public interface INotifyServerEmpty<T, U> where T : IClient where U : IClientPing
    {
        void ServerEmpty(IServer<T, U> server);
    }

    public interface INotifyServerShutdown<T, U> where T : IClient where U : IClientPing
    {
        void ServerShutdown(IServer<T, U> server);
    }

    public interface IStartGame<T, U> where T : IClient where U : IClientPing
    {
        void GameStarted(IServer<T, U> server);
    }

    public interface IClientJoined<T, U> where T : IClient where U : IClientPing
    {
        void ClientJoined(IServer<T, U> server, IServerConnectoin<T, U> conn);
    }

    public interface IEndGame<T, U> where T : IClient where U : IClientPing
    {
        void GameEnded(IServer<T, U> server);
    }

    public interface ITick<T, U> where T : IClient where U : IClientPing
    {
        void Tick(IServer<T, U> server);
        int TickTimeout { get; }
    }

    public abstract class ServerTrait { }
    
}
