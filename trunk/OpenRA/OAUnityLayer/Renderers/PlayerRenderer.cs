using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.ComponentAnim.Core;
using Engine.Interfaces;
using Engine.Primitives;
using UnityEngine;
using Animation = UnityEngine.Animation;

namespace OAUnityLayer.Renderers
{
    public class PlayerRenderer : IRender
    {
        public GameObject go;

        private Transform tran;

        private Animation animation;

        private Vector3 mCurPos;

        private Quaternion mCurRot;

        public PlayerRenderer(GameObject player)
        {
            this.go = player;
            this.tran = this.go.transform;
            this.animation = this.go.GetComponent<Animation>();
            this.tran.position = Vector3.zero;
        }



        public void Render(Actor self, IWorldRenderer wr)
        {
            //Vector3 pos = new Vector3(self.Pos.X * 1024,0,self.Pos.Y * 1024);
            //this.mCurPos = Vector3.Lerp(this.mCurPos, pos, Time.deltaTime*12);

            int rot = self.Facing;

            float rad = rot * Mathf.PI / 128;

            this.tran.eulerAngles = new Vector3(0,90 - rad * Mathf.Rad2Deg, 0);
        }

        public bool IsPlaying(string name)
        {
            return this.animation.IsPlaying(name);
        }

        public void CrossFadeQueued(string animation, float fadeLength, Engine.ComponentAnim.Core.QueueMode queue)
        {
            this.animation.CrossFadeQueued(animation,fadeLength,(UnityEngine.QueueMode)queue);
        }

        public void CrossFade(string animation, float fadeLength)
        {
            this.animation.CrossFade(animation,fadeLength);
        }

        public void RewindAnim()
        {
            this.animation.Rewind();
        }

        public void StopAnim()
        {
            this.animation.Stop();
        }

        public void SetAnimationStateSpeed(string name, float speed)
        {
            this.animation[name].speed = speed;
        }
    }
}
