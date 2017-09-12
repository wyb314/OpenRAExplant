using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Network.Interfaces
{
    public interface IServerOrder
    {
        string Name { get; }

        string Data { get; }

        byte[] Serialize();
    }
}
