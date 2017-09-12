using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public static class OrderIO
    {
        public static List<IOrder> ToOrderList(this byte[] bytes , INetWorld world)
        {
            var ms = new MemoryStream(bytes, 4, bytes.Length - 4);
            var reader = new BinaryReader(ms);
            var ret = new List<IOrder>();
            while (ms.Position < ms.Length)
            {
                var o = Order.Deserialize(world, reader);
                if (o != null)
                    ret.Add(o);
            }

            return ret;
        }

        public static byte[] SerializeSync(int sync)
        {
            var ms = new MemoryStream();
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write((byte)0x65);
                writer.Write(sync);
            }

            return ms.ToArray();
        }
    }
}
