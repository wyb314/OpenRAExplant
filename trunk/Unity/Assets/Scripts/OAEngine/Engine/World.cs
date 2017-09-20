using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Maps;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine.OrderGenerators;

namespace Engine
{
    public enum WorldType { Regular, Shellmap, Editor }
    public sealed class World : IWorld
    {
        public int Timestep;

        private readonly Map Map = null;
        internal readonly IOrderManager<ClientDefault> OrderManager;
        public Session<ClientDefault> LobbyInfo { get { return OrderManager.LobbyInfo; } }

        public Dictionary<int,Player> Players = new Dictionary<int, Player>();

        public World(ModData modData, Map map, IOrderManager<ClientDefault> orderManager, WorldType type)
        {
            this.OrderManager = orderManager;
            Timestep = orderManager.LobbyInfo.GlobalSettings.Timestep;
            this.Map = map;
            this.orderGenerator = new PlayerControllerOrderGenerator(Platform.platformInfo.inputer);
            this.CreatePlayers(orderManager);
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


        private void CreatePlayers(IOrderManager<ClientDefault> orderManager)
        {
            var worldPlayers = new List<Player>();
            Player localPlayer = null;
            foreach (var kv in orderManager.LobbyInfo.Slots)
            {
                var client = orderManager.LobbyInfo.ClientInSlot(kv.Key);
                if (client == null)
                    continue;

                var player = new Player(this, client);
                worldPlayers.Add(player);

                if (client.Index == orderManager.Connection.LocalClientId)
                    localPlayer = player;
            }

            this.Players = worldPlayers.ToDictionary(p => p.ClientIndex, p => p);

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


        public void IssureOrder(IOrder order)
        {
            this.OrderManager.IssueOrder(order);
        }

        public void ProcessOrder(int clientId, IOrder order)
        {
            Player p = this.Players[clientId];
            if (p != null)
            {
                p.ProcessOrder(order);
            }
        }
    }
}
