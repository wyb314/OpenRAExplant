using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Primitives
{
    public struct CPos: IEquatable<CPos>
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
    }
}
