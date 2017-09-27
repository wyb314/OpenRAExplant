

using System.Security.Cryptography.X509Certificates;
using Engine.ComponentAnim.Core;

namespace Engine.Interfaces
{
    public interface IRender
    {
        void Render(Actor self, IWorldRenderer wr);

        bool IsPlaying(string name);

        void CrossFadeQueued(string animation, float fadeLength, QueueMode queue);
        
        void CrossFade(string animation, float fadeLength);

        void RewindAnim();

        void StopAnim();

        void SetAnimationStateSpeed(string name, float speed);
    }
}
