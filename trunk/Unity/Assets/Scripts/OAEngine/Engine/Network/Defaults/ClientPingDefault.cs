using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class ClientPingDefault : IClientPing
    {
        public int Index { set; get; }

        public long Latency { set; get; }

        public long[] LatencyHistory { set; get; }

        public long LatencyJitter { set; get; }

        public byte[] Serialize()
        {
            return null;
        }
    }
}
