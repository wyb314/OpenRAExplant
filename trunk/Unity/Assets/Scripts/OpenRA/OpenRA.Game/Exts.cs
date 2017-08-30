using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using OpenRA.Support;

namespace OpenRA
{
    public static class Exts
    {
        public static string F(this string fmt, params object[] args)
        {
            return string.Format(fmt, args);
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

        public static IEnumerable<T> Append<T>(this IEnumerable<T> ts, params T[] moreTs)
        {
            return ts.Concat(moreTs);
        }

        /// <summary>
        /// .NET3.5 无此方法，故借助.NET 4.0的源代码扩展该方法
        /// </summary>
        /// <param name="destination"></param>
        public static void CopyTo(this Stream source,Stream destination)
        {
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (!source.CanRead && !source.CanWrite)
                throw new ObjectDisposedException(null);
            if (!destination.CanRead && !destination.CanWrite)
                throw new ObjectDisposedException("destination");
            if (!source.CanRead)
                throw new NotSupportedException("NotSupported_UnreadableStream");
            if (!destination.CanWrite)
                throw new NotSupportedException("NotSupported_UnwritableStream");


            byte[] buffer = new byte[4096];
            int read;
            while ((read = source.Read(buffer, 0, buffer.Length)) != 0)
                destination.Write(buffer, 0, read);
        }

        public static byte[] ReadBytes(this Stream s, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            var buffer = new byte[count];
            s.ReadBytes(buffer, 0, count);
            return buffer;
        }

        public static void ReadBytes(this Stream s, byte[] buffer, int offset, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
            while (count > 0)
            {
                int bytesRead;
                if ((bytesRead = s.Read(buffer, offset, count)) == 0)
                    throw new EndOfStreamException();
                offset += bytesRead;
                count -= bytesRead;
            }
        }


        public static string JoinWith<T>(this IEnumerable<T> ts, string j)
        {
            return Join(j, ts);
        }

        public static String Join<T>(String separator, IEnumerable<T> values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (separator == null)
                separator = String.Empty;

            using (IEnumerator<T> en = values.GetEnumerator())
            {
                if (!en.MoveNext())
                    return String.Empty;

                StringBuilder result = new StringBuilder();
                if (en.Current != null)
                {
                    // handle the case that the enumeration has null entries
                    // and the case where their ToString() override is broken
                    string value = en.Current.ToString();
                    if (value != null)
                        result.Append(value);
                }

                while (en.MoveNext())
                {
                    result.Append(separator);
                    if (en.Current != null)
                    {
                        // handle the case that the enumeration has null entries
                        // and the case where their ToString() override is broken
                        string value = en.Current.ToString();
                        if (value != null)
                            result.Append(value);
                    }
                }
                return result.ToString();
            }
        }

        public static Dictionary<TKey, TSource> ToDictionaryWithConflictLog<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
            string debugName, Func<TKey, string> logKey, Func<TSource, string> logValue)
        {
            return ToDictionaryWithConflictLog(source, keySelector, x => x, debugName, logKey, logValue);
        }

        public static Dictionary<TKey, TElement> ToDictionaryWithConflictLog<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector,
            string debugName, Func<TKey, string> logKey = null, Func<TElement, string> logValue = null)
        {
            // Fall back on ToString() if null functions are provided:
            logKey = logKey ?? (s => s.ToString());
            logValue = logValue ?? (s => s.ToString());

            // Try to build a dictionary and log all duplicates found (if any):
            var dupKeys = new Dictionary<TKey, List<string>>();
            var d = new Dictionary<TKey, TElement>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                var element = elementSelector(item);

                // Check for a key conflict:
                if (d.ContainsKey(key))
                {
                    List<string> dupKeyMessages;
                    if (!dupKeys.TryGetValue(key, out dupKeyMessages))
                    {
                        // Log the initial conflicting value already inserted:
                        dupKeyMessages = new List<string>();
                        dupKeyMessages.Add(logValue(d[key]));
                        dupKeys.Add(key, dupKeyMessages);
                    }

                    // Log this conflicting value:
                    dupKeyMessages.Add(logValue(element));
                    continue;
                }

                d.Add(key, element);
            }

            // If any duplicates were found, throw a descriptive error
            if (dupKeys.Count > 0)
            {
                var badKeysFormatted = Join(", ", dupKeys.Select(p => "{0}: [{1}]".F(logKey(p.Key), Join(",", p.Value))));
                var msg = "{0}, duplicate values found for the following keys: {1}".F(debugName, badKeysFormatted);
                throw new ArgumentException(msg);
            }

