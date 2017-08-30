using System;
using System.Collections.Generic;
using OpenRA.Graphics;

namespace OpenRA.Effects
{
    public class AsyncAction : IEffect
    {
        Action a;
        IAsyncResult ar;

        public AsyncAction(IAsyncResult ar, Action a)
        {
            this.a = a;
            this.ar = ar;
        }

        public void Tick(World world)
        {
            if (ar.IsCompleted)
            {
                world.AddFrameEndTask(w => { w.Remove(this); a(); });
            }
        }

        public IEnumerable<IRenderable> Render(WorldRenderer r) { yield break; }
    }
}
