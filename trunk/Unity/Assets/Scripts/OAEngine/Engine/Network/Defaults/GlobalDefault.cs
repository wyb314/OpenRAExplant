

using System;
using System.IO;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public sealed class GlobalDefault : IGlobal
    {
        private bool allowSpectators = true;
        public bool AllowSpectators
        {
            get { return this.allowSpectators; }

            set { this.allowSpectators = value; }
        }
        
        public bool AllowVersionMismatch { set; get; }

        public bool EnableSingleplayer { set; get; }

        public string GameUid { set; get; }

        public string Map { set; get; }

        private int orderLatency = 3;
        public int OrderLatency
        {
            get { return this.orderLatency; }

            set { this.orderLatency = value; }
        }

        public int RandomSeed { set; get; }

        public string ServerName { set; get; }

        private int timeStep = 40;
        public int Timestep
        {
            get { return this.timeStep; }

            set { this.timeStep = value; }
        }

        public byte[] Serialize()
        {
            byte[] bytes = null;
            using (var ms = new MemoryStream())
            {
                var w = new BinaryWriter(ms);
                w.Write(this.allowSpectators);
                w.Write(this.AllowVersionMismatch);
                w.Write(this.EnableSingleplayer);
                w.Write(this.GameUid);
                w.Write(this.Map);
                w.Write(this.orderLatency);
                w.Write(this.RandomSeed);
                w.Write(this.ServerName);
                w.Write(this.timeStep);
                bytes = ms.ToArray();
            }
            return bytes;
        }

        public static GlobalDefault Deserialize(byte[] data)
        {
            GlobalDefault globalData = new GlobalDefault();
            using (var ret = new MemoryStream(data))
            {
                var r = new BinaryReader(ret);
                globalData.allowSpectators = r.ReadBoolean();
                globalData.AllowVersionMismatch = r.ReadBoolean();
                globalData.EnableSingleplayer = r.ReadBoolean();
                globalData.GameUid = r.ReadString();
                globalData.Map = r.ReadString();
                globalData.orderLatency = r.ReadInt32();
                globalData.RandomSeed = r.ReadInt32();
                globalData.ServerName = r.ReadString();
                globalData.timeStep = r.ReadInt32();
            }
            return globalData;
        }
    }
}
