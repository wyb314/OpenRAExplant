using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Server;
using TrueSync;

namespace Engine
{
    public class ServerSettings
    {
        private string name = "OpenRA Game";
        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        private int listenPort = 1234;

        public int ListenPort
        {
            set { listenPort = value; }
            get { return this.listenPort; }
        }

        private int externalPort = 1234;

        public int ExternalPort
        {
            set { this.externalPort = value; }
            get { return this.externalPort; }
        }

        private bool advertiseOnline = true;

        public bool AdvertiseOnline
        {
            set { this.advertiseOnline = value; }
            get { return this.advertiseOnline; }
        }

        private string password = "";

        public string Password
        {
            set { this.password = value; }
            get { return this.password; }
        }


        private bool discoverNatDevices = false;

        public bool DiscoverNatDevices
        {
            set { this.discoverNatDevices = value; }
            get { return this.discoverNatDevices; }
        }

        private bool allowPortForward = true;

        public bool AllowPortForward
        {
            set { this.allowPortForward = value; }
            get { return this.allowPortForward; }
        }

        private int natDiscoveryTimeout = 1000;

        public int NatDiscoveryTimeout
        {
            set { this.natDiscoveryTimeout = value; }
            get { return this.natDiscoveryTimeout; }
        }

        private string map = null;

        public string Map
        {
            set { this.map = value; }
            get { return this.map; }
        }

        private string[] ban = null;

        public string[] Ban
        {
            set { this.ban = value; }
            get { return this.ban; }
        }
        
        public bool EnableSingleplayer { set; get; }


        private bool queryMapRepository = true;

        public bool QueryMapRepository
        {
            set { this.queryMapRepository = value; }
            get { return this.queryMapRepository; }
        }

        private string timestampFormat = "s";

        public string TimestampFormat
        {
            set { this.timestampFormat = value; }
            get { return this.timestampFormat; }
        }

        public ServerSettings Clone()
        {
            return (ServerSettings)MemberwiseClone();
        }
    }

    public class PlayerSettings
    {
        private string name = "Newbie";
        public string Name
        {
            set { this.name = value; }
            get { return this.name; }
        }

        private string lastServer = "localhost:1234";

        public string LastServer
        {
            set { this.lastServer = value; }
            get { return this.lastServer; }
        }
    }

    public class DebugSettings
    {
        public bool BotDebug { set; get; }
        public bool LuaDebug { set; get; }
        public bool PerfText { set; get; }
        public bool PerfGraph { set; get; }

        public FP LongTickThresholdMs = 1;
        public bool SanityCheckUnsyncedCode { set; get; }
        public int Samples { set; get; }
        public bool IgnoreVersionMismatch { set; get; }
        public bool StrictActivityChecking { set; get; }

        private bool sendSystemInformation = true;

        public bool SendSystemInformation
        {
            set { this.sendSystemInformation = value; }
            get { return this.sendSystemInformation; }
        }
        public int SystemInformationVersionPrompt { set; get; }

        public bool EnableDebugCommandsInReplays { set; get; }

        public string UUID = "";

        public DebugSettings()
        {
            this.UUID = System.Guid.NewGuid().ToString();
        }
    }

    public class Settings
    {
        private ServerSettings server = new ServerSettings();
        public ServerSettings Server
        {
            set { this.server = value; }
            get { return this.server; }
        }

        private DebugSettings debug = new DebugSettings();

        public DebugSettings Debug
        {
            set { this.debug = value; }
            get { return this.debug; }
        }

        private PlayerSettings player = new PlayerSettings();

        public PlayerSettings Player {
            set { this.player = value; }
            get { return this.player; }
        }

        public static string SanitizedServerName(string dirty)
        {
            var clean = SanitizedName(dirty);
            if (IsNullOrWhiteSpace(clean))
                return new ServerSettings().Name;
            else
                return clean;
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
            //var botNames = OpenRA.Game.ModData.DefaultRules.Actors["player"].TraitInfos<IBotInfo>().Select(t => t.Name);

            var clean = SanitizedName(dirty);

            //if (IsNullOrWhiteSpace(clean) || forbiddenNames.Contains(clean) || botNames.Contains(clean))
            //    clean = new PlayerSettings().Name;
            if (IsNullOrWhiteSpace(clean) || forbiddenNames.Contains(clean))
                clean = new PlayerSettings().Name;

            // avoid UI glitches
            if (clean.Length > 16)
                clean = clean.Substring(0, 16);

            return clean;
        }
    }
}
