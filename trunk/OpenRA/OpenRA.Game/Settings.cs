﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenRA.Traits;

namespace OpenRA
{
    public class ServerSettings
    {
        [Desc("Sets the server name.")]
        public string Name = "OpenRA Game";

        [Desc("Sets the internal port.")]
        public int ListenPort = 1234;

        [Desc("Sets the port advertised to the master server.")]
        public int ExternalPort = 1234;

        [Desc("Reports the game to the master server list.")]
        public bool AdvertiseOnline = true;

        [Desc("Locks the game with a password.")]
        public string Password = "";

        [Desc("Allow users to enable NAT discovery for external IP detection and automatic port forwarding.")]
        public bool DiscoverNatDevices = false;

        [Desc("Set this to false to disable UPnP even if compatible devices are found.")]
        public bool AllowPortForward = true;

        [Desc("Time in milliseconds to search for UPnP enabled NAT devices.")]
        public int NatDiscoveryTimeout = 1000;

        [Desc("Starts the game with a default map. Input as hash that can be obtained by the utility.")]
        public string Map = null;

        [Desc("Takes a comma separated list of IP addresses that are not allowed to join.")]
        public string[] Ban = { };

        [Desc("For dedicated servers only, controls whether a game can be started with just one human player in the lobby.")]
        public bool EnableSingleplayer = false;

        [Desc("Query map information from the Resource Center if they are not available locally.")]
        public bool QueryMapRepository = true;

        public string TimestampFormat = "s";

        public ServerSettings Clone()
        {
            return (ServerSettings)MemberwiseClone();
        }
    }

    public class DebugSettings
    {
        public bool BotDebug = true;
        public bool LuaDebug = true;
        public bool PerfText = true;
        public bool PerfGraph = true;
        public float LongTickThresholdMs = 1;
        public bool SanityCheckUnsyncedCode = false;
        public int Samples = 25;
        public bool IgnoreVersionMismatch = false;
        public bool StrictActivityChecking = false;
        public bool SendSystemInformation = true;
        public int SystemInformationVersionPrompt = 0;
        public string UUID = System.Guid.NewGuid().ToString();
    }

    public enum WindowMode
    {
        Windowed,
        Fullscreen,
        PseudoFullscreen,
    }

    public class GraphicSettings
    {
        [Desc("This can be set to Windowed, Fullscreen or PseudoFullscreen.")]
        public WindowMode Mode = WindowMode.Windowed;

        [Desc("Screen resolution in fullscreen mode.")]
        public int2 FullscreenSize = new int2(0, 0);

        [Desc("Screen resolution in windowed mode.")]
        public int2 WindowedSize = new int2(1024, 768);

        public bool HardwareCursors = true;

        public bool PixelDouble = false;
        public bool CursorDouble = false;

        [Desc("Add a frame rate limiter. It is recommended to not disable this.")]
        public bool CapFramerate = true;

        [Desc("At which frames per second to cap the framerate.")]
        public int MaxFramerate = 60;

        [Desc("Disable high resolution DPI scaling on Windows operating systems.")]
        public bool DisableWindowsDPIScaling = true;

        public int BatchSize = 8192;
        public int SheetSize = 2048;

        public string Language = "english";
        public string DefaultLanguage = "english";
        
    }

    public class SoundSettings
    {
        public float SoundVolume = 0.5f;
        public float MusicVolume = 0.5f;
        public float VideoVolume = 0.5f;

        public bool Shuffle = false;
        public bool Repeat = false;

        public string Device = null;

        public bool CashTicks = true;
        public bool Mute = false;
    }

    public class PlayerSettings
    {
        public string Name = "Newbie";
        //public HSLColor Color = new HSLColor(75, 255, 180);
        public string LastServer = "localhost:1234";
    }

    public enum MouseScrollType { Disabled, Standard, Inverted, Joystick }
    public enum StatusBarsType { Standard, DamageShow, AlwaysShow }

    public enum MPGameFilters
    {
        None = 0,
        Waiting = 1,
        Empty = 2,
        Protected = 4,
        Started = 8,
        Incompatible = 16
    }

    public class GameSettings
    {
        public string Platform = "Default";

        public bool ViewportEdgeScroll = true;
        public int ViewportEdgeScrollMargin = 5;

        public bool LockMouseWindow = false;
        public MouseScrollType MiddleMouseScroll = MouseScrollType.Standard;
        public MouseScrollType RightMouseScroll = MouseScrollType.Disabled;
        //public MouseButtonPreference MouseButtonPreference = new MouseButtonPreference();
        public float ViewportEdgeScrollStep = 10f;
        public float UIScrollSpeed = 50f;
        public int SelectionDeadzone = 24;
        public int MouseScrollDeadzone = 8;

        public bool UseClassicMouseStyle = false;
        public StatusBarsType StatusBars = StatusBarsType.Standard;
        public bool UsePlayerStanceColors = false;
        public bool DrawTargetLine = true;

        public bool AllowDownloading = true;

        public bool AllowZoom = true;
        public Modifiers ZoomModifier = Modifiers.Ctrl;

        public bool FetchNews = true;

        public MPGameFilters MPGameFilters = MPGameFilters.Waiting | MPGameFilters.Empty | MPGameFilters.Protected | MPGameFilters.Started;
    }


    public class ChatSettings
    {
        public string Hostname = "irc.openra.net";
        public int Port = 6667;
        public string Channel = "lobby";
        public string QuitMessage = "Battle control terminated!";
        public string TimestampFormat = "HH:mm";
        public bool ConnectAutomatically = false;
    }

    public class Settings
    {
        readonly string settingsFile;

