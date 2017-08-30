using System.Collections.Generic;

namespace OpenRA.Traits
{
    public class FactionInfo : TraitInfo<Faction>
    {
        [Desc("This is the name exposed to the players.")]
        public readonly string Name = null;

        [Desc("This is the internal name for owner checks.")]
        public readonly string InternalName = null;

        [Desc("Pick a random faction as the player's faction out of this list.")]
        public readonly HashSet<string> RandomFactionMembers = new HashSet<string>();

        [Desc("The side that the faction belongs to. For example, England belongs to the 'Allies' side.")]
        public readonly string Side = null;

        [Translate]
        public readonly string Description = null;

        public readonly bool Selectable = true;
    }

    public class Faction { /* we're only interested in the Info */ }
}
