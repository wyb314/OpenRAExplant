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

        private Transform tran;
        private Vector3 mCurPos;

        private Quaternion mCurRot;

        public PlayerRenderer(GameObject player)
        {
            this.go = player;
            this.tran = this.go.transform;
            this.tran.position = Vector3.zero;
        }



        public void Render(Actor self, IWorldRenderer wr)
        {
            //Vector3 pos = new Vector3(self.Pos.X * 1024,0,self.Pos.Y * 1024);
            //this.mCurPos = Vector3.Lerp(this.mCurPos, pos, Time.deltaTime*12);

            int rot = self.Rot;

            float rad = rot * Mathf.PI / 128;

            this.tran.eulerAngles = new Vector3(0,90 - rad * Mathf.Rad2Deg, 0);
        }


    }
}
