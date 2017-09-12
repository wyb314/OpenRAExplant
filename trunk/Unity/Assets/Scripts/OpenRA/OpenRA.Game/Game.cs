using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using  System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using OpenRA.Graphics;
using OpenRA.Network;
using OpenRA.Primitives;
using OpenRA.Support;

namespace OpenRA
{
    public enum RunStatus
    {
        Error = -1,
        Success = 0,
        Running = int.MaxValue
    }
    public static class Game
    {
        public const int NetTickScale = 3; // 120 ms net tick for 40 ms local tick
        public const int Timestep = 40;
        public const int TimestepJankThreshold = 250; // Don't catch up for delays larger than 250ms

        public static InstalledMods Mods { get; private set; }
        public static ExternalMods ExternalMods { get; private set; }



        static WorldRenderer worldRenderer;


        public static string EngineVersion { get; private set; }

        public static ModData ModData;
        public static Settings Settings;

        // More accurate replacement for Environment.TickCount
        static Stopwatch stopwatch = Stopwatch.StartNew();
        public static long RunTime { get { return stopwatch.ElapsedMilliseconds; } }
        
        public static int RenderFrame = 0;
        public static int NetFrameNumber { get { return OrderManager.NetFrameNumber; } }
        public static int LocalTick { get { return OrderManager.LocalFrameNumber; } }

        internal static OrderManager OrderManager;
        static Server.Server server;

        public static MersenneTwister CosmeticRandom = new MersenneTwister(); // not synced

        public static Renderer Renderer;

        public static Sound Sound;

        public static bool BenchmarkMode = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        /// <param name="recordReplay">是否记录重放</param>
        /// <returns></returns>
        public static OrderManager JoinServer(string host, int port, string password, bool recordReplay = false)
        {
            var connection = new NetworkConnection(host, port);
            if (recordReplay)
                connection.StartRecording(() => { return TimestampedFilename(); });

            var om = new OrderManager(host, port, password, connection);
            JoinInner(om);
            return om;
        }

        static string TimestampedFilename(bool includemilliseconds = false)
        {
            var format = includemilliseconds ? "yyyy-MM-ddTHHmmssfffZ" : "yyyy-MM-ddTHHmmssZ";
            return "OpenRA-" + DateTime.UtcNow.ToString(format, CultureInfo.InvariantCulture);
        }


        public static event Action<string, int> OnRemoteDirectConnect = (a, b) => { };
        public static event Action<OrderManager> ConnectionStateChanged = _ => { };
        static ConnectionState lastConnectionState = ConnectionState.PreConnecting;


        public static event Action LobbyInfoChanged = () => { };

        internal static void SyncLobbyInfo()
        {
            LobbyInfoChanged();
        }

        public static event Action BeforeGameStart = () => { };

        internal static void StartGame(string mapUID, WorldType type)
        {
            // Dispose of the old world before creating a new one.
            if (worldRenderer != null)
                worldRenderer.Dispose();

            //Cursor.SetCursor(null);
            BeforeGameStart();

            Map map = null;

            //using (new PerfTimer("PrepareMap"))
            //    map = ModData.PrepareMap(mapUID);
            using (new PerfTimer("NewWorld"))
                OrderManager.World = new World(ModData, map, OrderManager, type);

            worldRenderer = new WorldRenderer(ModData, OrderManager.World);

            using (new PerfTimer("LoadComplete"))
                OrderManager.World.LoadComplete(worldRenderer);

            if (OrderManager.GameStarted)
                return;

            //Ui.MouseFocusWidget = null;
            //Ui.KeyboardFocusWidget = null;

            OrderManager.LocalFrameNumber = 0;
            OrderManager.LastTickTime = RunTime;
            OrderManager.StartGame();
            //worldRenderer.RefreshPalette();
            //Cursor.SetCursor("default");

            GC.Collect();
        }

        public static void RestartGame()
        {
            var replay = OrderManager.Connection as ReplayConnection;
            var replayName = replay != null ? replay.Filename : null;
            var lobbyInfo = OrderManager.LobbyInfo;
            var orders = new[] {
                    Order.Command("sync_lobby {0}".F(lobbyInfo.Serialize())),
                    Order.Command("startgame")
            };

            // Disconnect from the current game
            Disconnect();
            //Ui.ResetAll();

            // Restart the game with the same replay/mission
            if (replay != null)
                JoinReplay(replayName);
            else
                CreateAndStartLocalServer(lobbyInfo.GlobalSettings.Map, orders);
        }

