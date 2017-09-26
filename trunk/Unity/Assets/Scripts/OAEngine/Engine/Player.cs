using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Maps;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Interfaces;
using Engine.Support;

namespace Engine
{
    public enum PowerState { Normal, Low, Critical }
    public enum WinState { Undefined, Won, Lost }

    public class Player
    {
        public readonly string InternalName;

        public readonly Actor PlayerActor;

        public World World { get; private set; }

        public readonly int ClientIndex;
        public Player(World world, IClient client)
        {
            this.World = world;

            if (client != null)
            {
                this.ClientIndex = client.Index;
            }
            this.PlayerActor = new Actor(world,"",null);
            
            world.Add(this.PlayerActor);
            //this.InternalName = pr.Name;

        }

        public void ProcessOrder(IOrder order)
        {
            this.PlayerActor.ProcessOrder(order);
        }
    }
}
