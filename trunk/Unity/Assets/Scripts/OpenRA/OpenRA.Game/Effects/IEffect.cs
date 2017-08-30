using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRA.Graphics;

namespace OpenRA.Effects
{
    public interface IEffect
    {
        void Tick(World world);
        IEnumerable<IRenderable> Render(WorldRenderer r);
    }

    public interface IEffectAboveShroud { IEnumerable<IRenderable> RenderAboveShroud(WorldRenderer wr); }
}
