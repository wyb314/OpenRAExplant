using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Enums;
using Engine.Network.Interfaces;
using Engine.Network.Server;
using Engine.Support;

namespace Engine.Network.Defaults.ServerTraits
{
    public class LobbyCommands : ServerTrait,
        IInterpretCommand<ClientDefault>,
        INotifyServerStart<ClientDefault>,
        INotifyServerEmpty<ClientDefault>,
        IClientJoined<ClientDefault>
    {
        public LobbyCommands()
        {
        }
        public static bool ValidateCommand(IServer<ClientDefault> server, IServerConnectoin<ClientDefault> conn, ClientDefault client, string cmd)
        {
            if (server.State == ServerState.GameStarted)
            {
                server.SendOrderTo(conn, "Message", "Cannot change state when game started. ({0})".F(cmd));
                return false;
            }
            else if (client.State == ClientState.Ready && !(cmd.StartsWith("state") || cmd == "startgame"))
            {
                server.SendOrderTo(conn, "Message", "Cannot change state when marked as ready.");
                return false;
            }

            return true;
        }

        public void ClientJoined(IServer<ClientDefault> server, IServerConnectoin<ClientDefault> conn)
        {
        }

        public bool InterpretCommand(IServer<ClientDefault> server, IServerConnectoin<ClientDefault> conn, ClientDefault client, string cmd)
        {
            if (server == null || conn == null || client == null || !ValidateCommand(server, conn, client, cmd))
                return false;
            switch (cmd)
            {
                case "state":
                {
                    break;
                }
                default:
                {
                    break;
                }

            }

            return true;
        }

        public void ServerEmpty(IServer<ClientDefault> server)
        {
        }

        public void ServerStarted(IServer<ClientDefault> server)
        {
            Log.Write("wyb", "LobbyCommands ServerStarted!");
        }
    }
}
