using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Network.Enums;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class SlotDefault : ISlot
    {
        public bool AllowBots { set; get; }

        public bool Closed { set; get; }

        public bool LockColor { set; get; }

        public bool LockFaction { set; get; }

        public bool LockSpawn { set; get; }

        public bool LockTeam { set; get; }

        public string PlayerReference { set; get; }

        public bool Required { set; get; }

        public byte[] Serialize()
        {
            byte[] bytes = null;
            using (var ret = new MemoryStream())
            {
                var w = new BinaryWriter(ret);
                w.Write(this.AllowBots);
                w.Write(this.Closed);
                w.Write(this.LockColor);
                w.Write(this.LockFaction);
                w.Write(this.LockSpawn);
                w.Write(this.LockTeam);
                w.Write(this.PlayerReference);
                w.Write(this.Required);
                bytes = ret.ToArray();
            }
            return bytes;
        }

        public static SlotDefault Deserialize(byte[] data)
        {
            SlotDefault slot = new SlotDefault();
            using (var ret = new MemoryStream(data))
            {
                var r = new BinaryReader(ret);
                slot.AllowBots = r.ReadBoolean();
                slot.Closed = r.ReadBoolean();
                slot.LockColor = r.ReadBoolean();
                slot.LockFaction = r.ReadBoolean();
                slot.LockSpawn = r.ReadBoolean();
                slot.LockTeam = r.ReadBoolean();
                slot.PlayerReference = r.ReadString();
                slot.Required = r.ReadBoolean();
            }
            return slot;
        }
    }
}
