using System;
using System.Collections.Generic;
using System.IO;
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
            byte[] bytes = null;
            using (var ret = new MemoryStream())
            {
                var w = new BinaryWriter(ret);
                w.Write(this.Index);
                w.Write(this.Latency);
                int latencyHistoryCount = this.LatencyHistory.Length;
                w.Write(latencyHistoryCount);
                for (int i = 0; i < latencyHistoryCount; i++)
                {
                    w.Write(this.LatencyHistory[i]);
                }
                w.Write(this.LatencyJitter);
                bytes = ret.ToArray();
            }
            return null;
        }


        public static ClientPingDefault Deserialize(byte[] data)
        {
            ClientPingDefault clientPing = new ClientPingDefault();
            using (var ret = new MemoryStream(data))
            {
                var r = new BinaryReader(ret);
                clientPing.Index = r.ReadInt32();
                clientPing.Latency = r.ReadInt64();
                int latencyHistoryCount = r.ReadInt32();
                long[] LatencyHistory = new long[latencyHistoryCount];
                clientPing.LatencyHistory = LatencyHistory;
                for (int i= 0; i < latencyHistoryCount; i++)
                {
                    LatencyHistory[i] = r.ReadInt64();
                }
                clientPing.LatencyJitter = r.ReadInt64();
            }
            return clientPing;
        }

    }
}
