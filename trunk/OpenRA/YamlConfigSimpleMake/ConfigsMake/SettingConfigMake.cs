using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine;
using YamlDotNet.Serialization;

namespace YamlConfigSimpleMake.ConfigsMake
{
    public static class SettingConfigMake
    {
        public static void Make()
        {
            Settings setting = new Settings();

            ServerSettings server = new ServerSettings();
            setting.Server = server;
            server.AllowPortForward = false;


            Engine.PlayerSettings player = new Engine.PlayerSettings();
            setting.Player = player;

            DebugSettings debug = new DebugSettings();
            setting.Debug = debug;
            debug.BotDebug = true;
            debug.LuaDebug = true;
            debug.PerfText = true;
            debug.PerfGraph = true;
            debug.SanityCheckUnsyncedCode = false;
            debug.EnableDebugCommandsInReplays = true;

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(setting);

            
            string contentsDir = Platform.SupportDir;
            if (!Directory.Exists(contentsDir))
            {
                Directory.CreateDirectory(contentsDir);
            }
            string settingPath = Platform.ResolvePath("^settings.yaml");

            using (StreamWriter sw = new StreamWriter(File.OpenWrite(settingPath), Encoding.UTF8))
            {
                sw.Write(yaml);
            }
            Console.WriteLine("Test write setting config successful!");
        }
    }
}
