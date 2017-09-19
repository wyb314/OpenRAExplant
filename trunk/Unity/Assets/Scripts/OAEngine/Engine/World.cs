using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;

namespace Engine
{
    public enum WorldType { Regular, Shellmap, Editor }
    public sealed class World : IWorld
    {

        public World(ModData modData, Map map, IOrderManager<ClientDefault> orderManager, WorldType type)
        {
            
        }

        IOrderGenerator orderGenerator;
        public IOrderGenerator OrderGenerator
        {
            get
            {
                return orderGenerator;
            }

            set
            {
                Sync.AssertUnsynced("The current order generator may not be changed from synced code");
                orderGenerator = value;
            }
        }

        public bool Paused { get; set; }

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
