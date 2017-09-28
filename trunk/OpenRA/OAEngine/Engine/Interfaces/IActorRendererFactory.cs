using System;
using System.Collections.Generic;
using Engine.Primitives;

namespace Engine.Interfaces
{
    public interface IActorRendererFactory
    {
        IRender CreateActorRenderer(WPos pos ,int PlayerId, string pfbName);

    }
}
