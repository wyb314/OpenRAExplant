using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using OAUnityLayer.Renderers;
using UnityEngine;

namespace OAUnityLayer.Factories
{
    public class NormalActorRendererFactory : IActorRendererFactory
    {
        public IRender CreateActorRenderer()
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Player"));
            return new PlayerRenderer(go);
        }
    }
}
