using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Engine.Primitives;
using Engine.Support;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine;
using Engine.Interfaces;

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
        public const int Timestep = 40;
        public const int TimestepJankThreshold = 250; // Don't catch up for delays larger than 250ms

        public static bool BenchmarkMode = false;

        public static event Action LobbyInfoChanged = () => { };
        public static event Action<string, int> OnRemoteDirectConnect = (a, b) => { };
        public static event Action<IOrderManager> ConnectionStateChanged = _ => { };
        public static event Action BeforeGameStart = () => { };
        static volatile ActionQueue delayedActions = new ActionQueue();
        public static event Action OnQuit = () => { };

        static ConnectionState lastConnectionState = ConnectionState.PreConnecting;
        internal static IOrderManager OrderManager;

        static RunStatus state = RunStatus.Running;

        static IWorldRenderer worldRenderer;

        public static Settings Settings;

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

            LobbyInfoChanged = () => { };
            ConnectionStateChanged = om => { };
            BeforeGameStart = () => { };
            OnRemoteDirectConnect = (a, b) => { };
            delayedActions = new ActionQueue();

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
            
            Platform.platformInfo.Tick = Loop;
            Platform.platformInfo.OnApplicationQuit = OnApplicationQuit;
            
            return state;
        }

        public static int LocalTick { get { return OrderManager.LocalFrameNumber; } }

        static void Loop(float elapsedTime)
        {
            IWorld world = OrderManager.World as IWorld;

            if (world == null)
                return;

            var isNetTick = LocalTick % NetTickScale == 0;

            if (!isNetTick || OrderManager.IsReadyForNextFrame)
            {
                ++OrderManager.LocalFrameNumber;

                Log.Write("debug", "--Tick: {0} ({1})", LocalTick, isNetTick ? "net" : "local");

                if (BenchmarkMode)
                    Log.Write("cpu", "{0};{1}".F(LocalTick, PerfHistory.Items["tick_time"].LastValue));

                if (isNetTick)
                    OrderManager.Tick();

                Sync.CheckSyncUnchanged(world, () =>
                {
                    //world.OrderGenerator.Tick(world);
                    //world.Selection.Tick(world);
                });

                world.Tick();

                PerfHistory.Tick();
            }
            else if (OrderManager.NetFrameNumber == 0)
            {
                //OrderManager.LastTickTime = RunTime;
            }
                

            // Wait until we have done our first world Tick before TickRendering
            if (OrderManager.LocalFrameNumber > 0)
            {
                Sync.CheckSyncUnchanged
                    (world, () => world.TickRender(worldRenderer));
            }
                
        }


        //static void InnerLogicTick(IOrderManager orderManager)
        //{
        //}


        static void OnApplicationQuit()
        {
            if (OrderManager != null)
            {
                OrderManager.Dispose();
            }
                
            OnQuit();
        }
    }
}
