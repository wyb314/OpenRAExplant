using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.ComponentsAI.AStarMachine;
using Engine.Interfaces;

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

        public Animation(IRender rendererProxy)
        {
            this.rendererProxy = rendererProxy;
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
