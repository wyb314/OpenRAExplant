using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Interfaces;
using Engine.Primitives;
using OAUnityLayer.Renderers;
using UnityEngine;
using OAUnityLayer;

namespace OAUnityLayer.Factories
{
    public class NormalActorRendererFactory : IActorRendererFactory
    {
        public IRender CreateActorRenderer(WPos pos, int PlayerId, string pfbName)
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(pfbName));
            go.name = string.Format("Client_{0}", PlayerId);
            Vector3 unityPos = pos.ConvertWPos2UnityPos();
            go.transform.position = unityPos;
            PlayerRenderer pr = new PlayerRenderer(go);

            pr.Init();
            return pr;
        }
    }
}
