using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.ComponentAnim.Core;
using Engine.Interfaces;
using Engine.Primitives;
using UnityEngine;
using Animation = UnityEngine.Animation;
using TrueSync;

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
        }

        private SmothPos sp;
        public void Init()
        {
            sp = this.go.AddComponent<SmothPos>();
           
            this.InitAnimation();
        }

        private void InitAnimation()
        {
            Animation anims = this.animation;
            anims.wrapMode = WrapMode.Once;

            anims["idle"].layer = 0;
            anims["idleSword"].layer = 0;
            anims["run"].layer = 0;
            anims["runSword"].layer = 0;
            anims["walk"].layer = 0;
            anims["walkSword"].layer = 0;

            anims["deathBack"].layer = 2;
            anims["deathFront"].layer = 2;


            anims["injuryFrontSword"].layer = 1;
            anims["injuryFrontSword"].speed = 0.9f;
            anims["injuryBackSword"].layer = 1;
            anims["injuryBackSword"].speed = 0.9f;

            anims["evadeSword"].layer = 1;

            anims["showSword"].layer = 0;
            anims["hideSword"].layer = 0;
            anims["showSwordRun"].layer = 0;
            anims["hidSwordRun"].layer = 0;
            //  anims["showSwordRun"].blendMode = AnimationBlendMode.Additive;
            //  anims["hidSwordRun"].blendMode = AnimationBlendMode.Additive;

            anims["useLever"].layer = 0;
            // combo XXXXXX
            anims["attackX"].speed = 0.9f;
            anims["attackXX"].speed = 0.8f;
            anims["attackXXX"].speed = 0.8f;
            anims["attackXXXX"].speed = 0.8f;
            anims["attackXXXXX"].speed = 0.8f;

            anims["attackX"].layer = 1;
            anims["attackXX"].layer = 1;
            anims["attackXXX"].layer = 1;
            anims["attackXXXX"].layer = 1;
            anims["attackXXXXX"].layer = 1;
            // combo OOOXX
            anims["attackO"].speed = 1.2f;
            anims["attackOO"].speed = 1.5f;
            anims["attackOOO"].speed = 1.1f;
            anims["attackOOOX"].speed = 1;
            anims["attackOOOXX"].speed = 1.4f;

            anims["attackO"].layer = 1;
            anims["attackOO"].layer = 1;
            anims["attackOOO"].layer = 1;
            anims["attackOOOX"].layer = 1;
            anims["attackOOOXX"].layer = 1;
            // COMBO X00XX
            anims["attackXO"].speed = 1;
            anims["attackXOO"].speed = 1.2f;
            anims["attackXOOX"].speed = 1.2f;
            anims["attackXOOXX"].speed = 1.2f;

            anims["attackXO"].layer = 1;
            anims["attackXOO"].layer = 1;
            anims["attackXOOX"].layer = 1;
            anims["attackXOOXX"].layer = 1;

            // COMBO XX0XX
            anims["attackXXO"].speed = 1;
            anims["attackXXOX"].speed = 1.2f;
            anims["attackXXOXX"].speed = 1.3f;

            anims["attackXXO"].layer = 1;
            anims["attackXXOX"].layer = 1;
            anims["attackXXOXX"].layer = 1;

            // Combo OOXOO
            anims["attackOOX"].speed = 1;
            anims["attackOOXO"].speed = 1;
            anims["attackOOXOO"].speed = 1.3f;

            anims["attackOOX"].layer = 1;
            anims["attackOOXO"].layer = 1;
            anims["attackOOXOO"].layer = 1;

            // COMBO OXOOO
            anims["attackOX"].speed = 1.1f;
            anims["attackOXO"].speed = 1.2f;
            anims["attackOXOX"].speed = 1;
            anims["attackOXOXO"].speed = 1;

            anims["attackOX"].layer = 1;
            anims["attackOXO"].layer = 1;
            anims["attackOXOX"].layer = 1;
            anims["attackOXOXO"].layer = 1;
        }

        private int lastRot = int.MaxValue;
        private WPos lastPos = new WPos(int.MaxValue, 0,int.MaxValue);

        private Vector3 curPos = Vector3.zero;
        private Quaternion curRot;
        public void Render(Actor self, IWorldRenderer wr)
        {
            sp.SetActor(self);
            return;
            //Vector3 pos = new Vector3(self.Pos.X * 1024,0,self.Pos.Y * 1024);
            //this.mCurPos = Vector3.Lerp(this.mCurPos, pos, Time.deltaTime*12);

            //if (this.lastRot != self.Facing)
            //{
            //    this.lastRot = self.Facing;
            //    int rot = self.Facing;

            //    float rad = rot * Mathf.PI / 128;

            //    curRot = Quaternion.Euler(new Vector3(0, -rad*Mathf.Rad2Deg, 0));

            //    this.tran.rotation = curRot;

            //}

            ////this.tran.eulerAngles = new Vector3(0, -rad * Mathf.Rad2Deg, 0);

            

            //if (this.lastPos != self.Pos)
            //{
            //    this.lastPos = self.Pos;
            //    //
            //    curPos = new Vector3(((float)self.Pos.X) / 1024, 0, -((float)self.Pos.Y) / 1024);

            //    this.tran.position = curPos;
            //}
            
            //this.tran.position = Vector3.Lerp(this.tran.position,curPos,((float)Game.Timestep) * 13);
           

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

    public class SmothPos : MonoBehaviour
    {
        private Transform tran;
        void Start()
        {
            tran = transform;
        }

        public Actor actor;
        public void SetActor(Actor actor)
        {
            this.actor = actor;
        }

        void FixedUpdate()
        {
            if (actor == null)
            {
                return;
            }

            //float rad = this.actor.Facing * Mathf.PI / 128;

            //Quaternion rot = Quaternion.Euler(new Vector3(0, -rad * Mathf.Rad2Deg, 0));

            //this.tran.rotation = Quaternion.Lerp(this.tran.rotation, rot, Time.deltaTime*8);

            //Vector3 curPos = new Vector3(((float)actor.Pos.X) / 1024, 0, -((float)actor.Pos.Y) / 1024);

            //this.tran.position = Vector3.Lerp(this.tran.position, curPos, ((float)Game.Timestep) * 13);
            

            Quaternion rot = new Quaternion(this.actor.Facing.x.AsFloat(),
                this.actor.Facing.y.AsFloat(),
                this.actor.Facing.z.AsFloat(),
                this.actor.Facing.w.AsFloat());

            this.tran.rotation = rot;

            Vector3 curPos = new Vector3(((float)actor.Pos.x), 0, (float)actor.Pos.y);

            this.tran.position = curPos;
        }
    }
}
