using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Engine;
using Engine.FileSystem;
using Engine.Network.Defaults;
using Engine.Network.Enums;
using Engine.Server.Logs;
using Engine.Support;
using Server;

namespace Engine.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var arguments = new Arguments(args);
            ServerPlatformInfo platformInfo = new ServerPlatformInfo();
            platformInfo.GatherInfomation();
            platformInfo.SetLogger(new ServerLogger());

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
            
            // Special case handling of Game.Mod argument: if it matches a real filesystem path
            // then we use this to override the mod search path, and replace it with the mod id
            var modID = arguments.GetValue("Game.Mod", null);
           

            if (modID == null)
                throw new InvalidOperationException("Game.Mod argument missing or mod could not be found.");

            // HACK: The engine code assumes that Game.Settings is set.
            // This isn't nearly as bad as ModData, but is still not very nice.
            Game.InitializeSettings(arguments);
            var settings = Game.Settings.Server;

            string maniPath = Platform.ModsDir+Path.DirectorySeparatorChar + modID + Path.DirectorySeparatorChar+"mod.yaml";
            Manifest manifest = YamlHelper.Deserialize<Manifest>(maniPath);
            manifest.Id = modID;
            // HACK: The engine code *still* assumes that Game.ModData is set
            var modData = Game.ModData = new ModData(manifest);
            //modData.MapCache.LoadMaps();

            settings.Map = "wyb";

            Console.WriteLine("[{0}] Starting dedicated server for mod: {1}", DateTime.Now.ToString(settings.TimestampFormat), modID);
            while (true)
            {
                var server = new ServerDefault(new IPEndPoint(IPAddress.Any, settings.ListenPort), settings, modData, true);

                while (true)
                {
                    Thread.Sleep(1000);
                    if (server.State == ServerState.GameStarted && server.Conns.Count < 1)
                    {
                        Console.WriteLine("[{0}] No one is playing, shutting down...", DateTime.Now.ToString(settings.TimestampFormat));
                        server.Shutdown();
                        break;
                    }
                }

                Console.WriteLine("[{0}] Starting a new server instance...", DateTime.Now.ToString(settings.TimestampFormat));
            }
        }
    }
}
