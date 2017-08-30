using System;
using System.Collections.Generic;
using System.Linq;
using OpenRA.Primitives;

namespace OpenRA
{
    public interface IActorInit { }

    public interface IActorInit<T> : IActorInit
    {
        T Value(World world);
    }

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

        //在TypeDictionary里检索对应的ActorInit并返回里面的值
        public U Get<T, U>() where T : IActorInit<U> { return Dict.Get<T>().Value(World); }
        public bool Contains<T>() where T : IActorInit { return Dict.Contains<T>(); }
    }

    public class LocationInit : IActorInit<CPos>
    {
        [FieldFromYamlKey]
        readonly CPos value = CPos.Zero;
        public LocationInit() { }
        public LocationInit(CPos init) { value = init; }
        public CPos Value(World world) { return value; }
    }

    public class OwnerInit : IActorInit<Player>
    {
        [FieldFromYamlKey]
        public readonly string PlayerName = "Neutral";
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
