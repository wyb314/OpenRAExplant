

using System.IO;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public sealed class Order : IOrder
    {

        public bool IsImmediate { set; get; }

        public string OrderString { private set; get; }


        public string TargetString { set; get; }

        public static Order Deserialize(INetWorld world, BinaryReader r)
        {
            return null;
        }

        public byte[] Serialize()
        {
            return null;
        }

    }
}