        public static void CreateAndStartLocalServer(string mapUID, IEnumerable<Order> setupOrders)
        {
            OrderManager om = null;

            Action lobbyReady = null;
            lobbyReady = () =>
            {
                LobbyInfoChanged -= lobbyReady;
                foreach (var o in setupOrders)
                    om.IssueOrder(o);
            };

            LobbyInfoChanged += lobbyReady;

            om = JoinServer(IPAddress.Loopback.ToString(), CreateLocalServer(mapUID), "");
        }
        

        public static bool IsHost
        {
            get
            {
                var id = OrderManager.Connection.LocalClientId;
                var client = OrderManager.LobbyInfo.ClientWithIndex(id);
                return client != null && client.IsAdmin;
            }
        }

        static RunStatus state = RunStatus.Running;
        public static event Action OnQuit = () => { };

        static volatile ActionQueue delayedActions = new ActionQueue();

        public static void RunAfterTick(Action a) { delayedActions.Add(a, RunTime); }
        public static void RunAfterDelay(int delayMilliseconds, Action a) { delayedActions.Add(a, RunTime + delayMilliseconds); }

        internal static void Initialize(Arguments args , IPlatformImpl platformInfo = null)
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

            // Load the engine version as early as possible so it can be written to exception logs
            try
            {
                EngineVersion = File.ReadAllText(Platform.ResolvePath(@"./VERSION")).Trim();
            }
            catch { }

            if(string.IsNullOrEmpty(EngineVersion))
            {
                EngineVersion = "Unknown";
            }

            Log.LogError("EngineVersion->" + EngineVersion,"wyb");
            // log engine version

            var modID = args.GetValue("Game.Mod", null);
            var explicitModPaths = new string[0];
            if (modID != null && (File.Exists(modID) || Directory.Exists(modID)))
            {
                explicitModPaths = new[] { modID };
                modID = Path.GetFileNameWithoutExtension(modID);
            }

            InitializeSettings(args);
            
            var modSearchArg = args.GetValue("Engine.ModSearchPaths", null);
            var modSearchPaths = modSearchArg != null ?
                FieldLoader.GetValue<string[]>("Engine.ModsPath", modSearchArg) :
                new[] { Path.Combine(".", "mods") };
            Mods = new InstalledMods(modSearchPaths, explicitModPaths);
            //ExternalMods = new ExternalMods();

            //Manifest currentMod;
            //if (modID != null && Mods.TryGetValue(modID, out currentMod))
            //{
            //    var launchPath = args.GetValue("Engine.LaunchPath", Assembly.GetEntryAssembly().Location);

            //    // Sanitize input from platform-specific launchers
            //    // Process.Start requires paths to not be quoted, even if they contain spaces
            //    if (launchPath.First() == '"' && launchPath.Last() == '"')
            //        launchPath = launchPath.Substring(1, launchPath.Length - 2);

            //    ExternalMods.Register(Mods[modID], launchPath, ModRegistration.User);

            //    ExternalMod activeMod;
            //    if (ExternalMods.TryGetValue(ExternalMod.MakeKey(Mods[modID]), out activeMod))
            //        ExternalMods.ClearInvalidRegistrations(activeMod, ModRegistration.User);
            //}

            InitializeMod(modID, args);
        }


        public static void InitializeMod(string mod, Arguments args)
        {
            // Clear static state if we have switched mods
            LobbyInfoChanged = () => { };
            ConnectionStateChanged = om => { };
            BeforeGameStart = () => { };
            OnRemoteDirectConnect = (a, b) => { };
            delayedActions = new ActionQueue();

            //Ui.ResetAll();//Clear all UI

            if (worldRenderer != null)
                worldRenderer.Dispose();
            worldRenderer = null;
            if (server != null)
                server.Shutdown();
            if (OrderManager != null)
                OrderManager.Dispose();

            if (ModData != null)
            {
                ModData.ModFiles.UnmountAll();
                ModData.Dispose();
            }

            ModData = null;

            if (mod == null)
                throw new InvalidOperationException("Game.Mod argument missing.");

            if (!Mods.ContainsKey(mod))
                throw new InvalidOperationException("Unknown or invalid mod '{0}'.".F(mod));

            Console.WriteLine("Loading mod: {0}", mod);

           // Sound.StopVideo();

            ModData = new ModData(Mods[mod], Mods, true);

            //if (!ModData.LoadScreen.BeforeLoad())
            //    return;

            using (new PerfTimer("LoadMaps"))
                ModData.MapCache.LoadMaps();

            ModData.InitializeLoaders(ModData.DefaultFileSystem);
            //Renderer.InitializeFonts(ModData);

            var grid = ModData.Manifest.Contains<MapGrid>() ? ModData.Manifest.Get<MapGrid>() : null;
           

            PerfHistory.Items["render"].HasNormalTick = false;
            PerfHistory.Items["batches"].HasNormalTick = false;
            PerfHistory.Items["render_widgets"].HasNormalTick = false;
            PerfHistory.Items["render_flip"].HasNormalTick = false;

            JoinLocal();

            return;
            try
            {
                //if (discoverNat != null) //NAT在引擎完整之后会补上
                //    discoverNat.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("NAT discovery failed: {0}", e.Message);
                Log.Write("nat", e.ToString());
                Settings.Server.AllowPortForward = false;
            }

            ModData.LoadScreen.StartGame(args);
        }

