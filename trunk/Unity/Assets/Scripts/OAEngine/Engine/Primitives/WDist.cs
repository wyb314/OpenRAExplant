using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Support;

namespace Engine.Primitives
{
    public struct WDist : IComparable, IComparable<WDist>, IEquatable<WDist>
    {
        public readonly int Length;
        public long LengthSquared { get { return (long)Length * Length; } }

        public WDist(int r) { Length = r; }
        public static readonly WDist Zero = new WDist(0);
        public static readonly WDist MaxValue = new WDist(int.MaxValue);
        public static WDist FromCells(int cells) { return new WDist(1024 * cells); }

        public static WDist operator +(WDist a, WDist b) { return new WDist(a.Length + b.Length); }
        public static WDist operator -(WDist a, WDist b) { return new WDist(a.Length - b.Length); }
        public static WDist operator -(WDist a) { return new WDist(-a.Length); }
        public static WDist operator /(WDist a, int b) { return new WDist(a.Length / b); }
        public static WDist operator *(WDist a, int b) { return new WDist(a.Length * b); }
        public static WDist operator *(int a, WDist b) { return new WDist(a * b.Length); }
        public static bool operator <(WDist a, WDist b) { return a.Length < b.Length; }
        public static bool operator >(WDist a, WDist b) { return a.Length > b.Length; }
        public static bool operator <=(WDist a, WDist b) { return a.Length <= b.Length; }
        public static bool operator >=(WDist a, WDist b) { return a.Length >= b.Length; }

        public static bool operator ==(WDist me, WDist other) { return me.Length == other.Length; }
        public static bool operator !=(WDist me, WDist other) { return !(me == other); }

        // Sampled a N-sample probability density function in the range [-1024..1024]
        // 1 sample produces a rectangular probability
        // 2 samples produces a triangular probability
        // ...
        // N samples approximates a true Gaussian
        public static WDist FromPDF(MersenneTwister r, int samples)
        {
            return new WDist(Exts.MakeArray(samples, _ => r.Next(-1024, 1024))
                .Sum() / samples);
        }

        public static bool TryParse(string s, out WDist result)
        {
            result = Zero;

            if (string.IsNullOrEmpty(s))
                return false;

            s = s.ToLowerInvariant();
            var components = s.Split('c');
            var cell = 0;
            var subcell = 0;

            switch (components.Length)
            {
                case 2:
                    if (!Exts.TryParseIntegerInvariant(components[0], out cell) ||
                        !Exts.TryParseIntegerInvariant(components[1], out subcell))
                        return false;
                    break;
                case 1:
                    if (!Exts.TryParseIntegerInvariant(components[0], out subcell))
                        return false;
                    break;
                default: return false;
            }

            // Propagate sign to fractional part
            if (cell < 0)
                subcell = -subcell;

            result = new WDist(1024 * cell + subcell);
            return true;
        }

        public override int GetHashCode() { return Length.GetHashCode(); }

        public bool Equals(WDist other) { return other == this; }
        public override bool Equals(object obj) { return obj is WDist && Equals((WDist)obj); }

        public int CompareTo(object obj)
        {
            if (!(obj is WDist))
                return 1;
            return Length.CompareTo(((WDist)obj).Length);
        }

        public int CompareTo(WDist other) { return Length.CompareTo(other.Length); }

        public override string ToString()
        {
            var absLength = Math.Abs(Length);
            var absValue = (absLength / 1024).ToString() + "c" + (absLength % 1024).ToString();
            return Length < 0 ? "-" + absValue : absValue;
        }
    }
}
