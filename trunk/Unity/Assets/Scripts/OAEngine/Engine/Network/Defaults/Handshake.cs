using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Engine.Network.Defaults
{
    public class HandshakeRequest
    {
        public string Mod;
        public string Version;
        public string Map;

        public static HandshakeRequest Deserialize(string data)
        {
            var handshake = new HandshakeRequest();
            return handshake;
        }

        public byte[] Serialize()
        {
            var ret = new MemoryStream();
            var w = new BinaryWriter(ret);
            w.Write(this.Mod);
            w.Write(Version);
            w.Write(Map);
            return ret.ToArray();
        }
    }

    public class HandshakeResponse
    {
        public string Mod;
        public string Version;
        public string Password;
        public ClientDefault Client;

        public static HandshakeResponse Deserialize(byte[] data)
        {
            var handshake = new HandshakeResponse();
            using (var ret = new MemoryStream(data))
            {
                var r = new BinaryReader(ret);
                handshake.Mod = r.ReadString();
                handshake.Version = r.ReadString();
                handshake.Password = r.ReadString();
                handshake.Client = ClientDefault.Deserialize(data);
            }
            return handshake;
        }

        public byte[] Serialize()
        {
            var ret = new MemoryStream();
            var w = new BinaryWriter(ret);
            w.Write(this.Mod);
            w.Write(Version);
            w.Write(this.Password);
            byte[] clientData = this.Client.Serialize();


            return ret.ToArray();
        }
    }
}
