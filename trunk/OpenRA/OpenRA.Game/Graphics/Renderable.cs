using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Graphics
{
    public interface IRenderable
    {
        WPos Pos { get; }
        //PaletteReference Palette { get; }
        int ZOffset { get; }
        bool IsDecoration { get; }

        //IRenderable WithPalette(PaletteReference newPalette);
        IRenderable WithZOffset(int newOffset);
        IRenderable OffsetBy(WVec offset);
        IRenderable AsDecoration();

        //IFinalizedRenderable PrepareRender(WorldRenderer wr);
    }
}
