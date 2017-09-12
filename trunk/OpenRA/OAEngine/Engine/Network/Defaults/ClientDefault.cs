using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Enums;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class ClientDefault : IClient
    {
        public string Bot { set; get; }

        public int Index { set; get; }

        public string IpAddress { set; get; }

        public bool IsAdmin { set; get; }

        public string Name { set; get; }

        public string Slot { set; get; }

        public ClientState State { set; get; }

        public int BotControllerClientIndex { set; get; }

        public bool IsReady { get { return State == ClientState.Ready; } }
        public bool IsInvalid { get { return State == ClientState.Invalid; } }

        public bool IsObserver { get { return Slot == null; } }

        public string Serialize()
        {
            return null;
        }
    }

    public static class ClientDefaultExts
    {
        public static string WriteToString(this List<ClientDefault> y)
        {
            return null;
        }
    }

    
}
