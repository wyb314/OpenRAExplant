

using System;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public sealed class GlobalDefault : IGlobal
    {
        private bool allowSpectators = true;
        public bool AllowSpectators
        {
            get { return this.allowSpectators; }

            set { this.allowSpectators = value; }
        }
        
        public bool AllowVersionMismatch { set; get; }

        public bool EnableSingleplayer { set; get; }

        public string GameUid { set; get; }

        public string Map { set; get; }

        private int orderLatency = 3;
        public int OrderLatency
        {
            get { return this.orderLatency; }

            set { this.orderLatency = value; }
        }

        public int RandomSeed { set; get; }

        public string ServerName { set; get; }

        private int timeStep = 40;
        public int Timestep
        {
            get { return this.timeStep; }

            set { this.timeStep = value; }
        }
    }
}
