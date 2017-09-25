using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Engine;
using YamlDotNet.Serialization;
using Engine.Maps;
using OAUnityLayer;
using Server;

namespace YamlConfigSimpleMake.ConfigsMake
{
    public static class MapConfigMake
    {
        public static void Make()
        {
            Map map1 = new Map();

            List<PlayerReference> players = new List<PlayerReference>();
            map1.Players = players;
            players.Add(new PlayerReference
            {
                Name = "France",
                Faction = "allies",
                Allies = new string[] { "Germany" },
                Enemies = new string[] { "USSR" },
            });
            players.Add(new PlayerReference
            {
                Name = "Germany",
                Faction = "allies",
                Allies = new string[] { "France" },
                NonCombatant = true,
                Enemies = new string[] { "USSR" },
            });
            players.Add(new PlayerReference
            {
                Name = "USSR",
                Playable = true,
                AllowBots = false,
                Required = true,
                LockFaction = true,
                LockSpawn = true,
                LockTeam = true,
                Faction = "soviet",
                Enemies = new string[] { "France", "Germany" },
            });
            players.Add(new PlayerReference
            {
                Name = "Neutral",
                OwnsWorld = true,
                NonCombatant = true,
                Faction = "allies"
            });


            List<ActorReference> actors = new List<ActorReference>();
            map1.Actors = actors;
            actors.Add(new ActorReference
            {
                ActorTypeName = "wood",
                InitInfo = new ActorInitInfo()
                {
                    Location = new LocationInit()
                    {
                        X = -30,
                        Y = 0,
                    },
                    Owner = new OwnerInit()
                    {
                        PlayerName = "Germany",
                    }
                },

            });
            actors.Add(new ActorReference
            {
                ActorTypeName = "wood",
                InitInfo = new ActorInitInfo()
                {
                    Location = new LocationInit()
                    {
                        X = -15,
                        Y = 0,
                    },
                    Owner = new OwnerInit()
                    {
                        PlayerName = "USSR",
                    }
                },

            });
            actors.Add(new ActorReference
            {
                ActorTypeName = "wood",
                InitInfo = new ActorInitInfo()
                {
                    Location = new LocationInit()
                    {
                        X = 15,
                        Y = 0,
                    },
                    Owner = new OwnerInit()
                    {
                        PlayerName = "Neutral",
                    }
                },

            });
            actors.Add(new ActorReference
            {
                ActorTypeName = "wood",
                InitInfo = new ActorInitInfo()
                {
                    Location = new LocationInit()
                    {
                        X = 30,
                        Y = 0,
                    },
                    Owner = new OwnerInit()
                    {
                        PlayerName = "France",
                    }
                },

            });

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(map1);

            string mapDir = Platform.ModsDir + Path.DirectorySeparatorChar + "ra" + Path.DirectorySeparatorChar +
                           Path.Combine("maps", "map1");

            if (!Directory.Exists(mapDir))
            {
                Directory.CreateDirectory(mapDir);
            }

            string mapPath = mapDir + Path.DirectorySeparatorChar + "map.yaml";
            
            using (StreamWriter sw = new StreamWriter(File.OpenWrite(mapPath), Encoding.UTF8))
            {
                sw.Write(yaml);
            }
            Console.WriteLine("Test write map config successful!");
            
        }
    }
}