        public readonly PlayerSettings Player = new PlayerSettings();
        public readonly GameSettings Game = new GameSettings();
        public readonly SoundSettings Sound = new SoundSettings();
        public readonly GraphicSettings Graphics = new GraphicSettings();
        public readonly ServerSettings Server = new ServerSettings();
        public readonly DebugSettings Debug = new DebugSettings();
        //public readonly KeySettings Keys = new KeySettings();
        public readonly ChatSettings Chat = new ChatSettings();

        public readonly Dictionary<string, object> Sections;

        // A direct clone of the file loaded from disk.
        // Any changed settings will be merged over this on save,
        // allowing us to persist any unknown configuration keys
        readonly List<MiniYamlNode> yamlCache = new List<MiniYamlNode>();

        public Settings(string file, Arguments args)
        {
            settingsFile = file;
            Sections = new Dictionary<string, object>()
            {
                { "Player", Player },
                { "Game", Game },
                { "Sound", Sound },
                { "Graphics", Graphics },
                { "Server", Server },
                { "Debug", Debug },
                //{ "Keys", Keys },
                { "Chat", Chat }
            };

            // Override fieldloader to ignore invalid entries
            var err1 = FieldLoader.UnknownFieldAction;
            var err2 = FieldLoader.InvalidValueAction;
            try
            {
                FieldLoader.UnknownFieldAction = (s, f) => Console.WriteLine("Ignoring unknown field `{0}` on `{1}`".F(s, f.Name));

                if (File.Exists(settingsFile))
                {
                    yamlCache = MiniYaml.FromFile(settingsFile);
                    foreach (var yamlSection in yamlCache)
                    {
                        object settingsSection;
                        if (Sections.TryGetValue(yamlSection.Key, out settingsSection))
                            LoadSectionYaml(yamlSection.Value, settingsSection);
                    }
                }

                // Override with commandline args
                foreach (var kv in Sections)
                    foreach (var f in kv.Value.GetType().GetFields())
                        if (args.Contains(kv.Key + "." + f.Name))
                            FieldLoader.LoadField(kv.Value, f.Name, args.GetValue(kv.Key + "." + f.Name, ""));
            }
            finally
            {
                FieldLoader.UnknownFieldAction = err1;
                FieldLoader.InvalidValueAction = err2;
            }
        }

        static string SanitizedName(string dirty)
        {
            if (string.IsNullOrEmpty(dirty))
                return null;

            var clean = dirty;

            // reserved characters for MiniYAML and JSON
            var disallowedChars = new char[] { '#', '@', ':', '\n', '\t', '[', ']', '{', '}', '"', '`' };
            foreach (var disallowedChar in disallowedChars)
                clean = clean.Replace(disallowedChar.ToString(), string.Empty);

            return clean;
        }

        public static string SanitizedServerName(string dirty)
        {
            var clean = SanitizedName(dirty);
            if (IsNullOrWhiteSpace(clean))
                return new ServerSettings().Name;
            else
                return clean;
        }

        public static bool IsNullOrWhiteSpace(String value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }


        public static string SanitizedPlayerName(string dirty)
        {
            var forbiddenNames = new string[] { "Open", "Closed" };
            var botNames = OpenRA.Game.ModData.DefaultRules.Actors["player"].TraitInfos<IBotInfo>().Select(t => t.Name);

            var clean = SanitizedName(dirty);

            if (IsNullOrWhiteSpace(clean) || forbiddenNames.Contains(clean) || botNames.Contains(clean))
                clean = new PlayerSettings().Name;

            // avoid UI glitches
            if (clean.Length > 16)
                clean = clean.Substring(0, 16);

            return clean;
        }

        public void Save()
        {
            foreach (var kv in Sections)
            {
                var sectionYaml = yamlCache.FirstOrDefault(x => x.Key == kv.Key);
                if (sectionYaml == null)
                {
                    sectionYaml = new MiniYamlNode(kv.Key, new MiniYaml(""));
                    yamlCache.Add(sectionYaml);
                }

                var defaultValues = Activator.CreateInstance(kv.Value.GetType());
                var fields = FieldLoader.GetTypeLoadInfo(kv.Value.GetType());
                foreach (var fli in fields)
                {
                    var serialized = FieldSaver.FormatValue(kv.Value, fli.Field);
                    var defaultSerialized = FieldSaver.FormatValue(defaultValues, fli.Field);

                    // Fields with their default value are not saved in the settings yaml
                    // Make sure that we erase any previously defined custom values
                    if (serialized == defaultSerialized)
                        sectionYaml.Value.Nodes.RemoveAll(n => n.Key == fli.YamlName);
                    else
                    {
                        // Update or add the custom value
                        var fieldYaml = sectionYaml.Value.Nodes.FirstOrDefault(n => n.Key == fli.YamlName);
                        if (fieldYaml != null)
                            fieldYaml.Value.Value = serialized;
                        else
                            sectionYaml.Value.Nodes.Add(new MiniYamlNode(fli.YamlName, new MiniYaml(serialized)));
                    }
                }
            }

            yamlCache.WriteToFile(settingsFile);
        }


        static void LoadSectionYaml(MiniYaml yaml, object section)
        {
            var defaults = Activator.CreateInstance(section.GetType());
            FieldLoader.InvalidValueAction = (s, t, f) =>
            {
                var ret = defaults.GetType().GetField(f).GetValue(defaults);
                Console.WriteLine("FieldLoader: Cannot parse `{0}` into `{2}:{1}`; substituting default `{3}`".F(s, t.Name, f, ret));
                return ret;
            };

            FieldLoader.Load(section, yaml);
        }
    }
}