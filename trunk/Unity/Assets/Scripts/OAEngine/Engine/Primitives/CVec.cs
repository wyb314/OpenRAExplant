using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Primitives
{
    public struct CVec : IEquatable<CVec>
    {
        public readonly int X, Y;

        public CVec(int x, int y) { X = x; Y = y; }
        public static readonly CVec Zero = new CVec(0, 0);

        public static CVec operator +(CVec a, CVec b) { return new CVec(a.X + b.X, a.Y + b.Y); }
        public static CVec operator -(CVec a, CVec b) { return new CVec(a.X - b.X, a.Y - b.Y); }
        public static CVec operator *(int a, CVec b) { return new CVec(a * b.X, a * b.Y); }
        public static CVec operator *(CVec b, int a) { return new CVec(a * b.X, a * b.Y); }
        public static CVec operator /(CVec a, int b) { return new CVec(a.X / b, a.Y / b); }

        public static CVec operator -(CVec a) { return new CVec(-a.X, -a.Y); }

        public static bool operator ==(CVec me, CVec other) { return me.X == other.X && me.Y == other.Y; }
        public static bool operator !=(CVec me, CVec other) { return !(me == other); }

        public static CVec Max(CVec a, CVec b) { return new CVec(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y)); }
        public static CVec Min(CVec a, CVec b) { return new CVec(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y)); }

        public static int Dot(CVec a, CVec b) { return a.X * b.X + a.Y * b.Y; }

        public CVec Sign() { return new CVec(Math.Sign(X), Math.Sign(Y)); }
        public CVec Abs() { return new CVec(Math.Abs(X), Math.Abs(Y)); }
        public int LengthSquared { get { return X * X + Y * Y; } }
        public int Length { get { return Exts.ISqrt(LengthSquared); } }
        

        public override int GetHashCode() { return X.GetHashCode() ^ Y.GetHashCode(); }

        public bool Equals(CVec other) { return other == this; }
        public override bool Equals(object obj) { return obj is CVec && Equals((CVec)obj); }

        public override string ToString() { return X + "," + Y; }

        public static readonly CVec[] Directions =
        {
            new CVec(-1, -1),
            new CVec(-1,  0),
            new CVec(-1,  1),
            new CVec(0, -1),
            new CVec(0,  1),
            new CVec(1, -1),
            new CVec(1,  0),
            new CVec(1,  1),
        };
    }
}
