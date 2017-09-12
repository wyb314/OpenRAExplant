using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Network.Interfaces;

namespace Engine.World
{
    public sealed class World : IWorld
    {
        public int SyncHash()
        {
            return 0;
        }

        public void Tick()
        {
            
        }
        
        public void TickRender(IWorldRenderer worldRenderer)
        {
        }
    }
}
