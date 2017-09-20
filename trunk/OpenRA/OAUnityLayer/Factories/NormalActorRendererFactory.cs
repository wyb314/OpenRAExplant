using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using OAUnityLayer.Renderers;

namespace OAUnityLayer.Factories
{
    public class NormalActorRendererFactory : IActorRendererFactory
    {
        public IRender CreateActorRenderer()
        {
            return new PlayerRenderer();
        }
    }
}
