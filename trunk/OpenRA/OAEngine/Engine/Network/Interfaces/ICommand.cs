using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface ICommand
    {
        string commandName { set;get; }
        byte[] Serialize();
    }
}
