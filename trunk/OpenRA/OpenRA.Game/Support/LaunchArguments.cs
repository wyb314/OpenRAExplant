using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA
{
    public class LaunchArguments
    {
        [Desc("Connect to the following server given as IP:PORT on startup.")]
        public string Connect;

        [Desc("Connect to the unified resource identifier openra://IP:PORT on startup.")]
        public string URI;

        [Desc("Automatically start playing the given replay file.")]
        public string Replay;

        [Desc("Dump performance data into cpu.csv and render.csv in the logs folder.")]
        public bool Benchmark;

        public LaunchArguments(Arguments args)
        {
            if (args == null)
                return;

            foreach (var f in GetType().GetFields())
                if (args.Contains("Launch" + "." + f.Name))
                    FieldLoader.LoadField(this, f.Name, args.GetValue("Launch" + "." + f.Name, ""));
        }

        public string GetConnectAddress()
        {
            var connect = string.Empty;

            if (!string.IsNullOrEmpty(Connect))
                connect = Connect;

            if (!string.IsNullOrEmpty(URI))
                connect = URI.Substring(URI.IndexOf("://", System.StringComparison.Ordinal) + 3).TrimEnd('/');

            return connect;
        }
    }
}
