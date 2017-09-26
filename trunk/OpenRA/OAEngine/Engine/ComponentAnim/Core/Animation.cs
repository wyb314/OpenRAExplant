using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.ComponentAnim.Core
{
    public enum QueueMode
    {
        CompleteOthers = 0,
        PlayNow = 2,
    }
    public class Animation
    {

        public AnimationState this[string name]
        {
            get
            {
                return this.GetState(name);
            }
        }

        public bool IsPlaying(string name)
        {
            return false;
        }

        public void CrossFadeQueued(string animation, float fadeLength, QueueMode queue)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="fadeLength">millisecond</param>
        public void CrossFade(string animation, int fadeLength)
        {
            
        }

        public void Rewind()
        {

        }

        public void Stop()
        {

        }
    }
}