        public static void InitializeSettings(Arguments args)
        {
            Settings = new Settings(Platform.ResolvePath(@"^settings.yaml"), args);
        }

        public static void AddChatLine(Color color, string name, string text)
        {
            OrderManager.AddChatLine(color, name, text);
        }

        public static void Debug(string s, params object[] args)
        {
            AddChatLine(Color.white, "Debug", string.Format(s, args));
        }

        public static void Disconnect()
        {
            if (OrderManager.World != null)
                OrderManager.World.TraitDict.PrintReport();

            OrderManager.Dispose();
            CloseServer();
            JoinLocal();
        }


        static void JoinInner(OrderManager om)
        {
            if (OrderManager != null) OrderManager.Dispose();
            OrderManager = om;
            lastConnectionState = ConnectionState.PreConnecting;
            ConnectionStateChanged(OrderManager);
        }

        static void JoinLocal()
        {
            JoinInner(new OrderManager("<no server>", -1, "", new EchoConnection()));
        }

        public static void JoinReplay(string replayFile)
        {
            JoinInner(new OrderManager("<no server>", -1, "", new ReplayConnection(replayFile)));
        }

        public static void CloseServer()
        {
            if (server != null)
                server.Shutdown();
        }

        public static int CreateLocalServer(string map)
        {
            var settings = new ServerSettings()
            {
                Name = "Skirmish Game",
                Map = map,
                AdvertiseOnline = false,
                AllowPortForward = false
            };

            server = new Server.Server(new IPEndPoint(IPAddress.Loopback, 0), settings, ModData, false);

            return server.Port;
        }

        internal static RunStatus Run()
        {
            if (Settings.Graphics.MaxFramerate < 1)
            {
                Settings.Graphics.MaxFramerate = new GraphicSettings().MaxFramerate;
                Settings.Graphics.CapFramerate = false;
            }

            Platform.platformInfo.RegisterLogicTick(LogicTick);
            Platform.platformInfo.OnApplicationQuit = OnApplicationQuit;
            nextLogic = RunTime;
            nextRender = RunTime;
            forcedNextRender = RunTime;

            //try
            //{
            //    Loop();
            //}
            //finally
            //{
            //    // Ensure that the active replay is properly saved
            //    if (OrderManager != null)
            //        OrderManager.Dispose();
            //}

            //OnApplicationQuit();

            return state;
        }

        


        static void OnApplicationQuit()
        {
            if (OrderManager != null)
                OrderManager.Dispose();
            if (worldRenderer != null)
                worldRenderer.Dispose();
            if (ModData != null)
            {
                ModData.Dispose();
            }
            
            //ChromeProvider.Deinitialize();

            //GlobalChat.Dispose();
            //Sound.Dispose();
            if (Renderer != null)
            {
                Renderer.Dispose();
            }
            

            OnQuit();
        }


        const int MaxLogicTicksBehind = 250;

        const int MinReplayFps = 10;
        private static float nextLogic;
        private static float nextRender;
        private static float forcedNextRender;


        static void Loop(float elapsedTime)
        {
            // Ideal time between logic updates. Timestep = 0 means the game is paused
            // but we still call LogicTick() because it handles pausing internally.
            var logicInterval = worldRenderer != null && worldRenderer.World.Timestep != 0 ? worldRenderer.World.Timestep : Timestep;

            // Ideal time between screen updates
            var maxFramerate = Settings.Graphics.CapFramerate ? Settings.Graphics.MaxFramerate.Clamp(1, 1000) : 1000;
            var renderInterval = 1000 / maxFramerate;

            var now = RunTime;

            // If the logic has fallen behind too much, skip it and catch up
            if (now - nextLogic > MaxLogicTicksBehind)
                nextLogic = now;

            // When's the next update (logic or render)
            var nextUpdate = Math.Min(nextLogic, nextRender);
            if (now >= nextUpdate)
            {
                var forceRender = now >= forcedNextRender;

                if (now >= nextLogic)
                {
                    nextLogic += logicInterval;

                    LogicTick(1);

                    // Force at least one render per tick during regular gameplay
                    if (OrderManager.World != null && !OrderManager.World.IsReplay)
                        forceRender = true;
                }

                var haveSomeTimeUntilNextLogic = now < nextLogic;
                var isTimeToRender = now >= nextRender;

                if ((isTimeToRender && haveSomeTimeUntilNextLogic) || forceRender)
                {
                    nextRender = now + renderInterval;

                    var maxRenderInterval = Math.Max(1000 / MinReplayFps, renderInterval);
                    forcedNextRender = now + maxRenderInterval;

                    RenderTick();
                }
            }
        }

