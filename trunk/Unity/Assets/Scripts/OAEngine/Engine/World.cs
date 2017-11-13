using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using Engine.Interfaces;
using Engine.Maps;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine.OrderGenerators;
using Engine.Physics;
using Engine.Physics.Walls;
using Engine.Support;
using TrueSync;
using IWorld = Engine.Interfaces.IWorld;

namespace Engine
{
    public enum WorldType { Regular, Shellmap, Editor }
    public sealed class World : IWorld
    {
        public int Timestep;

        //internal readonly TraitDictionary TraitDict = new TraitDictionary();
        readonly SortedDictionary<uint, Actor> actors = new SortedDictionary<uint, Actor>();

        private readonly Map Map = null;
        internal readonly IOrderManager<ClientDefault> OrderManager;
        public Session<ClientDefault> LobbyInfo { get { return OrderManager.LobbyInfo; } }

        public readonly MersenneTwister SharedRandom;

        public Player[] Players = new Player[0];

        public readonly List<Agent> WorldAgents = new List<Agent>();
        

        readonly Queue<Action<World>> frameEndActions = new Queue<Action<World>>();

        public World(ModData modData, Map map, IOrderManager<ClientDefault> orderManager, WorldType type)
        {
            this.OrderManager = orderManager;
            Timestep = orderManager.LobbyInfo.GlobalSettings.Timestep;
            this.Map = map;
            this.orderGenerator = new PlayerControllerOrderGenerator();

            SharedRandom = new MersenneTwister(orderManager.LobbyInfo.GlobalSettings.RandomSeed);

            this.InitPhysics();


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

        public void AddFrameEndTask(Action<World> a) { frameEndActions.Enqueue(a); }
        public int WorldTick { get; private set; }
        
        public void InitPhysics()
        {
            TSRandom.Init();
            var pm = PhysicsManager.New();// init 2d  deterministic physics
            pm.LockedTimeStep = new FP(Time.Timestep) / new FP(1000); ;
            PhysicsManager.instance.Init();

        }


        private void CreatePlayers(IOrderManager<ClientDefault> orderManager)
        {
            var worldPlayers = new List<Player>();
            Player localPlayer = null;
            foreach (var client in orderManager.LobbyInfo.Clients)
            {
                //var client = orderManager.LobbyInfo.ClientInSlot(kv.Key);
                if (client == null)
                    continue;

                var player = new Player(this, client);
                worldPlayers.Add(player);

                if (client.Index == orderManager.Connection.LocalClientId)
                    localPlayer = player;
            }

            this.Players = worldPlayers.ToArray();

        }


        public void LoadComplete(IWorldRenderer worldRenderer)
        {
            this.BuildScene();
        }


        private void BuildScene()
        {
            SampleWall2D wall0 = new SampleWall2D(new TSVector2(0,5),new TSVector2(100,2));
            wall0.AddToPhysicWorld();
        }

        public void AddAgent(Agent agent)
        {
            if (!this.WorldAgents.Contains(agent))
            {
                this.WorldAgents.Add(agent);
            }
        }

        public List<Agent> GetEmemys(Agent self)
        {
            List<Agent> result = null;
            foreach (var play in this.Players)
            {
                if (play.PlayerActor.agent != self)
                {
                    if (result == null) result = new List<Agent>();
                    result.Add(play.PlayerActor.agent);
                }
            }
            return result;
        }

        public int SyncHash()
        {
            return 0;
        }

        uint nextAID = 0;
        internal uint NextAID()
        {
            return nextAID++;
        }

        public void Tick()
        {
            if (!Paused)
            {
                WorldTick++;

                Time.time += Time.deltaTime;

                PhysicsManager.instance.UpdateStep();
                
                foreach (var a in actors.Values)
                    a.Tick();
            }

            while (frameEndActions.Count != 0)
                frameEndActions.Dequeue()(this);
        }
        
        public void TickRender(IWorldRenderer worldRenderer)
        {
            foreach (var a in actors.Values)
                a.RenderSelf(worldRenderer);
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

        public void Add(Actor a)
        {
            actors.Add(a.ActorID, a);
        }
    }
}
