

namespace Engine.Maps
{
    public partial class PlayerReference
    {
        public string Name { set; get; }
        public string Bot { set; get; }
        public string StartingUnitsClass { set; get; }

        private bool allowBots = true;

        public bool AllowBots
        {
            set { this.allowBots = value; }
            get { return this.allowBots; }
        }
        public bool Playable { set; get; }
        public bool Required { set; get; }
        public bool OwnsWorld { set; get; }
        public bool Spectating { set; get; }
        public bool NonCombatant { set; get; }

        public bool LockFaction { set; get; }
        public string Faction { set; get; }

        public bool LockSpawn { set; get; }
        public int Spawn { set; get; }

        public bool LockTeam { set; get; }
        public int Team { set; get; }

        public string[] Allies = { };
        public string[] Enemies = { };
    }
}
