﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA
{
    public class PlayerReference
    {
        public string Name;
        public string Palette;
        public string Bot = null;
        public string StartingUnitsClass = null;
        public bool AllowBots = true;
        public bool Playable = false;
        public bool Required = false;
        public bool OwnsWorld = false;
        public bool Spectating = false;
        public bool NonCombatant = false;

        public bool LockFaction = false;
        public string Faction;

        public bool LockColor = false;
        //public HSLColor Color = new HSLColor(0, 0, 238);

        public bool LockSpawn = false;
        public int Spawn = 0;

        public bool LockTeam = false;
        public int Team = 0;

        public string[] Allies = { };
        public string[] Enemies = { };

        public PlayerReference() { }
        public PlayerReference(MiniYaml my) { FieldLoader.Load(this, my); }

        public override string ToString() { return Name; }
    }
}
