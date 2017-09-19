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

        private long[] latencyHistory = {};

        public long[] LatencyHistory
        {
            set
            {
                if (value == null)
                {
                    this.latencyHistory = new long[0];
                }
                else
                {
                    this.latencyHistory = value;
                }
            }
            get { return this.latencyHistory; }
        }

        public bool IsLatencyHistoryEmpty
        {
            get { return this.latencyHistory == null || this.latencyHistory.Length == 0; }
        }

        public long LatencyJitter { set; get; }

        public byte[] Serialize()
        {
            byte[] bytes = null;
            using (var ret = new MemoryStream())
            {
                var w = new BinaryWriter(ret);
                w.Write(this.Index);
                w.Write(this.Latency);
                int latencyHistoryCount = this.IsLatencyHistoryEmpty ? 0 : this.LatencyHistory.Length;
                w.Write(latencyHistoryCount);
                if (latencyHistoryCount > 0)
                {
                    for (int i = 0; i < latencyHistoryCount; i++)
                    {
                        w.Write(LatencyHistory[i]);
                    }
                }
                w.Write(this.LatencyJitter);
                bytes = ret.ToArray();
            }
            return bytes;
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
                if (latencyHistoryCount > 0)
                {
                    long[] LatencyHistory = new long[latencyHistoryCount];
                    clientPing.LatencyHistory = LatencyHistory;
                    for (int i = 0; i < latencyHistoryCount; i++)
                    {
                        LatencyHistory[i] = r.ReadInt64();
                    }
                }
                
                clientPing.LatencyJitter = r.ReadInt64();
            }
            return clientPing;
        }

    }


    public static class ClientPingDefaultExts
    {
        public static byte[] WriteToBytes(this List<ClientPingDefault> y)
        {
            byte[] bytes = null;
            using (var mr = new MemoryStream())
            {
                var w = new BinaryWriter(mr);
                int count = y.Count;
                w.Write(count);
                foreach (var clientPing in y)
                {
                    byte[] clientBytes = clientPing.Serialize();
                    w.Write(clientBytes.Length);
                    w.Write(clientBytes);
                }
                bytes = mr.ToArray();
            }

            return bytes;
        }

        public static List<ClientPingDefault> ReadToClientPings(this byte[] bytes)
        {
            List<ClientPingDefault> clientPings = null;
            using (var mr = new MemoryStream(bytes))
            {
                var br = new BinaryReader(mr);
                int count = br.ReadInt32();
                clientPings = new List<ClientPingDefault>(count);

                for (int i = 0; i < count; i++)
                {
                    int clientPingBytesLength = br.ReadInt32();
                    ClientPingDefault client = ClientPingDefault.Deserialize(br.ReadBytes(clientPingBytesLength));
                    clientPings.Add(client);
                }
            }

            return clientPings;
        }
    }

}
