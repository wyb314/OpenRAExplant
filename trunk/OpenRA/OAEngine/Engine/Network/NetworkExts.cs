using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network
{
    public static class NetworkExts
    {
        public static string F(this string fmt, params object[] args)
        {
            return string.Format(fmt, args);
        }

        public static T MaxBy<T, U>(this IEnumerable<T> ts, Func<T, U> selector)
        {
            return ts.CompareBy(selector, -1, true);
        }

        static T CompareBy<T, U>(this IEnumerable<T> ts, Func<T, U> selector, int modifier, bool throws)
        {
            var comparer = Comparer<U>.Default;
            T t;
            U u;
            using (var e = ts.GetEnumerator())
            {
                if (!e.MoveNext())
                    if (throws)
                        throw new ArgumentException("Collection must not be empty.", "ts");
                    else
                        return default(T);
                t = e.Current;
                u = selector(t);
                while (e.MoveNext())
                {
                    var nextT = e.Current;
                    var nextU = selector(nextT);
                    if (comparer.Compare(nextU, u) * modifier < 0)
                    {
                        t = nextT;
                        u = nextU;
                    }
                }

                return t;
            }
        }
    }
}
