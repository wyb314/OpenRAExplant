using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Interfaces;
using Engine.Primitives;
using UnityEngine;

namespace OAUnityLayer.Renderers
{
    public class PlayerRenderer : IRender
    {
        public GameObject go;

        private Vector3 mCurPos;

        private Quaternion mCurRot;

        public PlayerRenderer()
        {
            this.go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
        }



        public void Render(Actor self, IWorldRenderer wr)
        {
            Vector3 pos = new Vector3(self.Pos.X * 1024,0,self.Pos.Y * 1024);
            this.mCurPos = Vector3.Lerp(this.mCurPos, pos, Time.deltaTime*12);
        }


    }
}
