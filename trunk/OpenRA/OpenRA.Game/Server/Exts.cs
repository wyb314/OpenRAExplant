using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Server
{
    static class Exts
    {
        public static IEnumerable<T> Except<T>(this IEnumerable<T> ts, T t)
        {
            return ts.Except(new[] { t });
        }
    }
}
