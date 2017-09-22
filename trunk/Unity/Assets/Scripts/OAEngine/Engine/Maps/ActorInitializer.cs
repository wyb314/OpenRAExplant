using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Primitives;

namespace Engine.Maps
{
    public interface IActorInitializer
    {
        World World { get; }
        T Get<T>() where T : IActorInit;
        U Get<T, U>() where T : IActorInit<U>;
        bool Contains<T>() where T : IActorInit;
    }

    public class ActorInitializer : IActorInitializer
    {
        public readonly Actor Self;
        public World World { get { return Self.World; } }

        internal TypeDictionary Dict;

        public ActorInitializer(Actor actor, TypeDictionary dict)
        {
            Self = actor;
            Dict = dict;
        }

        public T Get<T>() where T : IActorInit { return Dict.Get<T>(); }
        public U Get<T, U>() where T : IActorInit<U> { return Dict.Get<T>().Value(World); }
        public bool Contains<T>() where T : IActorInit { return Dict.Contains<T>(); }
    }

    public interface IActorInit { }

    public interface IActorInit<T> : IActorInit
    {
        T Value(World world);
    }

    public class LocationInit : IActorInit<CPos>
    {
#region YAML Field
        public int X { set; get; }

        public int Y { set; get; }
#endregion

        public CPos value = CPos.Zero;
        public LocationInit() { }
        public LocationInit(CPos init) { value = init; }
        public CPos Value(World world) { return value; }
    }

    public class OwnerInit : IActorInit<Player>
    {
#region YAML Field

        private string playerName = "Neutral";

        public string PlayerName
        {
            set { this.playerName = value; }
            get { return this.playerName; }
        }
#endregion

        Player player;

        public OwnerInit() { }
        public OwnerInit(string playerName) { PlayerName = playerName; }

        public OwnerInit(Player player)
        {
            this.player = player;
            PlayerName = player.InternalName;
        }

        public Player Value(World world)
        {
            if (player != null)
                return player;

            return world.Players.First(x => x.InternalName == PlayerName);
        }
    }
}
