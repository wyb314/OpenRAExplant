using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network
{
    public class Session<T> where T : IClient
    {
        public List<T> Clients = new List<T>();
        public List<IClientPing> ClientPings = new List<IClientPing>();

        public IGlobal GlobalSettings;


        public T ClientWithIndex(int clientID)
        {
            return Clients.SingleOrDefault(c => c.Index == clientID);
        }

        public IEnumerable<T> NonBotClients
        {
            get { return Clients.Where(c => c.Bot == null); }
        }

        public byte[] Serialize()
        {
            byte[] bytes = null;

            using (var ms = new MemoryStream())
            {
                var w = new BinaryWriter(ms);
                w.Write(this.Clients.Count);
                foreach (var client in Clients)
                {
                    var clientBytes = client.Serialize();
                    w.Write(clientBytes.Length);
                    w.Write(clientBytes);
                }

                foreach (var clientPing in ClientPings)
                {
                    var clientPingBytes = clientPing.Serialize();
                    w.Write(clientPingBytes.Length);
                    w.Write(clientPingBytes);
                }

            }
               

            return null;
        }

        public static Session<T> Deserialize(string data)
        {
            return null;
        }
    }
}
