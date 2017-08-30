using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRA.Graphics;
using OpenRA.Network;
using OpenRA.Primitives;
using OpenRA.Server;

namespace OpenRA.Traits
{
    public enum SubCell
    {
        Invalid = int.MinValue,
        Any = int.MinValue/2,
        FullCell = 0,
        First = 1
    }

    public interface ITraitInfoInterface
    {
    }

    public interface ITraitInfo : ITraitInfoInterface
    {
        object Create(ActorInitializer init);
    }

    public interface Requires<T> where T : class, ITraitInfoInterface
    {
    }

    public interface ILobbyOptions : ITraitInfoInterface
    {
        IEnumerable<LobbyOption> LobbyOptions(Ruleset rules);
    }

    public class LobbyOption
    {
        public readonly string Id;
        public readonly string Name;
        public readonly IReadOnlyDictionary<string, string> Values;
        public readonly string DefaultValue;
        public readonly bool Locked;

        public LobbyOption(string id, string name, IReadOnlyDictionary<string, string> values, string defaultValue,
            bool locked)
        {
            Id = id;
            Name = name;
            Values = values;
            DefaultValue = defaultValue;
            Locked = locked;
        }

        public virtual string ValueChangedMessage(string playerName, string newValue)
        {
            return playerName + " changed " + Name + " to " + Values[newValue] + ".";
        }
    }

    public class LobbyBooleanOption : LobbyOption
    {
        static readonly Dictionary<string, string> BoolValues = new Dictionary<string, string>()
        {
            {true.ToString(), "enabled"},
            {false.ToString(), "disabled"}
        };

        public LobbyBooleanOption(string id, string name, bool defaultValue, bool locked)
            : base(id, name, new ReadOnlyDictionary<string, string>(BoolValues), defaultValue.ToString(), locked)
        {
        }

        public override string ValueChangedMessage(string playerName, string newValue)
        {
            return playerName + " " + BoolValues[newValue] + " " + Name + ".";
        }
    }

    public interface INotifyCreated
    {
        void Created(Actor self);
    }

    public class TraitInfo<T> : ITraitInfo where T : new()
    {
        public virtual object Create(ActorInitializer init)
        {
            return new T();
        }
    }

    public interface IFogVisibilityModifier
    {
        bool IsVisible(Actor actor);
        bool HasFogVisibility();
    }

    public interface IEffectiveOwner
    {
        bool Disguised { get; }
        Player Owner { get; }
    }

    public interface IOccupySpace
    {
        WPos CenterPosition { get; }
        CPos TopLeft { get; }
        IEnumerable<Pair<CPos, SubCell>> OccupiedCells();
    }

    public interface ITargetable
    {
        // Check IsTraitEnabled or !IsTraitDisabled first
        HashSet<string> TargetTypes { get; }
        bool TargetableBy(Actor self, Actor byActor);
        bool RequiresForceFire { get; }
    }

    public interface IFacing
    {
        int TurnSpeed { get; }
        int Facing { get; set; }
    }

    [Flags]
    public enum DamageState
    {
        Undamaged = 1,
        Light = 2,
        Medium = 4,
        Heavy = 8,
        Critical = 16,
        Dead = 32
    }

    public class Damage
    {
        public readonly int Value;
        public readonly HashSet<string> DamageTypes;

        public Damage(int damage, HashSet<string> damageTypes)
        {
            Value = damage;
            DamageTypes = damageTypes;
        }

        public Damage(int damage)
        {
            Value = damage;
            DamageTypes = new HashSet<string>();
        }
    }

    public interface IHealth
    {
        DamageState DamageState { get; }
        int HP { get; }
        int MaxHP { get; }
        int DisplayHP { get; }
        bool IsDead { get; }

        void InflictDamage(Actor self, Actor attacker, Damage damage, bool ignoreModifiers);
        void Kill(Actor self, Actor attacker);
    }

    public interface IRenderModifier
    {
        IEnumerable<IRenderable> ModifyRender(Actor self, WorldRenderer wr, IEnumerable<IRenderable> r);

    }
    
    public interface IDisable { bool Disabled { get; } }

    public interface IVisibilityModifier { bool IsVisible(Actor self, Player byPlayer); }

    public interface IDefaultVisibility { bool IsVisible(Actor self, Player byPlayer); }

    public interface IValidateOrder { bool OrderValidation(OrderManager orderManager, World world, int clientId, Order order); }

    [Flags]
    public enum Stance
    {
        None = 0,
        Enemy = 1,
        Neutral = 2,
        Ally = 4,
    }

    public interface IBotInfo : ITraitInfoInterface
    {
        string Type { get; }
        string Name { get; }
    }

    public interface IGameOver { void GameOver(World world); }

