

using System.IO;
using Engine.Network.Interfaces;
using UnityEditor;

namespace Engine.Network.Defaults
{
    enum OrderFields : byte
    {
        TargetActor = 0x01,
        TargetLocation = 0x02,
        TargetString = 0x04,
        Queued = 0x08,
        ExtraLocation = 0x10,
        ExtraData = 0x20
    }

    public sealed class Order : IOrder
    {

        public bool IsImmediate { set; get; }

        public string OrderString { private set; get; }
        
        public string TargetString { set; get; }

        public Order(bool isImmediate, string orderString,string targetString)
        {
            this.IsImmediate = isImmediate;
            this.OrderString = orderString;
            this.TargetString = targetString;
        }

        public static Order Command(string text)
        {
            return new Order(true, "Command", text);
        }

        public static Order Deserialize(INetWorld world, BinaryReader r)
        {
            return null;
        }

        public byte[] Serialize()
        {
            if (IsImmediate)
            {
                var ret = new MemoryStream();
                var w = new BinaryWriter(ret);
                w.Write((byte)0xfe);
                w.Write(OrderString);
                w.Write(TargetString);
                return ret.ToArray();
            }

            switch (OrderString)
            {
                /*
				 * Format:
				 * u8: orderID.
				 * 0xFF: Full serialized order.
				 * varies: rest of order.
				 */
                default:
                    // TODO: specific serializers for specific orders.
                    {
                        var ret = new MemoryStream();
                        var w = new BinaryWriter(ret);
                        w.Write((byte)0xFF);
                        w.Write(OrderString);
                        //w.Write(UIntFromActor(Subject));

                        OrderFields fields = 0;
                        //if (TargetActor != null) fields |= OrderFields.TargetActor;
                        //if (TargetLocation != CPos.Zero) fields |= OrderFields.TargetLocation;
                        if (TargetString != null) fields |= OrderFields.TargetString;
                        //if (Queued) fields |= OrderFields.Queued;
                        //if (ExtraLocation != CPos.Zero) fields |= OrderFields.ExtraLocation;
                        //if (ExtraData != 0) fields |= OrderFields.ExtraData;

                        w.Write((byte)fields);

                        //if (TargetActor != null)
                        //    w.Write(UIntFromActor(TargetActor));
                        //if (TargetLocation != CPos.Zero)
                        //    w.Write(TargetLocation);
                        if (TargetString != null)
                            w.Write(TargetString);
                        //if (ExtraLocation != CPos.Zero)
                        //    w.Write(ExtraLocation);
                        //if (ExtraData != 0)
                        //    w.Write(ExtraData);

                        return ret.ToArray();
                    }
            }
        }

    }
}
