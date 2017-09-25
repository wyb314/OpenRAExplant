using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine;
using Engine.Network.Defaults.ServerTraits;
using YamlDotNet.Serialization;

namespace YamlConfigSimpleMake.ConfigsMake
{
    public static class ManifestConfigMake
    {
        public static void Make()
        {
            Manifest manifest = new Manifest();

            ModMetadata metaData = new ModMetadata();
            metaData.Title = "Red Alert";
            metaData.Version = "{DEV_VERSION}";
            manifest.Metadata = metaData;


            manifest.ServerTraits = new string[]
            {
            typeof(LobbyCommands).FullName,
            typeof(LobbySettingsNotification).FullName,
            typeof(MasterServerPinger).FullName,
            typeof(PlayerPinger).FullName
            };

            string modDir = Platform.ModsDir + Path.DirectorySeparatorChar + "ra";
            if (!Directory.Exists(modDir))
            {
                Directory.CreateDirectory(modDir);
            }
            string modPath = modDir + Path.DirectorySeparatorChar + "mod.yaml";
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(manifest);

            using (StreamWriter sw = new StreamWriter(File.OpenWrite(modPath), Encoding.UTF8))
            {
                sw.Write(yaml);
            }

            Console.WriteLine("Test write manifest config successful!");
        }
    }
}