    public interface IActorMap
    {
        IEnumerable<Actor> GetActorsAt(CPos a);
        IEnumerable<Actor> GetActorsAt(CPos a, SubCell sub);
        bool HasFreeSubCell(CPos cell, bool checkTransient = true);
        SubCell FreeSubCell(CPos cell, SubCell preferredSubCell = SubCell.Any, bool checkTransient = true);
        SubCell FreeSubCell(CPos cell, SubCell preferredSubCell, Func<Actor, bool> checkIfBlocker);
        bool AnyActorsAt(CPos a);
        bool AnyActorsAt(CPos a, SubCell sub, bool checkTransient = true);
        bool AnyActorsAt(CPos a, SubCell sub, Func<Actor, bool> withCondition);
        void AddInfluence(Actor self, IOccupySpace ios);
        void RemoveInfluence(Actor self, IOccupySpace ios);
        int AddCellTrigger(CPos[] cells, Action<Actor> onEntry, Action<Actor> onExit);
        void RemoveCellTrigger(int id);
        int AddProximityTrigger(WPos pos, WDist range, WDist vRange, Action<Actor> onEntry, Action<Actor> onExit);
        void RemoveProximityTrigger(int id);
        void UpdateProximityTrigger(int id, WPos newPos, WDist newRange, WDist newVRange);
        void AddPosition(Actor a, IOccupySpace ios);
        void RemovePosition(Actor a, IOccupySpace ios);
        void UpdatePosition(Actor a, IOccupySpace ios);
        IEnumerable<Actor> ActorsInBox(WPos a, WPos b);
    }

    public interface INotifySelected { void Selected(Actor self); }
    public interface INotifySelection { void SelectionChanged(); }
    public interface IWorldLoaded { void WorldLoaded(World w, WorldRenderer wr); }
    public interface ICreatePlayers { void CreatePlayers(World w); }

    public interface IVoiced
    {
        string VoiceSet { get; }
        bool PlayVoice(Actor self, string phrase, string variant);
        bool PlayVoiceLocal(Actor self, string phrase, string variant, float volume);
        bool HasVoice(Actor self, string voice);
    }

    public interface ITargetableInfo : ITraitInfoInterface
    {
        HashSet<string> GetTargetTypes();
    }

    [Flags]
    public enum TargetModifiers { None = 0, ForceAttack = 1, ForceQueue = 2, ForceMove = 4 }

    public static class TargetModifiersExts
    {
        public static bool HasModifier(this TargetModifiers self, TargetModifiers m)
        {
            // PERF: Enum.HasFlag is slower and requires allocations.
            return (self & m) == m;
        }
    }

    public interface IOrderTargeter
    {
        string OrderID { get; }
        int OrderPriority { get; }
        bool CanTarget(Actor self, Target target, List<Actor> othersAtTarget, ref TargetModifiers modifiers, ref string cursor);
        bool IsQueued { get; }
        bool TargetOverridesSelection(TargetModifiers modifiers);
    }

    public interface IIssueOrder
    {
        IEnumerable<IOrderTargeter> Orders { get; }
        Order IssueOrder(Actor self, IOrderTargeter order, Target target, bool queued);
    }

    public interface INotifyAddedToWorld { void AddedToWorld(Actor self); }

    public interface INotifyRemovedFromWorld { void RemovedFromWorld(Actor self); }

    public interface INotifyIdle { void TickIdle(Actor self); }

    public sealed class RequireExplicitImplementationAttribute : Attribute { }

    [RequireExplicitImplementation]
    public interface ITemporaryBlocker
    {
        bool CanRemoveBlockage(Actor self, Actor blocking);
        bool IsBlocking(Actor self, CPos cell);
    }

    public interface ITick { void Tick(Actor self); }

    public interface ITickRender { void TickRender(WorldRenderer wr, Actor self); }

    public interface IRender
    {
        IEnumerable<IRenderable> Render(Actor self, WorldRenderer wr);
    }


    public interface ITooltipInfo : ITraitInfoInterface
    {
        string TooltipForPlayerStance(Stance stance);
        bool IsOwnerRowVisible { get; }
    }

    public interface ITooltip
    {
        ITooltipInfo TooltipInfo { get; }
        Player Owner { get; }
    }

    public interface IOccupySpaceInfo : ITraitInfoInterface
    {
        IReadOnlyDictionary<CPos, SubCell> OccupiedCells(ActorInfo info, CPos location, SubCell subCell = SubCell.Any);
        bool SharesCell { get; }
    }

    public interface IAutoSelectionSize { int2 SelectionSize(Actor self); }

    public interface ISelectionDecorationsInfo : ITraitInfoInterface
    {
        int[] SelectionBoxBounds { get; }
    }

    public interface INotifyBecomingIdle { void OnBecomingIdle(Actor self); }

    public interface INotifyActorDisposing { void Disposing(Actor self); }

    public interface INotifyOwnerChanged { void OnOwnerChanged(Actor self, Player oldOwner, Player newOwner); }

    public interface IWarhead
    {
        int Delay { get; }
        bool IsValidAgainst(Actor victim, Actor firedBy);
        bool IsValidAgainst(FrozenActor victim, Actor firedBy);
        void DoImpact(Target target, Actor firedBy, IEnumerable<int> damageModifiers);
    }

    public interface IRulesetLoaded<TInfo> { void RulesetLoaded(Ruleset rules, TInfo info); }
    public interface IRulesetLoaded : IRulesetLoaded<ActorInfo>, ITraitInfoInterface { }

    public enum PipType { Transparent, Green, Yellow, Red, Gray, Blue, Ammo, AmmoEmpty }

    public interface IBot
    {
        void Activate(Player p);
        IBotInfo Info { get; }
    }


    public interface UsesInit<T> : ITraitInfo where T : IActorInit { }

    public interface ITargetablePositions
    {
        IEnumerable<WPos> TargetablePositions(Actor self);
    }

    public interface IResolveOrder { void ResolveOrder(Actor self, Order order); }
}
