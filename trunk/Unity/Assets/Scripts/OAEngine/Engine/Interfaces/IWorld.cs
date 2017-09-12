using System;
using System.Collections.Generic;
using Engine.Network.Interfaces;


namespace Engine.Interfaces
{
    public interface IWorld : INetWorld
    {

        void LoadComplete(IWorldRenderer worldRenderer);

        void TickRender(IWorldRenderer worldRenderer);

    }
}
