using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface INetWorld
    {
        bool Paused { get; set; }

        //bool PredictedPaused { get; set; }

        int SyncHash();

        void Tick();
     
    }
}
