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
        public List<T> Clients = null;
        public List<IClientPing> ClientPings = null;

        public IGlobal GlobalSettings = null;


        public T ClientWithIndex(int clientID)
        {
            return Clients.SingleOrDefault(c => c.Index == clientID);
        }

        public IEnumerable<T> NonBotClients
        {
            get { return Clients.Where(c => c.Bot == null); }
        }


        public Session()
        {
            this.Clients = new List<T>();
            this.ClientPings = new List<IClientPing>();
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
                w.Write(this.ClientPings.Count);
                foreach (var clientPing in ClientPings)
                {
                    var clientPingBytes = clientPing.Serialize();
                    w.Write(clientPingBytes.Length);
                    w.Write(clientPingBytes);
                }

                byte[] globalData = this.GlobalSettings.Serialize();
                w.Write(globalData.Length);
                w.Write(globalData);

                bytes = ms.ToArray();
            }
            return bytes;
        }

        public string FirstEmptySlot()
        {
            return "No Slot";
            //return Slots.FirstOrDefault(s => !s.Value.Closed && ClientInSlot(s.Key) == null).Key;
        }
    }
}
