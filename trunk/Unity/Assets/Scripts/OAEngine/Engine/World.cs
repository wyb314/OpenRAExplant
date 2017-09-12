using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Network.Interfaces;
using OAEngine.Engine;

namespace Engine
{
    public enum WorldType { Regular, Shellmap, Editor }
    public sealed class World : IWorld
    {

        public World(ModData modData, Map map, IOrderManager orderManager, WorldType type)
        {
            
        }

        public void LoadComplete(IWorldRenderer worldRenderer)
        {
        }

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
