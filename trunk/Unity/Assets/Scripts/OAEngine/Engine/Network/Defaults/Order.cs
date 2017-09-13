

using System.IO;
using Engine.Network.Interfaces;
using Engine.Support;
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
        ExtraData = 0x20,
        ExtDatas = 0x40
    }

    static class OrderFieldsExts
    {
        public static bool HasField(this OrderFields of, OrderFields f)
        {
            return (of & f) != 0;
        }
    }

    public sealed class Order : IOrder
    {

        public bool IsImmediate { set; get; }

        public string OrderString { private set; get; }
        
        public string TargetString { set; get; }

        public uint ExtraData;

        public byte[] ExtDatas;

        public Order(bool isImmediate, string orderString,string targetString,uint extraData = 0, byte[] extDatas = null)
        {
            this.IsImmediate = isImmediate;
            this.OrderString = orderString;
            this.TargetString = targetString;
            this.ExtraData = extraData;
            this.ExtDatas = extDatas;
        }

        public static Order Command(string text)
        {
            return new Order(true, "Command", text);
        }

        public static Order Pong(string pingTime)
        {
            return new Order(true,"Pong", pingTime);
        }

        public static Order Deserialize(INetWorld world, BinaryReader r)
        {
            var magic = r.ReadByte();
            switch (magic)
            {
                case 0xFF:
                    {
                        var order = r.ReadString();
                        var subjectId = r.ReadUInt32();
                        var flags = (OrderFields)r.ReadByte();

                        //var targetActorId = flags.HasField(OrderFields.TargetActor) ? r.ReadUInt32() : uint.MaxValue;
                        //var targetLocation = (CPos)(flags.HasField(OrderFields.TargetLocation) ? r.ReadInt2() : int2.Zero);
                        var targetString = flags.HasField(OrderFields.TargetString) ? r.ReadString() : null;
                        //var queued = flags.HasField(OrderFields.Queued);
                        //var extraLocation = (CPos)(flags.HasField(OrderFields.ExtraLocation) ? r.ReadInt2() : int2.Zero);
                        var extraData = flags.HasField(OrderFields.ExtraData) ? r.ReadUInt32() : 0;
                        var extDatas = flags.HasField(OrderFields.ExtDatas) ? r.ReadBytes(r.ReadInt32()) : null;


                        if (world == null)
                            return new Order(false,order, targetString,extraData, extDatas);

                        //Actor subject, targetActor;
                        //if (!TryGetActorFromUInt(world, subjectId, out subject) || !TryGetActorFromUInt(world, targetActorId, out targetActor))
                        //    return null;

                        return new Order(false, order, targetString, extraData, extDatas);
                    }

                case 0xfe:
                    {
                        var name = r.ReadString();
                        var data = r.ReadString();

                        return new Order(true, name, data);
                    }

                default:
                    {
                        Log.Write("debug", "Received unknown order with magic {0}", magic);
                        return null;
                    }
            }
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
                        if (ExtraData != 0) fields |= OrderFields.ExtraData;
                        if (ExtDatas != null) fields |= OrderFields.ExtDatas;

                        w.Write((byte)fields);

                        //if (TargetActor != null)
                        //    w.Write(UIntFromActor(TargetActor));
                        //if (TargetLocation != CPos.Zero)
                        //    w.Write(TargetLocation);
                        if (TargetString != null)
                            w.Write(TargetString);
                        //if (ExtraLocation != CPos.Zero)
                        //    w.Write(ExtraLocation);
                        if (ExtraData != 0)
                            w.Write(ExtraData);
                        if (ExtDatas != null)
                        {
                            w.Write(ExtDatas.Length);
                            w.Write(ExtDatas);
                        }
                       

                        return ret.ToArray();
                    }
            }
        }

    }
}
