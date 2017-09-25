using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Engine.Network.Enums;
using Engine.Network.Interfaces;
using Engine.Network.Server;
using Engine.Primitives;
using Engine.Support;
using System.Threading;

namespace Engine.Network.Defaults
{
    public class ServerDefault : IServer<ClientDefault>
    {
        public List<IServerConnectoin<ClientDefault>> Conns { private set; get; }

        public List<IServerConnectoin<ClientDefault>> PreConns { private set; get; }

        public IPAddress Ip { private set; get; }

        public Session<ClientDefault> LobbyInfo { private set; get; }

        public int Port { private set; get; }
        
        public TypeDictionary serverTraits { private set;get; }
        
        public ServerState State { private set; get; }

        public bool Dedicated { private set; get; }

        public ModData ModData { private set; get; }

        public ServerSettings ServerSettings { private set; get; }
        
        readonly int randomSeed;
        public readonly MersenneTwister Random = new MersenneTwister();
        readonly TcpListener listener;
        private readonly Thread serverThread;

        public ServerDefault(IPEndPoint endpoint, ServerSettings serverSettings, ModData modData, bool dedicated)
        {
            this.Conns = new List<IServerConnectoin<ClientDefault>>();
            this.PreConns = new List<IServerConnectoin<ClientDefault>>();

            listener = new TcpListener(endpoint);
            listener.Start();
            var localEndpoint = (IPEndPoint)listener.LocalEndpoint;
            Ip = localEndpoint.Address;
            Port = localEndpoint.Port;
            Dedicated = dedicated;
            ServerSettings = serverSettings;

            ServerSettings.Name = Engine.Settings.SanitizedServerName(ServerSettings.Name);

            ModData = modData;

            randomSeed  = (int)DateTime.Now.ToBinary();

            State = ServerState.WaitingPlayers;
            this.serverTraits = new TypeDictionary();
            // UPnP is only supported for servers created by the game client.
            //if (!dedicated && ServerSettings.AllowPortForward)
            //    UPnP.ForwardPort(ServerSettings.ListenPort, ServerSettings.ExternalPort).Wait();

            foreach (var trait in modData.Manifest.ServerTraits)
                serverTraits.Add(modData.ObjectCreator.CreateObject<ServerTrait>(trait));

            LobbyInfo = new Session<ClientDefault>
            {
                GlobalSettings = new GlobalDefault()
                {
                    RandomSeed = randomSeed,
                    Map = serverSettings.Map,
                    ServerName = serverSettings.Name,
                    EnableSingleplayer = serverSettings.EnableSingleplayer || !dedicated,
                    GameUid = Guid.NewGuid().ToString()
                }
            };

            this.serverThread = new Thread(_ =>
            {
                try
                {
                    foreach (var t in serverTraits.WithInterface<INotifyServerStart<ClientDefault>>())
                        t.ServerStarted(this);
                    Log.Write("server", "Initial map: {0}", LobbyInfo.GlobalSettings.Map);

                    var timeout = 5;
                    timeout = serverTraits.WithInterface<ITick<ClientDefault>>()
                                .Min(t => t.TickTimeout);

                    for (;;)
                    {
                        var checkRead = new List<Socket>();
                        if (State == ServerState.WaitingPlayers)
                            checkRead.Add(listener.Server);

                        checkRead.AddRange(Conns.Select(c => c.Socket));
                        checkRead.AddRange(PreConns.Select(c => c.Socket));

                        if (checkRead.Count > 0)
                            Socket.Select(checkRead, null, null, timeout);

                        if (State == ServerState.ShuttingDown)
                        {
                            EndGame();
                            break;
                        }

                        foreach (var s in checkRead)
                        {
                            if (s == listener.Server)
                            {
                                AcceptConnection();
                                continue;
                            }

                            var preConn = PreConns.SingleOrDefault(c => c.Socket == s);
                            if (preConn != null)
                            {
                                preConn.ReadData(this);
                                continue;
                            }

                            var conn = Conns.SingleOrDefault(c => c.Socket == s);
                            if (conn != null)
                                conn.ReadData(this);
                        }

                        foreach (var t in serverTraits.WithInterface<ITick<ClientDefault>>())
                            t.Tick(this);

                        if (State == ServerState.ShuttingDown)
                        {
                            EndGame();
                            //if (!dedicated && ServerSettings.AllowPortForward)
                            //    UPnP.RemovePortForward().Wait();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string log = "Error msg : {0} stackTrace->{1}".F(ex.Message, ex.StackTrace);
                    Log.Write("server", log);

                }



                foreach (var t in serverTraits.WithInterface<INotifyServerShutdown<ClientDefault>>())
                    t.ServerShutdown(this);

                PreConns.Clear();
                Conns.Clear();
                try
                {
                    listener.Stop();
                }
                catch
                {
                }
            })
            {
                IsBackground = true
            };

            this.serverThread.Start();

        }
        
        void AcceptConnection()
        {
            Socket newSocket;

            try
            {
                if (!listener.Server.IsBound)
                    return;

                newSocket = listener.AcceptSocket();
            }
            catch (Exception e)
            {
                /* TODO: Could have an exception here when listener 'goes away' when calling AcceptConnection! */
                /* Alternative would be to use locking but the listener doesn't go away without a reason. */
                //Log.Write("server", "Accepting the connection failed.", e);
                return;
            }

            Log.Write("server", "Accept a connection! ");
            var newConn = new ServerConnectoinDefault(){ Socket = newSocket };
            try
            {
                newConn.Socket.Blocking = false;
                newConn.Socket.NoDelay = true;

                // assign the player number.
                newConn.PlayerIndex = ChooseFreePlayerIndex();
                SendData(newConn.Socket, BitConverter.GetBytes(ProtocolVersion.Version));
                SendData(newConn.Socket, BitConverter.GetBytes(newConn.PlayerIndex));
                PreConns.Add(newConn);
                
                // Dispatch a handshake order
                var request = new HandshakeRequest
                {
                    Mod = ModData != null ? ModData.Manifest.Id : "wyb",
                    Version = ModData != null ? ModData.Manifest.Metadata.Version : "Develop",
                    Map = LobbyInfo.GlobalSettings.Map
                };

                DispatchOrdersToClient(newConn, 0, 0, new ServerOrderDefault("HandshakeRequest", request.Serialize()).Serialize());
            }
            catch (Exception e)
            {
                DropClient(newConn);
                Log.Write("server", "Dropping client {0} because handshake failed: {1}", newConn.PlayerIndex.ToString(CultureInfo.InvariantCulture), e);
            }
        }

        int nextPlayerIndex;
        public int ChooseFreePlayerIndex()
        {
            return nextPlayerIndex++;
        }

        void DispatchOrdersToClient(IServerConnectoin<ClientDefault> c, int client, int frame, byte[] data)
        {
            try
            {
                SendData(c.Socket, BitConverter.GetBytes(data.Length + 4));
                SendData(c.Socket, BitConverter.GetBytes(client));
                SendData(c.Socket, BitConverter.GetBytes(frame));
                SendData(c.Socket, data);
            }
            catch (Exception e)
            {
                DropClient(c);
                Log.Write("server", "Dropping client {0} because dispatching orders failed: {1}",
                    client.ToString(CultureInfo.InvariantCulture), e);
            }
        }

        static void SendData(Socket s, byte[] data)
        {
            var start = 0;
            var length = data.Length;

            // Non-blocking sends are free to send only part of the data
            while (start < length)
            {
                SocketError error;
                var sent = s.Send(data, start, length - start, SocketFlags.None, out error);
                if (error == SocketError.WouldBlock)
                {
                    //Log.Write("server", "Non-blocking send of {0} bytes failed. Falling back to blocking send.", length - start);
                    s.Blocking = true;
                    sent = s.Send(data, start, length - start, SocketFlags.None);
                    s.Blocking = false;
                }
                else if (error != SocketError.Success)
                    throw new SocketException((int)error);

                start += sent;
            }
        }

        public void DropClient(IServerConnectoin<ClientDefault> toDrop)
        {
            if (!PreConns.Remove(toDrop))
            {
                Conns.Remove(toDrop);

                var dropClient = LobbyInfo.Clients.FirstOrDefault(c1 => c1.Index == toDrop.PlayerIndex);
                if (dropClient == null)
                    return;

                var suffix = "";
                if (State == ServerState.GameStarted)
                    suffix = dropClient.IsObserver ? " (Spectator)" :  "";
                SendMessage("{0}{1} has disconnected.".F(dropClient.Name, suffix));

                // Send disconnected order, even if still in the lobby
                DispatchOrdersToClients(toDrop, 0, new ServerOrderDefault("Disconnected", null).Serialize());

                LobbyInfo.Clients.RemoveAll(c => c.Index == toDrop.PlayerIndex);
                LobbyInfo.ClientPings.RemoveAll(p => p.Index == toDrop.PlayerIndex);

                // Client was the server admin
                // TODO: Reassign admin for game in progress via an order
                if (Dedicated && dropClient.IsAdmin && State == ServerState.WaitingPlayers)
                {
                    // Remove any bots controlled by the admin
                    LobbyInfo.Clients.RemoveAll(c => c.Bot != null && c.BotControllerClientIndex == toDrop.PlayerIndex);

                    var nextAdmin = LobbyInfo.Clients.Where(c1 => c1.Bot == null)
                        .MinByOrDefault(c => c.Index);

                    if (nextAdmin != null)
                    {
                        nextAdmin.IsAdmin = true;
                        SendMessage("{0} is now the admin.".F(nextAdmin.Name));
                    }
                }

                DispatchOrders(toDrop, toDrop.MostRecentFrame, new byte[] { 0xbf });

                // All clients have left: clean up
                if (!Conns.Any())
                    foreach (var t in serverTraits.WithInterface<INotifyServerEmpty<ClientDefault>>())
                        t.ServerEmpty(this);

                if (Conns.Any() || Dedicated)
                    SyncLobbyClients();

                if (!Dedicated && dropClient.IsAdmin)
                    Shutdown();
            }

            try
            {
                toDrop.Socket.Disconnect(false);
            }
            catch { }
        }

        public void SendMessage(string text)
        {
            DispatchOrdersToClients(null, 0, new ServerOrderDefault("Message",Encoding.UTF8.GetBytes(text)).Serialize());

            if (Dedicated)
                Console.WriteLine("[{0}] {1}".F(DateTime.Now.ToString(ServerSettings.TimestampFormat), text));
        }

        public void SyncLobbyClients()
        {
            if (State != ServerState.WaitingPlayers)
                return;

            // TODO: Only need to sync the specific client that has changed to avoid conflicts!
            //var clientData = LobbyInfo.Clients.Select(client => client.Serialize()).ToList();

            DispatchOrders(null, 0, new ServerOrderDefault("SyncLobbyClients", LobbyInfo.Clients.WriteToBytes()).Serialize());

            foreach (var t in serverTraits.WithInterface<INotifySyncLobbyInfo<ClientDefault>>())
                t.LobbyInfoSynced(this);
        }

        public void DispatchOrders(IServerConnectoin<ClientDefault> conn, int frame, byte[] data)
        {
            if (frame == 0 && conn != null)
            {
                //string log = "DispatchOrders InterpretServerOrders PlayerIndex->{0} frame->{1}".F(conn.PlayerIndex, frame);
                //Log.Write("wybserver", log);
                //Console.WriteLine(log);
                InterpretServerOrders(conn, data);
            }
            else
            {
                //string log = "DispatchOrders DispatchOrdersToClients PlayerIndex->{0} frame->{1}".F(conn != null ? conn.PlayerIndex : -1, frame);
                //Log.Write("wybserver", log);
                //Console.WriteLine(log);
                DispatchOrdersToClients(conn, frame, data);
            }

        }

        void InterpretServerOrders(IServerConnectoin<ClientDefault> conn, byte[] data)
        {

            var ms = new MemoryStream(data);
            var br = new BinaryReader(ms);

            try
            {
                while (ms.Position < ms.Length)
                {
                    var so = ServerOrderDefault.Deserialize(br);
                    if (so == null) return;
                    InterpretServerOrder(conn, so);
                }
            }
            catch (EndOfStreamException) { }
            catch (NotImplementedException) { }
        }

        /// <summary>
        /// 解析特殊消息
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="so"></param>
        public void InterpretServerOrder(IServerConnectoin<ClientDefault> conn, IServerOrder so)
        {
            string log = "InterpretServerOrder so name->{0}".F(so.Name);
            Log.Write("wybserver", log);

            switch (so.Name)
            {
                case "Command":
                    {
                        var handledBy = serverTraits.WithInterface<IInterpretCommand<ClientDefault>>()
                            .FirstOrDefault(t => t.InterpretCommand(this, conn, GetClient(conn), so.Data));

                        if (handledBy == null)
                        {
                            Log.Write("server", "Unknown server command: {0}", so.Data);
                            SendOrderTo(conn, "Message", "Unknown server command: {0}".F(so.Data));
                        }

                        break;
                    }
                case "HandshakeResponse":
                    {
                        ValidateClient(conn, so.Data);
                        break;
                    }
                case "Pong":
                    {
                        long pingSent;
                        if (!NetworkExts.TryParseInt64Invariant(Encoding.UTF8.GetString(so.Data), out pingSent))
                        {
                            Log.Write("server", "Invalid order pong payload: {0}", so.Data);
                            break;
                        }

                        var client = GetClient(conn);
                        if (client == null)
                            return;

                        var pingFromClient = LobbyInfo.PingFromClient(client);
                        if (pingFromClient == null)
                            return;

                        var history = pingFromClient.LatencyHistory.ToList();
                        history.Add(Game.RunTime - pingSent);

                        // Cap ping history at 5 values (25 seconds)
                        if (history.Count > 5)
                            history.RemoveRange(0, history.Count - 5);

                        pingFromClient.Latency = history.Sum() / history.Count;
                        pingFromClient.LatencyJitter = (history.Max() - history.Min()) / 2;
                        pingFromClient.LatencyHistory = history.ToArray();

                        SyncClientPing();
                        break;
                    }
                default:
                    break;
            }
        }

        public void SyncClientPing()
        {
            // TODO: Split this further into per client ping orders
            var clientPings = LobbyInfo.ClientPings.Select(ping => ping as ClientPingDefault).ToList();

            DispatchOrders(null, 0, new ServerOrderDefault("SyncClientPings", clientPings.WriteToBytes()).Serialize());

            foreach (var t in serverTraits.WithInterface<INotifySyncLobbyInfo<ClientDefault>>())
                t.LobbyInfoSynced(this);
        }

        void ValidateClient(IServerConnectoin<ClientDefault> newConn, byte[] data)
        {
            try
            {
                if (State == ServerState.GameStarted)
                {
                    Log.Write("server", "Rejected connection from {0}; game is already started.",
                        newConn.Socket.RemoteEndPoint);

                    SendOrderTo(newConn, "ServerError", "The game has already started");
                    DropClient(newConn);
                    return;
                }

                var handshake = HandshakeResponse.Deserialize(data);

                if (!string.IsNullOrEmpty(ServerSettings.Password) && handshake.Password != ServerSettings.Password)
                {
                    var message = string.IsNullOrEmpty(handshake.Password) ? "Server requires a password" : "Incorrect password";
                    SendOrderTo(newConn, "AuthenticationError", message);
                    DropClient(newConn);
                    return;
                }

                var client = new ClientDefault
                {
                    Name = Settings.SanitizedPlayerName(handshake.Client.Name),
                    IpAddress = ((IPEndPoint)newConn.Socket.RemoteEndPoint).Address.ToString(),
                    Index = newConn.PlayerIndex,
                    Slot = LobbyInfo.FirstEmptySlot(),
                    State = ClientState.Invalid,
                    IsAdmin = !LobbyInfo.Clients.Any(c1 => c1.IsAdmin)
                };

                if (client.IsObserver && !LobbyInfo.GlobalSettings.AllowSpectators)
                {
                    SendOrderTo(newConn, "ServerError", "The game is full");
                    DropClient(newConn);
                    return;
                }

                //if (client.Slot != null)
                //    SyncClientToPlayerReference(client, Map.Players.Players[client.Slot]);
                //else
                //    client.Color = HSLColor.FromRGB(255, 255, 255);

                if (ModData.Manifest.Id != handshake.Mod)
                {
                    Log.Write("server", "Rejected connection from {0}; mods do not match.",
                        newConn.Socket.RemoteEndPoint);

                    SendOrderTo(newConn, "ServerError", "Server is running an incompatible mod");
                    DropClient(newConn);
                    return;
                }

                if (ModData.Manifest.Metadata.Version != handshake.Version && !LobbyInfo.GlobalSettings.AllowVersionMismatch)
                {
                    Log.Write("server", "Rejected connection from {0}; Not running the same version.",
                        newConn.Socket.RemoteEndPoint);

                    SendOrderTo(newConn, "ServerError", "Server is running an incompatible version");
                    DropClient(newConn);
                    return;
                }

                // 禁止某些Ip连接服务器
                //var bans = Settings.Ban.Union(TempBans);
                //if (bans.Contains(client.IpAddress))
                //{
                //    Log.Write("server", "Rejected connection from {0}; Banned.", newConn.Socket.RemoteEndPoint);
                //    SendOrderTo(newConn, "ServerError", "You have been {0} from the server".F(Settings.Ban.Contains(client.IpAddress) ? "banned" : "temporarily banned"));
                //    DropClient(newConn);
                //    return;
                //}

                // Promote connection to a valid client
                PreConns.Remove(newConn);
                Conns.Add(newConn);
                LobbyInfo.Clients.Add(client);
                var clientPing = new ClientPingDefault { Index = client.Index };
                LobbyInfo.ClientPings.Add(clientPing);

                Log.Write("server", "Client {0}: Accepted connection from {1}.",
                    newConn.PlayerIndex, newConn.Socket.RemoteEndPoint);

                foreach (var t in serverTraits.WithInterface<IClientJoined<ClientDefault>>())
                    t.ClientJoined(this, newConn);

                SyncLobbyInfo();

                Log.Write("server", "{0} ({1}) has joined the game.",
                    client.Name, newConn.Socket.RemoteEndPoint);

                if (LobbyInfo.NonBotClients.Count() > 1)
                    SendMessage("{0} has joined the game.".F(client.Name));

                // Send initial ping
                SendOrderTo(newConn, "Ping", Game.RunTime.ToString(CultureInfo.InvariantCulture));

                if (Dedicated)
                {
                    var motdFile = Platform.ResolvePath("^", "motd.txt");
                    if (!File.Exists(motdFile))
                        File.WriteAllText(motdFile, "Welcome, have fun and good luck!");

                    var motd = File.ReadAllText(motdFile);
                    if (!string.IsNullOrEmpty(motd))
                        SendOrderTo(newConn, "Message", motd);
                }

                //if (Map.DefinesUnsafeCustomRules)
                //    SendOrderTo(newConn, "Message", "This map contains custom rules. Game experience may change.");

                //if (!LobbyInfo.GlobalSettings.EnableSingleplayer)
                //    SendOrderTo(newConn, "Message", TwoHumansRequiredText);
                //else if (Map.Players.Players.Where(p => p.Value.Playable).All(p => !p.Value.AllowBots))
                //    SendOrderTo(newConn, "Message", "Bots have been disabled on this map.");

                if (handshake.Mod == "{DEV_VERSION}")
                    SendMessage("{0} is running an unversioned development build, ".F(client.Name) +
                        "and may desynchronize the game state if they have incompatible rules.");
            }
            catch (Exception ex)
            {
                Log.Write("server", "Dropping connection {0} because an error occurred:", newConn.Socket.RemoteEndPoint);
                Log.Write("server", "Serer Error msg->{0} stackTrace->{1}".F(ex.Message,ex.StackTrace));
                DropClient(newConn);
            }
        }

        //IServerConnectoin<ClientDefault, ClientPingDefault>
        public void DispatchOrdersToClients(IServerConnectoin<ClientDefault> conn, int frame, byte[] data)
        {
            var from = conn != null ? conn.PlayerIndex : 0;
            foreach (var c in Conns.Except(conn).ToList())
            {
                DispatchOrdersToClient(c, from, frame, data);
            }
                
        }

        public void EndGame()
        {
            foreach (var t in serverTraits.WithInterface<IEndGame<ClientDefault>>())
                t.GameEnded(this);
        }

        public ClientDefault GetClient(IServerConnectoin<ClientDefault> conn)
        {
            return LobbyInfo.ClientWithIndex(conn.PlayerIndex);
        }

        public void Shutdown()
        {
            State = ServerState.ShuttingDown;
        }

        public void StartGame()
        {
            listener.Stop();

            Console.WriteLine("[{0}] Game started", DateTime.Now.ToString(ServerSettings.TimestampFormat));

            // Drop any unvalidated clients
            foreach (var c in PreConns.ToArray())
                DropClient(c);

            // Drop any players who are not ready
            foreach (var c in Conns.Where(c => GetClient(c).IsInvalid).ToArray())
            {
                SendOrderTo(c, "ServerError", "You have been kicked from the server!");
                DropClient(c);
            }

            // HACK: Turn down the latency if there is only one real player
            if (LobbyInfo.NonBotClients.Count() == 1)
                LobbyInfo.GlobalSettings.OrderLatency = 1;

            SyncLobbyInfo();
            State = ServerState.GameStarted;

            foreach (var c in Conns)
                foreach (var d in Conns)
                    DispatchOrdersToClient(c, d.PlayerIndex, 0x7FFFFFFF, new byte[] { 0xBF });

            DispatchOrders(null, 0,
                new ServerOrderDefault("StartGame", null).Serialize());

            foreach (var t in serverTraits.WithInterface<IStartGame<ClientDefault>>())
                t.GameStarted(this);
        }

        public void SendOrderTo(IServerConnectoin<ClientDefault> conn, string order, string data)
        {
            DispatchOrdersToClient(conn, 0, 0, new ServerOrderDefault(order, Encoding.UTF8.GetBytes(data)).Serialize());
        }

        public void SyncLobbyInfo()
        {
            if (State == ServerState.WaitingPlayers) // Don't do this while the game is running, it breaks things!
                DispatchOrders(null, 0, new ServerOrderDefault("SyncInfo", LobbyInfo.Serialize()).Serialize());

            foreach (var t in serverTraits.WithInterface<INotifySyncLobbyInfo<ClientDefault>>())
                t.LobbyInfoSynced(this);
        }

        public void Dispose()
        {
            if (this.serverThread != null)
            {
                this.serverThread.Abort();
            }
            
            this.Shutdown();
        }
    }
}
