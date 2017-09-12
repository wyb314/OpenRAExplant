using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network
{
    public class Session<T,U> where T : IClient where U : IClientPing
    {
        public List<T> Clients = new List<T>();
        public List<U> ClientPings = new List<U>();

        public IGlobal GlobalSettings;


        public T ClientWithIndex(int clientID)
        {
            return Clients.SingleOrDefault(c => c.Index == clientID);
        }

        public IEnumerable<T> NonBotClients
        {
            get { return Clients.Where(c => c.Bot == null); }
        }

        public string Serialize()
        {
            return null;
        }

        public static Session<T, U> Deserialize(string data)
        {
            return null;
        }
    }
}
