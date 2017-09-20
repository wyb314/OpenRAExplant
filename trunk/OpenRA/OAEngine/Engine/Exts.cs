using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Network;

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

        public enum ISqrtRoundMode { Floor, Nearest, Ceiling }
        public static int ISqrt(int number, ISqrtRoundMode round = ISqrtRoundMode.Floor)
        {
            if (number < 0)
                throw new InvalidOperationException("Attempted to calculate the square root of a negative integer: {0}".F(number));

            return (int)ISqrt((uint)number, round);
        }

        public static uint ISqrt(uint number, ISqrtRoundMode round = ISqrtRoundMode.Floor)
        {
            var divisor = 1U << 30;

            var root = 0U;
            var remainder = number;

            // Find the highest term in the divisor
            while (divisor > number)
                divisor >>= 2;

            // Evaluate the root, two bits at a time
            while (divisor != 0)
            {
                if (root + divisor <= remainder)
                {
                    remainder -= root + divisor;
                    root += 2 * divisor;
                }

                root >>= 1;
                divisor >>= 2;
            }

            // Adjust for other rounding modes
            if (round == ISqrtRoundMode.Nearest && remainder > root)
                root += 1;
            else if (round == ISqrtRoundMode.Ceiling && root * root < number)
                root += 1;

            return root;
        }

        public static long ISqrt(long number, ISqrtRoundMode round = ISqrtRoundMode.Floor)
        {
            if (number < 0)
                throw new InvalidOperationException("Attempted to calculate the square root of a negative integer: {0}".F(number));

            return (long)ISqrt((ulong)number, round);
        }

        public static ulong ISqrt(ulong number, ISqrtRoundMode round = ISqrtRoundMode.Floor)
        {
            var divisor = 1UL << 62;

            var root = 0UL;
            var remainder = number;

            // Find the highest term in the divisor
            while (divisor > number)
                divisor >>= 2;

            // Evaluate the root, two bits at a time
            while (divisor != 0)
            {
                if (root + divisor <= remainder)
                {
                    remainder -= root + divisor;
                    root += 2 * divisor;
                }

                root >>= 1;
                divisor >>= 2;
            }

            // Adjust for other rounding modes
            if (round == ISqrtRoundMode.Nearest && remainder > root)
                root += 1;
            else if (round == ISqrtRoundMode.Ceiling && root * root < number)
                root += 1;

            return root;
        }

    }
}
