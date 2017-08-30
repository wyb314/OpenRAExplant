using System;
using System.Collections.Generic;
using System.Linq;


namespace OpenRA.Graphics
{
    [Flags]
    public enum ScrollDirection { None = 0, Up = 1, Left = 2, Down = 4, Right = 8 }

    public static class ViewportExts
    {
        public static bool Includes(this ScrollDirection d, ScrollDirection s)
        {
            // PERF: Enum.HasFlag is slower and requires allocations.
            return (d & s) == s;
        }

        public static ScrollDirection Set(this ScrollDirection d, ScrollDirection s, bool val)
        {
            return (d.Includes(s) != val) ? d ^ s : d;
        }
    }

    public class Viewport
    {
        readonly WorldRenderer worldRenderer;

        // Map bounds (world-px)
        readonly Rectangle mapBounds;
        readonly Size tileSize;

        // Viewport geometry (world-px)
        public int2 CenterLocation { get; private set; }

        public void Center(IEnumerable<Actor> actors)
        {
            if (!actors.Any())
                return;

            //Center(actors.Select(a => a.CenterPosition).Average());
        }

        public int2 ViewToWorldPx(int2 view)
        {

            //return (1f / Zoom * view.ToFloat2()).ToInt2() + TopLeft;
            return new int2();

        }
    }
}
