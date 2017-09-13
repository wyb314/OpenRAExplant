using System;
using System.Collections.Generic;
using Engine.Network.Enums;

namespace Engine.Network.Interfaces
{
    public interface IClient
    {
        int Index { set; get; }

        string Name { set; get; }

        string IpAddress { set; get; }

        ClientState State { set; get; }

        string Bot { set; get; }

        bool IsAdmin { set; get; }

        string Slot { set; get; }

        int BotControllerClientIndex { set; get; }

        byte[] Serialize();
    }
}
