using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;

namespace Engine.Network.Server
{
    public interface IInterpretCommand<T> where T : IClient
    {
        bool InterpretCommand(IServer<T> server, IServerConnectoin<T> conn, T client, byte[] data);
    }

    public interface INotifySyncLobbyInfo<T> where T : IClient 
    {
        void LobbyInfoSynced(IServer<T> server);
    }

    public interface INotifyServerStart<T> where T : IClient
    {
        void ServerStarted(IServer<T> server);
    }

    public interface INotifyServerEmpty<T> where T : IClient
    {
        void ServerEmpty(IServer<T> server);
    }

    public interface INotifyServerShutdown<T> where T : IClient
    {
        void ServerShutdown(IServer<T> server);
    }

    public interface IStartGame<T> where T : IClient
    {
        void GameStarted(IServer<T> server);
    }

    public interface IClientJoined<T> where T : IClient
    {
        void ClientJoined(IServer<T> server, IServerConnectoin<T> conn);
    }

    public interface IEndGame<T> where T : IClient
    {
        void GameEnded(IServer<T> server);
    }

    public interface ITick<T> where T : IClient 
    {
        void Tick(IServer<T> server);
        int TickTimeout { get; }
    }

    public abstract class ServerTrait { }
    
}
