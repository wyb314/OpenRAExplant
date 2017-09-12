using System;
using System.Collections.Generic;
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

        public string Serialize()
        {
            return null;
        }
    }

    public class HandshakeResponse
    {
        public string Mod;
        public string Version;
        public string Password;
        public ClientDefault Client;

        public static HandshakeResponse Deserialize(string data)
        {
            var handshake = new HandshakeResponse();
            handshake.Client = new ClientDefault();

            //var ys = MiniYaml.FromString(data);
            //foreach (var y in ys)
            //{
            //    switch (y.Key)
            //    {
            //        case "Handshake":
            //            FieldLoader.Load(handshake, y.Value);
            //            break;
            //        case "Client":
            //            FieldLoader.Load(handshake.Client, y.Value);
            //            break;
            //    }
            //}

            return handshake;
        }

        public string Serialize()
        {
            //var data = new List<MiniYamlNode>();
            //data.Add(new MiniYamlNode("Handshake", null,
            //    new string[] { "Mod", "Version", "Password" }.Select(p => FieldSaver.SaveField(this, p)).ToList()));
            //data.Add(new MiniYamlNode("Client", FieldSaver.Save(Client)));

            //return data.WriteToString();
            return null;
        }
    }
}
