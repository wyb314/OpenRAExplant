using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRA.Network;

namespace OpenRA.Server
{
    public interface ITick
    {
        void Tick(Server server);
        int TickTimeout { get; }
    }
    public interface IEndGame { void GameEnded(Server server); }

    public abstract class ServerTrait { }

    public interface IInterpretCommand { bool InterpretCommand(Server server, Connection conn, Session.Client client, string cmd); }

    public interface IStartGame { void GameStarted(Server server); }

    public interface INotifySyncLobbyInfo { void LobbyInfoSynced(Server server); }

    public interface INotifyServerStart { void ServerStarted(Server server); }

    public interface INotifyServerEmpty { void ServerEmpty(Server server); }

    public interface INotifyServerShutdown { void ServerShutdown(Server server); }

    public class DebugServerTrait : ServerTrait, IInterpretCommand, IStartGame, INotifySyncLobbyInfo, INotifyServerStart, INotifyServerShutdown, IEndGame
    {
        public bool InterpretCommand(Server server, Connection conn, Session.Client client, string cmd)
        {
            Console.WriteLine("Server received command from player {1}: {0}", cmd, conn.PlayerIndex);
            return false;
        }

        public void GameStarted(Server server)
        {
            Console.WriteLine("GameStarted()");
        }

        public void LobbyInfoSynced(Server server)
        {
            Console.WriteLine("LobbyInfoSynced()");
        }

        public void ServerStarted(Server server)
        {
            Console.WriteLine("ServerStarted()");
        }

        public void ServerShutdown(Server server)
        {
            Console.WriteLine("ServerShutdown()");
        }

        public void GameEnded(Server server)
        {
            Console.WriteLine("GameEnded()");
        }
    }

    public interface IClientJoined { void ClientJoined(Server server, Connection conn); }
}
