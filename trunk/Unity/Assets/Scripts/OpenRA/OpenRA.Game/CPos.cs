using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA
{
    public struct CPos : IEquatable<CPos>
    {
        public readonly int X, Y;
        public readonly byte Layer;

        public CPos(int x, int y) { X = x; Y = y; Layer = 0; }
        public CPos(int x, int y, byte layer) { X = x; Y = y; Layer = layer; }
        public static readonly CPos Zero = new CPos(0, 0, 0);

        public static explicit operator CPos(int2 a) { return new CPos(a.X, a.Y); }

        public static CPos operator +(CVec a, CPos b) { return new CPos(a.X + b.X, a.Y + b.Y, b.Layer); }
        public static CPos operator +(CPos a, CVec b) { return new CPos(a.X + b.X, a.Y + b.Y, a.Layer); }
        public static CPos operator -(CPos a, CVec b) { return new CPos(a.X - b.X, a.Y - b.Y, a.Layer); }
        public static CVec operator -(CPos a, CPos b) { return new CVec(a.X - b.X, a.Y - b.Y); }

        public static bool operator ==(CPos me, CPos other) { return me.X == other.X && me.Y == other.Y && me.Layer == other.Layer; }
        public static bool operator !=(CPos me, CPos other) { return !(me == other); }

        public override int GetHashCode() { return X.GetHashCode() ^ Y.GetHashCode() ^ Layer.GetHashCode(); }

        public bool Equals(CPos other) { return X == other.X && Y == other.Y && Layer == other.Layer; }
        public override bool Equals(object obj) { return obj is CPos && Equals((CPos)obj); }

        public override string ToString() { return X + "," + Y; }

        public MPos ToMPos(Map map)
        {
            return ToMPos(map.Grid.Type);
        }

        public MPos ToMPos(MapGridType gridType)
        {
            if (gridType == MapGridType.Rectangular)
                return new MPos(X, Y);

            // Convert from RectangularIsometric cell (x, y) position to rectangular map position (u, v)
            //  - The staggered rows make this fiddly (hint: draw a diagram!)
            // (a) Consider the relationships:
            //  - +1x (even -> odd) adds (0, 1) to (u, v)
            //  - +1x (odd -> even) adds (1, 1) to (u, v)
            //  - +1y (even -> odd) adds (-1, 1) to (u, v)
            //  - +1y (odd -> even) adds (0, 1) to (u, v)
            // (b) Therefore:
            //  - ax + by adds (a - b)/2 to u (only even increments count)
            //  - ax + by adds a + b to v
            var u = (X - Y) / 2;
            var v = X + Y;
            return new MPos(u, v);
        }
    }
}
