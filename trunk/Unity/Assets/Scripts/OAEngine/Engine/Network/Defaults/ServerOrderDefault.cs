using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class ServerOrderDefault : IServerOrder
    {
        public string Data { private set; get; }

        public string Name { private set; get; }


        public ServerOrderDefault(string name, string data)
        {
            Name = name;
            Data = data;
        }

        public static ServerOrderDefault Deserialize(BinaryReader r)
        {
            byte b;
            switch (b = r.ReadByte())
            {
                case 0xff:
                    Console.WriteLine("This isn't a server order.");
                    return null;

                case 0xfe:
                    {
                        var name = r.ReadString();
                        var data = r.ReadString();

                        return new ServerOrderDefault(name, data);
                    }

                default:
                    throw new NotImplementedException(b.ToString("x2"));
            }
        }

        public byte[] Serialize()
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            bw.Write((byte)0xfe);
            bw.Write(Name);
            bw.Write(Data);
            return ms.ToArray();
        }
    }
}
