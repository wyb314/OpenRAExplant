using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class ClientPingDefault : IClientPing
    {
        public int Index
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public long Latency
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public long[] LatencyHistory
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public long LatencyJitter
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
