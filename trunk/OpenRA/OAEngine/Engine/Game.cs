using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Engine.Primitives;
using Engine.Support;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine;
using Engine.FileSystem;
using Engine.Interfaces;
using Engine.Maps;
using TrueSync;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Engine
{
    public enum RunStatus
    {
        Error = -1,
        Success = 0,
        Running = int.MaxValue
    }
    public static class Game
    {
        public const int NetTickScale = 3;
        public const int Timestep = 23;
        public const int TimestepJankThreshold = 250; // Don't catch up for delays larger than 250ms

        public static bool BenchmarkMode = false;

        public static event Action LobbyInfoChanged = () => { };
        public static event Action<string, int> OnRemoteDirectConnect = (a, b) => { };
        public static event Action<IOrderManager<ClientDefault>> ConnectionStateChanged = _ => { };
        public static event Action BeforeGameStart = () => { };
        static volatile ActionQueue delayedActions = new ActionQueue();
        public static event Action OnQuit = () => { };

        static ConnectionState lastConnectionState = ConnectionState.PreConnecting;
        internal static IOrderManager<ClientDefault> OrderManager;

        static RunStatus state = RunStatus.Running;

        static IWorldRenderer worldRenderer;

        public static ModData ModData;
        public static Settings Settings;

        static IServer<ClientDefault> server;
        static Stopwatch stopwatch = Stopwatch.StartNew();
        public static long RunTime { get { return stopwatch.ElapsedMilliseconds; } }

        internal static void Initialize(Arguments args, IPlatformImpl platformInfo = null)
        {
            Platform.SetCurrentPlatform(platformInfo);
            Log.SetLogger(platformInfo.Logger);

            Log.AddChannel("perf", "perf.log");
            Log.AddChannel("debug", "debug.log");
            Log.AddChannel("server", "server.log");
            Log.AddChannel("sound", "sound.log");
            Log.AddChannel("graphics", "graphics.log");
            Log.AddChannel("geoip", "geoip.log");
            Log.AddChannel("irc", "irc.log");
            Log.AddChannel("nat", "nat.log");
            Log.AddChannel("wyb", "wyb.log");

            InitializeSettings(args);
            var modID = args.GetValue("Game.Mod", null);
            Log.Write("wyb","Mod: {0} .".F(modID));
            InitializeMod(modID,args);
        }

        public static void InitializeSettings(Arguments args)
        {
            string settingPath = Platform.ResolvePath("^settings.yaml");
            Settings = YamlHelper.Deserialize<Settings>(settingPath);
        }


        public static void InitializeMod(string modID, Arguments args)
        {
            LobbyInfoChanged = () => { };
            ConnectionStateChanged = om => { };
            BeforeGameStart = () => { };
            OnRemoteDirectConnect = (a, b) => { };
            delayedActions = new ActionQueue();

            if (worldRenderer != null)
            {
                worldRenderer.Dispose();
                worldRenderer = null;
            }
            if (server != null)
            {
                server.Dispose();
            }
            if (OrderManager != null)
            {
                OrderManager.Dispose();
            }
            ModData = null;
            if (modID == null)
                throw new InvalidOperationException("Game.Mod argument missing.");
            string manifestPath = Platform.ModsDir + Path.DirectorySeparatorChar + modID + Path.DirectorySeparatorChar + "mod.yaml";
            Manifest manifest = YamlHelper.Deserialize<Manifest>(manifestPath);
            manifest.Id = modID;
            ModData = new ModData(manifest);
            
            JoinLocal();
        }

        static void JoinLocal()
        {
            JoinInner(new OrderManagerDefault("<no server>", -1, "", new EchoConnection()));
        }

        static void JoinInner(OrderManagerDefault om)
        {
            if (OrderManager != null) OrderManager.Dispose();
            OrderManager = om;
            lastConnectionState = ConnectionState.PreConnecting;
            ConnectionStateChanged(OrderManager);
        }

        internal static RunStatus Run()
        {
            
            Platform.platformInfo.LogicTick = LogicTick;
            //Platform.platformInfo.Tick = Tick;
            Platform.platformInfo.OnApplicationQuit = OnApplicationQuit;
            
            return state;
        }

        public static int LocalTick { get { return OrderManager.LocalFrameNumber; } }

        public static int WorldTick { get { return OrderManager.World.WorldTick; } }

        public static FP WorldTime { get { return OrderManager.World.WorldTick * Game.Timestep / new FP(1000); } }

        static void LogicTick(float elapsedTime)
        {
            IWorld world = OrderManager.World as IWorld;

            Sync.CheckSyncUnchanged(world, OrderManager.TickImmediate);

            if (world == null)
                return;
            
            var isNetTick = LocalTick % NetTickScale == 0;

            if (!isNetTick || OrderManager.IsReadyForNextFrame)
            {
                ++OrderManager.LocalFrameNumber;

                //Log.Write("debug", "--Tick: {0} ({1})", LocalTick, isNetTick ? "net" : "local");

                if (BenchmarkMode)
                    Log.Write("cpu", "{0};{1}".F(LocalTick, PerfHistory.Items["tick_time"].LastValue));

                if (isNetTick)
                    OrderManager.Tick();

                Sync.CheckSyncUnchanged(world, () =>
                {
                    world.OrderGenerator.Tick(world);
                    //world.Selection.Tick(world);
                });

                world.Tick();

                PerfHistory.Tick();
            }
            //else if (OrderManager.NetFrameNumber == 0)
            //{
            //    //OrderManager.LastTickTime = RunTime;
            //}


            // Wait until we have done our first world Tick before TickRendering
            if (OrderManager.LocalFrameNumber > 0)
            {
                Sync.CheckSyncUnchanged
                    (world, () => world.TickRender(worldRenderer));
            }

        }
        
        //static void Tick(float elapsedTime)
        //{
        //    IWorld world = OrderManager.World as IWorld;
            
        //    if (world == null)
        //        return;

        //    world.OrderGenerator.Tick(world);

        //    if (OrderManager.LocalFrameNumber > 0)
        //    {
        //        Sync.CheckSyncUnchanged
        //            (world, () => world.TickRender(worldRenderer));
        //    }
        //}

        public static void CreateAndStartLocalServer(string mapUID, IEnumerable<Order> setupOrders)
        {
            IOrderManager<ClientDefault> om = null;

            Action lobbyReady = null;
            lobbyReady = () =>
            {
                LobbyInfoChanged -= lobbyReady;
                foreach (var o in setupOrders)
                    om.IssueOrder(o);
            };

            LobbyInfoChanged += lobbyReady;

            string ip = IPAddress.Loopback.ToString();
            om = JoinServer(ip, CreateLocalServer(mapUID), "",false);
        }

        public static int CreateLocalServer(string map)
        {
            //TODO:
            var settings = new ServerSettings()
            {
                Name = "Skirmish Game",
                Map = map,
                AdvertiseOnline = false,
                AllowPortForward = false
            };

            if (server != null)
            {
                server.Dispose();
            }
            server = new ServerDefault(new IPEndPoint(IPAddress.Loopback, 0), settings, ModData, false);

            return server.Port;
        }


        public static IOrderManager<ClientDefault> JoinServer(string host, int port, string password, bool recordReplay = true)
        {
            var connection = new NetworkConnection(host, port);
            //if (recordReplay)
            //    connection.StartRecording(() => { return TimestampedFilename(); });

            var om = new OrderManagerDefault(host, port, password, connection);
            JoinInner(om);
            return om;
        }

        internal static void StartGame(string mapUID, WorldType type)
        {
            // Dispose of the old world before creating a new one.
            if (worldRenderer != null)
                worldRenderer.Dispose();

            //Cursor.SetCursor(null);
            BeforeGameStart();

            Map map;

            IWorld world = OrderManager.World as IWorld;
            
            using (new PerfTimer("PrepareMap"))
                map = ModData.PrepareMap(mapUID);
            using (new PerfTimer("NewWorld"))
            {
                world = new World(ModData, map, OrderManager, type);
                OrderManager.World = world;
            }
               
            worldRenderer = new WorldRenderer(ModData, world);

            using (new PerfTimer("LoadComplete"))
            {
                world.LoadComplete(worldRenderer);
            }

            if (OrderManager.GameStarted)
                return;

            //Ui.MouseFocusWidget = null;
            //Ui.KeyboardFocusWidget = null;

            OrderManager.LocalFrameNumber = 0;

            OrderManager.StartGame();

            AllUserInput(true);

            //worldRenderer.RefreshPalette();
            //Cursor.SetCursor("default");

            GC.Collect();
        }
        
        internal static void AllUserInput(bool allow)
        {
            if (Platform.platformInfo != null && Platform.platformInfo.inputter != null)
            {
                Platform.platformInfo.inputter.AllowGetInput = allow;
            }
        }

        internal static void SyncLobbyInfo()
        {
            LobbyInfoChanged();
        }

        public static void CloseServer()
        {
            if (server != null)
                server.Shutdown();
        }
        


        static void OnApplicationQuit()
        {
            if (server != null)
            {
                server.Dispose();
            }
            if (OrderManager != null)
            {
                OrderManager.Dispose();
            }
                
            OnQuit();
        }
    }
}
