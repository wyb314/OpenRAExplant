using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenRA.Graphics;

namespace OpenRA
{
    public interface IOrderGenerator
    {
        IEnumerable<Order> Order(World world, CPos cell, int2 worldPixel, MouseInput mi);
        void Tick(World world);
        IEnumerable<IRenderable> Render(WorldRenderer wr, World world);
        IEnumerable<IRenderable> RenderAboveShroud(WorldRenderer wr, World world);
        string GetCursor(World world, CPos cell, int2 worldPixel, MouseInput mi);
    }
}
