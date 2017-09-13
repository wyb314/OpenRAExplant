using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Enums;
using Engine.Network.Interfaces;
using Engine.Support;

namespace Engine.Network.Defaults
{
    public class OrderProcessorDefault : IOrderProcessor<ClientDefault>
    {
        public void ProcessOrder(IOrderManager<ClientDefault> orderManager, INetWorld world, int clientId, IOrder order)
        {
            Log.Write("wyb","Process OrderString->"+order.OrderString);

            switch (order.TargetString)
            {
                case "Message":
                    Log.Write("wyb",order.TargetString);
                    break;
                case "Disconnected":
                    {
                        var client = orderManager.LobbyInfo.ClientWithIndex(clientId);
                        if (client != null)
                            client.State = ClientState.Disconnected;
                        break;
                    }
                case "StartGame":
                    break;
                case "PauseGame":
                    {
                        var client = orderManager.LobbyInfo.ClientWithIndex(clientId);
                        if (client != null)
                        {
                            Log.LogError("wyb", "Pause game :"+order.TargetString);
                            var pause = order.TargetString == "Pause";
                            orderManager.World.Paused = pause;
                            //orderManager.World.PredictedPaused = pause;
                        }
                        break;
                    }
                case "HandshakeRequest":
                    break;
                case "ServerError":
                    {
                        orderManager.ServerError = order.TargetString;
                        orderManager.AuthenticationFailed = false;
                        break;
                    }
                case "AuthenticationError":
                    {
                        orderManager.ServerError = order.TargetString;
                        orderManager.AuthenticationFailed = false;
                        break;
                    }
                case "SyncInfo":
                    {
                        orderManager.LobbyInfo = Session<ClientDefault>.Deserialize(order.TargetString);
                        SetOrderLag(orderManager);
                        Game.SyncLobbyInfo();
                        break;
                    }
                case "SyncLobbyClients":
                    {
                        //var clients = new List<IClient>();
                        //var nodes = MiniYaml.FromString(order.TargetString);
                        //foreach (var node in nodes)
                        //{
                        //    var strings = node.Key.Split('@');
                        //    if (strings[0] == "Client")
                        //        clients.Add(Session.Client.Deserialize(node.Value));
                        //}

                        //orderManager.LobbyInfo.Clients = clients;
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
                        orderManager.IssueOrder(Order.Pong(order.TargetString));
                        break;
                    }
                default:
                    break;

            }
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
