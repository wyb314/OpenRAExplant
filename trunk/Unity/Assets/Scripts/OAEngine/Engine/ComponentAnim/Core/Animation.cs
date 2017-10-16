using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using Engine.Interfaces;
using TrueSync;

namespace Engine.ComponentAnim.Core
{
    public enum QueueMode
    {
        CompleteOthers = 0,
        PlayNow = 2,
    }
    public class Animation
    {
        public IRender rendererProxy { private set; get; }

        private Dictionary<string,FP> animLengthDir = new Dictionary<string, FP>();
        


        public Animation(IRender rendererProxy)
        {
            this.rendererProxy = rendererProxy;
        }

        public void Init()
        {
            animLengthDir.Add("runSword", 0.533f);
            animLengthDir.Add("run", 0.533f);
            animLengthDir.Add("idleSword", 1.033f);
            animLengthDir.Add("idle", 1f);
            animLengthDir.Add("evadeSword", 0.5f);
            animLengthDir.Add("walkSword", 1.1f);
            animLengthDir.Add("walk", 0.9667f);
            animLengthDir.Add("showSword", 0.5f);
            animLengthDir.Add("hideSword", 2.5f);
            animLengthDir.Add("useLever", 3.333f);
            animLengthDir.Add("injuryFrontSword", 0.5f);
            animLengthDir.Add("injuryBackSword", 0.5f);
            animLengthDir.Add("attackJump", 1.1f);
            animLengthDir.Add("deathFront", 0.8f);
            animLengthDir.Add("deathBack", 1.5f);
            animLengthDir.Add("attackKnockdown", 1.667f);
            animLengthDir.Add("hidSwordRun", 1f);
            animLengthDir.Add("showSwordRun", 0.6f);
            animLengthDir.Add("attackX", 0.8f);
            animLengthDir.Add("attackXX", 0.833f);
            animLengthDir.Add("attackXXX", 0.8f);
            animLengthDir.Add("attackXXXX", 0.8f);
            animLengthDir.Add("attackXXXXX", 0.8f);
            animLengthDir.Add("attackO", 1.3f);
            animLengthDir.Add("attackOO", 1.5f);
            animLengthDir.Add("attackOOO", 1.333f);
            animLengthDir.Add("attackOOOX", 1.333f);
            animLengthDir.Add("attackOOOXX", 1.5f);
            animLengthDir.Add("attackXO", 1.333f);
            animLengthDir.Add("attackXOO", 1.233f);

            animLengthDir.Add("attackXOOX", 1.4f);
            animLengthDir.Add("attackXOOXX", 1.733f);
            animLengthDir.Add("attackXXO", 1.333f);
            animLengthDir.Add("attackXXOX", 1.5f);
            animLengthDir.Add("attackXXOXX", 1.433f);

            animLengthDir.Add("attackOOX", 1.233f);
            animLengthDir.Add("attackOOXO", 1.5f);
            animLengthDir.Add("attackOOXOO", 1.667f);
            animLengthDir.Add("attackOX", 1.167f);
            animLengthDir.Add("attackOXO", 1.267f);
            animLengthDir.Add("attackOXOX", 1.067f);
            animLengthDir.Add("attackOXOXO", 1.2f);

        }

        public FP GetAnimLength(string name)
        {

            return this.animLengthDir[name];
        }

        public bool IsPlaying(string name)
        {
            if (this.rendererProxy != null)
            {
                return this.rendererProxy.IsPlaying(name);
            }
            return false;
        }

        public void CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
        {
            if (this.rendererProxy != null)
            {
                this.rendererProxy.CrossFadeQueued(animation,fadeLength,queue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength">millisecond</param>
        public void CrossFade(string animation, float fadeLength)
        {
            if (this.rendererProxy != null)
            {
                this.rendererProxy.CrossFade(animation,fadeLength);
            }
        }

        public void Rewind()
        {
            if (this.rendererProxy != null)
            {
                this.rendererProxy.RewindAnim();
            }
        }

        public void Stop()
        {
            if (this.rendererProxy != null)
            {
                this.rendererProxy.StopAnim();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="speed">使用浮点数不会对结果造成影响</param>
        public void SetAnimationStateSpeed(string name,float speed)
        {
        }
    }
}
