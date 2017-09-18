using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface ISlot
    {
        string PlayerReference { get; }

        bool Closed { get; }

        bool AllowBots { get; }

        bool LockFaction { get; }

        bool LockColor { get; }

        bool LockTeam { get; }
        bool LockSpawn { get; }

        bool Required { get; }

        byte[] Serialize();
    }
}
