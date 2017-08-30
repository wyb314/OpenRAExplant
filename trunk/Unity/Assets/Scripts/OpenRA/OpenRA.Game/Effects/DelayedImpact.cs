using System;
using System.Collections.Generic;
using OpenRA.Graphics;
using OpenRA.Traits;

namespace OpenRA.Effects
{
    public class DelayedImpact : IEffect
    {
        readonly Target target;
        readonly Actor firedBy;
        readonly IEnumerable<int> damageModifiers;
        readonly IWarhead wh;

        int delay;

        public DelayedImpact(int delay, IWarhead wh, Target target, Actor firedBy, IEnumerable<int> damageModifiers)
        {
            this.wh = wh;
            this.delay = delay;

            this.target = target;
            this.firedBy = firedBy;
            this.damageModifiers = damageModifiers;
        }

        public void Tick(World world)
        {
            if (--delay <= 0)
                world.AddFrameEndTask(w => { w.Remove(this); wh.DoImpact(target, firedBy, damageModifiers); });
        }

        public IEnumerable<IRenderable> Render(WorldRenderer wr) { yield break; }
    }
}