        static void LogicTick(float elapsedTime)
        {
            delayedActions.PerformActions(RunTime);

            if (OrderManager.Connection.ConnectionState != lastConnectionState)
            {
                lastConnectionState = OrderManager.Connection.ConnectionState;
                ConnectionStateChanged(OrderManager);
            }

            InnerLogicTick(OrderManager);
            if (worldRenderer != null && OrderManager.World != worldRenderer.World)
                InnerLogicTick(worldRenderer.World.OrderManager);
        }

        static void InnerLogicTick(OrderManager orderManager)
        {
            //var world = orderManager.World;

            //if (world == null)
            //    return;

            var isNetTick = LocalTick % NetTickScale == 0;

            if (!isNetTick || orderManager.IsReadyForNextFrame)
            {
                ++orderManager.LocalFrameNumber;

                if (isNetTick) //网络循环
                {
                    orderManager.Tick();
                }

                //Sync.CheckSyncUnchanged(world, () =>
                //{
                //    world.OrderGenerator.Tick(world);
                //    world.Selection.Tick(world);
                //});

                //world.Tick();
            }

            //if (orderManager.LocalFrameNumber > 0)
            //    Sync.CheckSyncUnchanged(world, () => world.TickRender(worldRenderer));
        }


        public static bool TakeScreenshot = false;
        static void RenderTick()
        {
            using (new PerfSample("render"))
            {
                ++RenderFrame;

                // worldRenderer is null during the initial install/download screen
                if (worldRenderer != null)
                {
                    //Renderer.BeginFrame(worldRenderer.Viewport.TopLeft, worldRenderer.Viewport.Zoom);
                    //Sound.SetListenerPosition(worldRenderer.Viewport.CenterPosition);
                    worldRenderer.Draw();
                }
                else
                {
                    //Renderer.BeginFrame(int2.Zero, 1f);
                }
                

                //using (new PerfSample("render_widgets"))
                //{
                //    Renderer.WorldModelRenderer.BeginFrame();
                //    //Ui.PrepareRenderables();
                //    Renderer.WorldModelRenderer.EndFrame();

                //    Ui.Draw();

                //    if (ModData != null && ModData.CursorProvider != null)
                //    {
                //        Cursor.SetCursor(Ui.Root.GetCursorOuter(Viewport.LastMousePos) ?? "default");
                //        Cursor.Render(Renderer);
                //    }
                //}

                //using (new PerfSample("render_flip"))
                //    Renderer.EndFrame(new DefaultInputHandler(OrderManager.World));

                if (TakeScreenshot)
                {
                    TakeScreenshot = false;
                    //TakeScreenshotInner();//截屏，Unity有自己的实现
                }
            }

            PerfHistory.Items["render"].Tick();
            PerfHistory.Items["batches"].Tick();
            PerfHistory.Items["render_widgets"].Tick();
            PerfHistory.Items["render_flip"].Tick();

            if (BenchmarkMode)
                Log.Write("render", "{0};{1}".F(RenderFrame, PerfHistory.Items["render"].LastValue));
        }

        public static T CreateObject<T>(string name)
        {
            return ModData.ObjectCreator.CreateObject<T>(name);
        }


        public static void LoadShellMap()
        {
            var shellmap = ChooseShellmap();

            using (new PerfTimer("StartGame"))
                StartGame(shellmap, WorldType.Shellmap);
        }

        public static bool HasFlag(MapVisibility sourceFlag, MapVisibility destinationFlag)
        {
            int sourceVal = (int)sourceFlag;
            int desVal = (int)destinationFlag;

            return (sourceVal & desVal) == desVal;
        }

        static string ChooseShellmap()
        {
            var shellmaps = ModData.MapCache
                .Where(m => m.Status == MapStatus.Available && HasFlag(m.Visibility,MapVisibility.Shellmap))
                .Select(m => m.Uid);

            if (!shellmaps.Any())
                throw new Exception("No valid shellmaps available");

            return shellmaps.Random(CosmeticRandom);
        }
        
        public static void RemoteDirectConnect(string host, int port)
        {
            OnRemoteDirectConnect(host, port);
        }
    }
}
