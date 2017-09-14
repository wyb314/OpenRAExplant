using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Network.Enums;
using Engine.Network.Interfaces;
using Engine.Primitives;
using Engine.Support;

namespace Engine.Network.Defaults
{
    public class OrderProcessorDefault : IOrderProcessor<ClientDefault>
    {
        public void ProcessOrder(IOrderManager<ClientDefault> orderManager, INetWorld world, int clientId, IOrder order)
        {
            Log.Write("wyb","Process OrderString->"+order.OrderString);

            switch (order.OrderString)
            {
                case "Message":
                    Log.Write("wyb",Encoding.UTF8.GetString(order.ExtDatas));
                    break;
                case "Disconnected":
                    {
                        var client = orderManager.LobbyInfo.ClientWithIndex(clientId);
                        if (client != null)
                            client.State = ClientState.Disconnected;
                        break;
                    }
                case "StartGame":
                    {
                        //if (Game.ModData.MapCache[orderManager.LobbyInfo.GlobalSettings.Map].Status != MapStatus.Available)
                        //{
                        //    Game.Disconnect();
                        //    Game.LoadShellMap();

                        //    // TODO: After adding a startup error dialog, notify the replay load failure.
                        //    break;
                        //}
                        
                        Game.StartGame(orderManager.LobbyInfo.GlobalSettings.Map, WorldType.Regular);
                        break;
                    }
                case "PauseGame":
                    {
                        var client = orderManager.LobbyInfo.ClientWithIndex(clientId);
                        if (client != null)
                        {
                            
                            var pause = Encoding.UTF8.GetString(order.ExtDatas) == "Pause";
                            Log.LogError("wyb", "Pause game :" + pause);
                            orderManager.World.Paused = pause;
                            //orderManager.World.PredictedPaused = pause;
                        }
                        break;
                    }
                case "HandshakeRequest":
                    {
                        Order o = order as Order;
                        HandshakeRequest handshake = HandshakeRequest.Deserialize(o.ExtDatas);

                        Log.Write("wyb", "HandshakeRequest Mod->{0} map->{1} version->{2}".F(handshake.Mod,handshake.Map,handshake.Version));

                        break;
                    }
                case "ServerError":
                    {
                        orderManager.ServerError = Encoding.UTF8.GetString(order.ExtDatas);
                        orderManager.AuthenticationFailed = false;
                        break;
                    }
                case "AuthenticationError":
                    {
                        orderManager.ServerError = Encoding.UTF8.GetString(order.ExtDatas);
                        orderManager.AuthenticationFailed = false;
                        break;
                    }
                case "SyncInfo":
                    {
                        Order o = order as Order;
                        orderManager.LobbyInfo = DeserializeSession(o.ExtDatas);
                        SetOrderLag(orderManager);
                        Game.SyncLobbyInfo();
                        break;
                    }
                case "SyncLobbyClients":
                    {
                        Order o = order as Order;
                        var clients = o.ExtDatas.ReadToBytes();
                        
                        orderManager.LobbyInfo.Clients = clients;
                        Game.SyncLobbyInfo();
                        break;
                    }
                case "SyncLobbySlots":
                    break;
                case "SyncLobbyGlobalSettings":
                    break;
                case "SyncClientPings":
                    break;
                case "Ping":
                    {
                        orderManager.IssueOrder(Order.Pong(Encoding.UTF8.GetString(order.ExtDatas)));
                        break;
                    }
                default:
                    break;

            }
        }


        public static Session<ClientDefault> DeserializeSession(byte[] data)
        {
            Session<ClientDefault> session = new Session<ClientDefault>();

            using (var ms = new MemoryStream(data))
            {
                BinaryReader br = new BinaryReader(ms);
                int clientDatasCount = br.ReadInt32();
                List<ClientDefault> clientDats = new List<ClientDefault>(clientDatasCount);
                session.Clients = clientDats;
                for (int i = 0; i < clientDatasCount; i++)
                {
                    int clientDataByteCount = br.ReadInt32();
                    ClientDefault client = ClientDefault.Deserialize(br.ReadBytes(clientDataByteCount));
                    clientDats.Add(client);
                }

                int clientPingsCount = br.ReadInt32();
                List<IClientPing> clientPings = new List<IClientPing>(clientPingsCount);
                session.ClientPings = clientPings;
                for (int i = 0; i < clientPingsCount; i++)
                {
                    int clientPingByteCount = br.ReadInt32();
                    ClientPingDefault clientPing = ClientPingDefault.Deserialize(br.ReadBytes(clientPingByteCount));
                    clientPings.Add(clientPing);
                }

                int globalDataCount = br.ReadInt32();
                GlobalDefault global = GlobalDefault.Deserialize(br.ReadBytes(globalDataCount));
                session.GlobalSettings = global;

            }
            return session;
        }

        static void SetOrderLag(IOrderManager<ClientDefault> o)
        {
            if (o.FramesAhead != o.LobbyInfo.GlobalSettings.OrderLatency && !o.GameStarted)
            {
                o.FramesAhead = o.LobbyInfo.GlobalSettings.OrderLatency;
                Log.Write("server", "Order lag is now {0} frames.", o.LobbyInfo.GlobalSettings.OrderLatency);
            }
        }
    }
}
