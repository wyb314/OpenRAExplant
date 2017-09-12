using System;
using System.Collections.Generic;
using Engine.Network.Interfaces;


namespace Engine.Interfaces
{
    public interface IWorld : INetWorld
    {
        void TickRender(IWorldRenderer worldRenderer);
    }
}
