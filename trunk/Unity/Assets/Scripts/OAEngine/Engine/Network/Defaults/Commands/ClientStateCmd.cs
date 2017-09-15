using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults.Commands
{
    public class ClientStateCmd : ICommand
    {
        public string commandName { set; get; }

        public byte[] Serialize()
        {
            return null;
        }
    }
}
