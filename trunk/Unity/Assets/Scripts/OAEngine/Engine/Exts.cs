using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine
{
    public static class Exts
    {
        public static V GetOrAdd<K, V>(this Dictionary<K, V> d, K k)
            where V : new()
        {
            return d.GetOrAdd(k, _ => new V());
        }

        public static V GetOrAdd<K, V>(this Dictionary<K, V> d, K k, Func<K, V> createFn)
        {
            V ret;
            if (!d.TryGetValue(k, out ret))
                d.Add(k, ret = createFn(k));
            return ret;
        }

        public static bool HasAttribute<T>(this MemberInfo mi)
        {
            return mi.GetCustomAttributes(typeof(T), true).Length != 0;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> ts, T t)
        {
            return ts.Except(new[] { t });
        }

        public static T MinByOrDefault<T, U>(this IEnumerable<T> ts, Func<T, U> selector)
        {
            return ts.CompareBy(selector, 1, false);
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

        public static IEnumerable<string> GetNamespaces(this Assembly a)
        {
            return a.GetTypes().Select(t => t.Namespace).Distinct().Where(n => n != null);
        }
    }
}
