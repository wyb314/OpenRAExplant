

using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Engine.Network.Interfaces;
using Engine.OrderGenerators;
using Engine.Support;

namespace Engine.Network.Defaults
{
    enum OrderFields : byte
    {
        TargetActor = 0x01,
        TargetLocation = 0x02,
        TargetString = 0x04,
        Queued = 0x08,
        OpCode = 0x10,
        OpData = 0x20,
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
        
        //public uint ExtraData;

        public byte[] ExtDatas { private set; get; }


        public byte OpCode { private set; get; }

        public byte OpData { private set; get; }

        public Order(bool isImmediate, string orderString, byte[] extDatas = null,byte opCode = byte.MaxValue,byte opData = byte.MaxValue)
        {
            this.IsImmediate = isImmediate;
            this.OrderString = orderString;
            this.ExtDatas = extDatas;
        }

        public static Order Command(string text)
        {
            return new Order(true, "Command", Encoding.UTF8.GetBytes(text));
        }

        public static Order Pong(string pingTime)
        {
            return new Order(true,"Pong", Encoding.UTF8.GetBytes(pingTime));
        }

        public static Order Pong(byte opCode , byte opData)
        {
            return new Order(true, "Controller", null,opCode,opData);
        }

        public static Order HandshakeResponse(byte[] bytes)
        {
            return new Order(true,"HandshakeResponse", bytes) ;
        }

        public static Order Deserialize(INetWorld world, BinaryReader r)
        {
            var magic = r.ReadByte();
            switch (magic)
            {
                case 0xFF:
                    {
                        var order = r.ReadString();
                        //var subjectId = r.ReadUInt32();
                        var flags = (OrderFields)r.ReadByte();

                        //var targetActorId = flags.HasField(OrderFields.TargetActor) ? r.ReadUInt32() : uint.MaxValue;
                        //var targetLocation = (CPos)(flags.HasField(OrderFields.TargetLocation) ? r.ReadInt2() : int2.Zero);
                        //var targetString = flags.HasField(OrderFields.TargetString) ? r.ReadString() : null;
                        //var queued = flags.HasField(OrderFields.Queued);
                        //var extraLocation = (CPos)(flags.HasField(OrderFields.ExtraLocation) ? r.ReadInt2() : int2.Zero);
                        //var extraData = flags.HasField(OrderFields.ExtraData) ? r.ReadUInt32() : 0;
                        
                        var extDatas = flags.HasField(OrderFields.ExtDatas) ? r.ReadBytes(r.ReadInt32()) : null;
                        byte opCode = flags.HasField(OrderFields.OpCode) ? r.ReadByte() : ControllerConst.NULL_OP_CODE;
                        byte opData = 0;
                        if (opCode != ControllerConst.NULL_OP_CODE)
                        {
                            opData = flags.HasField(OrderFields.OpData) ? r.ReadByte() : ControllerConst.NULL_OP_CODE;
                        }

                        if (world == null)
                            return new Order(false,order,extDatas,opCode, opData);

                        //Actor subject, targetActor;
                        //if (!TryGetActorFromUInt(world, subjectId, out subject) || !TryGetActorFromUInt(world, targetActorId, out targetActor))
                        //    return null;

                        return new Order(false, order, extDatas, opCode, opData);
                    }

                case 0xfe:
                    {
                        var name = r.ReadString();
                        int dataLength = r.ReadInt32();
                        
                        byte[] result = null;
                        if (dataLength > 0)
                        {
                            result = r.ReadBytes(dataLength);
                        }
                        return new Order(true, name,result);
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
                int count = this.ExtDatas == null ? 0 : this.ExtDatas.Length;
                w.Write(count);
                if (count > 0)
                {
                    w.Write(this.ExtDatas);
                }
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
                        //if (TargetString != null) fields |= OrderFields.TargetString;
                        //if (Queued) fields |= OrderFields.Queued;
                        //if (ExtraLocation != CPos.Zero) fields |= OrderFields.ExtraLocation;
                        //if (ExtraData != 0) fields |= OrderFields.ExtraData;
                        if (ExtDatas != null) fields |= OrderFields.ExtDatas;
                        if (OpCode != ControllerConst.NULL_OP_CODE)
                        {
                            fields |= OrderFields.OpCode;
                            fields |= OrderFields.OpData;
                        }
                       
                        w.Write((byte)fields);

                        //if (TargetActor != null)
                        //    w.Write(UIntFromActor(TargetActor));
                        //if (TargetLocation != CPos.Zero)
                        //    w.Write(TargetLocation);
                        //if (TargetString != null)
                        //    w.Write(TargetString);
                        //if (ExtraLocation != CPos.Zero)
                        //    w.Write(ExtraLocation);
                        //if (ExtraData != 0)
                        //    w.Write(ExtraData);
                        if (ExtDatas == null)
                        {
                            w.Write(ExtDatas.Length);
                            w.Write(ExtDatas);
                        }

                        if (OpCode != ControllerConst.NULL_OP_CODE)
                        {
                            w.Write(this.OpCode);
                            w.Write(this.OpData);
                        }

                        return ret.ToArray();
                    }
            }
        }

    }
}
