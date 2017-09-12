using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class ServerConnectoinDefault : IServerConnectoin<ClientDefault, ClientPingDefault>
    {
        public int PlayerIndex { set; get; }

        public int MostRecentFrame { private set;get; }

        public Socket Socket { set; get; }

        public void ReadData(IServer<ClientDefault, ClientPingDefault> server)
        {
            
        }
    }
}
