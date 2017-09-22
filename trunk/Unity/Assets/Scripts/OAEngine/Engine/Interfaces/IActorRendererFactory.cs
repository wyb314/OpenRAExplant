using System;
using System.Collections.Generic;

namespace Engine.Interfaces
{
    public interface IActorRendererFactory
    {
        IRender CreateActorRenderer();

    }
}
