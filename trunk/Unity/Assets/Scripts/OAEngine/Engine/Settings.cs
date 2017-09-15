using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Server;

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
            private set { listenPort = value; }
            get { return this.listenPort; }
        }

        private int externalPort = 1234;

        public int ExternalPort
        {
            private set { this.externalPort = value; }
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
            private set { this.password = value; }
            get { return this.password; }
        }


        private bool discoverNatDevices = false;

        public bool DiscoverNatDevices
        {
            private set { this.discoverNatDevices = value; }
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
            private set { this.natDiscoveryTimeout = value; }
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
            private set { this.ban = value; }
            get { return this.ban; }
        }

        private bool enableSingleplayer = false;

        public bool EnableSingleplayer
        {
            private set { this.enableSingleplayer = value; }
            get { return this.enableSingleplayer; }
        }


        private bool queryMapRepository = true;

        public bool QueryMapRepository
        {
            private set { this.queryMapRepository = value; }
            get { return this.queryMapRepository; }
        }

        private string timestampFormat = "s";

        public string TimestampFormat
        {
            private set { this.timestampFormat = value; }
            get { return this.timestampFormat; }
        }

        public ServerSettings Clone()
        {
            return (ServerSettings)MemberwiseClone();
        }
    }

    public class PlayerSettings
    {
        public string Name = "Newbie";
        public string LastServer = "localhost:1234";
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

    public class Settings
    {
        public readonly ServerSettings Server = new ServerSettings();
        public readonly DebugSettings Debug = new DebugSettings();
        public readonly PlayerSettings Player = new PlayerSettings();

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
