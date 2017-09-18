using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static bool TryParseInt64Invariant(string s, out long i)
        {
            return long.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out i);
        }

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
}
