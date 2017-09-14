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

        public static HandshakeRequest Deserialize(byte[] data)
        {
            var handshake = new HandshakeRequest();
            using (var ms = new MemoryStream(data))
            {
                var br = new BinaryReader(ms);
                handshake.Mod = br.ReadString();
                handshake.Version = br.ReadString();
                handshake.Map = br.ReadString();
            }
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
                byte[] clientData = r.ReadBytes(r.ReadInt32());
                handshake.Client = ClientDefault.Deserialize(clientData);
            }
            return handshake;
        }

        public byte[] Serialize()
        {
            byte[] ret = null;

            using (var ms = new MemoryStream())
            {
                var w = new BinaryWriter(ms);
                w.Write(this.Mod);
                w.Write(Version);
                w.Write(this.Password);
                byte[] clientData = this.Client.Serialize();
                w.Write(clientData.Length);
                w.Write(clientData);
                ret = ms.ToArray();
            }
            return ret;
        }
    }
}