            // Return the dictionary we built:
            return d;
        }

        public static bool HasAttribute<T>(this MemberInfo mi)
        {
            return mi.GetCustomAttributes(typeof(T), true).Length != 0;
        }

        public static T[] GetCustomAttributes<T>(this MemberInfo mi, bool inherit)where T : class
        {
            return (T[])mi.GetCustomAttributes(typeof(T), inherit);
        }

        public static bool TryParseIntegerInvariant(string s, out int i)
        {
            return int.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out i);
        }

        public static T[] MakeArray<T>(int count, Func<int, T> f)
        {
            var result = new T[count];
            for (var i = 0; i < count; i++)
                result[i] = f(i);

            return result;
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

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
                return min;
            else if (val.CompareTo(max) > 0)
                return max;
            else
                return val;
        }


        public static IEnumerable<string> GetNamespaces(this Assembly a)
        {
            return a.GetTypes().Select(t => t.Namespace).Distinct().Where(n => n != null);
        }

        public static int ParseIntegerInvariant(string s)
        {
            return int.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
        }
        

        internal static ulong ToUInt64(Object value)
        {
            // Helper function to silently convert the value to UInt64 from the other base types for enum without throwing an exception.
            // This is need since the Convert functions do overflow checks.
            TypeCode typeCode = Convert.GetTypeCode(value);
            ulong result;

            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    result = (UInt64)Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    break;

                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                    break;

                default:
                    // All unsigned types will be directly cast 
                    throw new InvalidOperationException("InvalidOperation_UnknownEnumType");
            }
            return result;
        }

        public static bool TryParseInt64Invariant(string s, out long i)
        {
            return long.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out i);
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

        public static T MaxBy<T, U>(this IEnumerable<T> ts, Func<T, U> selector)
        {
            return ts.CompareBy(selector, -1, true);
        }

        public static class Enum<T>
        {
            public static T Parse(string s) { return (T)Enum.Parse(typeof(T), s); }
            public static T[] GetValues() { return (T[])Enum.GetValues(typeof(T)); }

            public static bool TryParse(string s, bool ignoreCase, out T value)
            {
                // The string may be a comma delimited list of values
                var names = ignoreCase ? Enum.GetNames(typeof(T)).Select(x => x.ToLowerInvariant()) : Enum.GetNames(typeof(T));
                var values = ignoreCase ? s.Split(',').Select(x => x.Trim().ToLowerInvariant()) : s.Split(',').Select(x => x.Trim());

                if (values.Any(x => !names.Contains(x)))
                {
                    value = default(T);
                    return false;
                }

                value = (T)Enum.Parse(typeof(T), s, ignoreCase);

                return true;
            }
        }


        public static int IntegerDivisionRoundingAwayFromZero(int dividend, int divisor)
        {
            int remainder;
            var quotient = Math.DivRem(dividend, divisor, out remainder);
            if (remainder == 0)
                return quotient;
            return quotient + (Math.Sign(dividend) == Math.Sign(divisor) ? 1 : -1);
        }

        public static bool Contains(this Rectangle r, int2 p)
        {
            return r.Contains(p.X,p.Y);
        }

        public static bool HasModifier(this Modifiers k, Modifiers mod)
        {
            // PERF: Enum.HasFlag is slower and requires allocations.
            return (k & mod) == mod;
        }

        public static T MaxByOrDefault<T, U>(this IEnumerable<T> ts, Func<T, U> selector)
        {
            return ts.CompareBy(selector, -1, false);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public interface IDisabledTrait { bool IsTraitDisabled { get; } }

        public static bool IsTraitEnabled(this object trait)
        {
            return trait as IDisabledTrait == null || !(trait as IDisabledTrait).IsTraitDisabled;
        }

        public static bool IsTraitEnabled<T>(T t)
        {
            return IsTraitEnabled(t as object);
        }

        public static T MinBy<T, U>(this IEnumerable<T> ts, Func<T, U> selector)
        {
            return ts.CompareBy(selector, 1, true);
        }

        public static T Random<T>(this IEnumerable<T> ts, MersenneTwister r)
        {
            return Random(ts, r, true);
        }

        static T Random<T>(IEnumerable<T> ts, MersenneTwister r, bool throws)
        {
            var xs = ts as ICollection<T>;
            xs = xs ?? ts.ToList();
            if (xs.Count == 0)
            {
                if (throws)
                    throw new ArgumentException("Collection must not be empty.", "ts");
                else
                    return default(T);
            }
            else
                return xs.ElementAt(r.Next(xs.Count));
        }

        public static T RandomOrDefault<T>(this IEnumerable<T> ts, MersenneTwister r)
        {
            return Random(ts, r, false);
        }

        public static int IndexOf<T>(this T[] array, T value)
        {
            return Array.IndexOf(array, value);
        }

        public static bool IsNullOrWhiteSpace(String value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }

        public static IEnumerable<T> Iterate<T>(this T t, Func<T, T> f)
        {
            for (;;) { yield return t; t = f(t); }
        }
    }
}
