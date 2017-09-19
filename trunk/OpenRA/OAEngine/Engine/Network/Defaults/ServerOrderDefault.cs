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
        public string Name { private set; get; }
        
        public byte[] Data { private set; get; }
        

        public ServerOrderDefault(string name, byte[] data)
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
                        var length = r.ReadInt32();
                        
                        byte[] bytes = null;
                        if (length > 0)
                        {
                            bytes = r.ReadBytes(length);
                        }
                        return new ServerOrderDefault(name, bytes);
                    }

                default:
                    throw new NotImplementedException(b.ToString("x2"));
            }
        }

        public byte[] Serialize()
        {
            byte[] ret = null;
            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms);
                bw.Write((byte)0xfe);
                bw.Write(Name);
                int count = Data == null ? 0 : Data.Length;
                bw.Write(count);
                if (count > 0)
                {
                    bw.Write(Data);
                }
                ret = ms.ToArray();
            }
            return ret;
        }
    }
}
