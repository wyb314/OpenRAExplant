using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Maps;
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

        static void CheckAutoStart(ServerDefault server)
        {
            var nonBotPlayers = server.LobbyInfo.NonBotPlayers;

            // Are all players and admin (could be spectating) ready?
            if (nonBotPlayers.Any(c => c.State != ClientState.Ready) ||
                server.LobbyInfo.Clients.First(c => c.IsAdmin).State != ClientState.Ready)
                return;

            // Does server have at least 2 human players?
            if (!server.LobbyInfo.GlobalSettings.EnableSingleplayer && nonBotPlayers.Count() < 2)
                return;

            // Are the map conditions satisfied?
            if (server.LobbyInfo.Slots.Any(sl => sl.Value.Required && server.LobbyInfo.ClientInSlot(sl.Key) == null))
                return;

            server.StartGame();
        }


        public bool InterpretCommand(IServer<ClientDefault> server, IServerConnectoin<ClientDefault> conn, ClientDefault client, byte[] data)
        {
            string cmd = Encoding.UTF8.GetString(data);
            Log.Write("server", "InterpretCommand->" + cmd);

            if (server == null || conn == null || client == null || !ValidateCommand(server, conn, client, cmd))
                return false;
            var cmdName = cmd.Split(' ').First();
            var cmdValue = cmd.Split(' ').Skip(1).JoinWith(" ");
            

            switch (cmdName)
            {
                case "state":
                {
                    var state = ClientState.Invalid;
                    if (!Enum<ClientState>.TryParse(cmdValue, false, out state))
                    {
                        server.SendOrderTo(conn, "Message", "Malformed state command");
                        return true;
                    }
                        
                    client.State = state;

                    Log.Write("server", "Player @{0} is {1}",
                        conn.Socket.RemoteEndPoint, client.State);
                    ServerDefault sd = server as ServerDefault;
                    sd.SyncLobbyClients();

                    CheckAutoStart(sd);

                    return true;
                }
                case "startgame":
                {
                    if (!client.IsAdmin)
                    {
                        server.SendOrderTo(conn, "Message", "Only the host can start the game.");
                        return true;
                    }

                    if (server.LobbyInfo.Slots.Any(sl => sl.Value.Required &&
                        server.LobbyInfo.ClientInSlot(sl.Key) == null))
                    {
                        server.SendOrderTo(conn, "Message", "Unable to start the game until required slots are full.");
                        return true;
                    }

                    if (!server.LobbyInfo.GlobalSettings.EnableSingleplayer && server.LobbyInfo.NonBotPlayers.Count() < 2)
                    {
                        server.SendOrderTo(conn, "Message", ServerConst.TwoHumansRequiredText);
                        return true;
                    }

                    server.StartGame();
                    return true;
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
            //var uid = server.LobbyInfo.GlobalSettings.Map;
            //server.Map = server.ModData.MapCache[uid];
            //if (server.Map.Status != MapStatus.Available)
            //    throw new InvalidOperationException("Map {0} not found".F(uid));

            ServerDefault _server = server as ServerDefault;
            server.LobbyInfo.Slots = _server.Map.Players
                .Select(p => MakeSlotFromPlayerReference(p))
                .Where(s => s != null)
                .ToDictionary(s => s.PlayerReference, s => s as ISlot);

            //LoadMapSettings(server, server.LobbyInfo.GlobalSettings, server.Map.Rules);
        }

        static SlotDefault MakeSlotFromPlayerReference(PlayerReference pr)
        {
            if (!pr.Playable) return null;
            return new SlotDefault
            {
                PlayerReference = pr.Name,
                Closed = false,
                AllowBots = pr.AllowBots,
                LockFaction = pr.LockFaction,
                //LockColor = pr.LockColor,
                LockTeam = pr.LockTeam,
                LockSpawn = pr.LockSpawn,
                Required = pr.Required,
            };
        }
    }
}
