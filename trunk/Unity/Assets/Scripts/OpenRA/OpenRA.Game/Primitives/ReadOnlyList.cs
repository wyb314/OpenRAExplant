using System;
using System.Collections.Generic;

namespace OpenRA
{
    public interface IReadOnlyList<T> : IEnumerable<T>
    {
        int Count { get; }
        T this[int index] { get; }
    }

    public static class ReadOnlyList
    {
        public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
        {
            return list as IReadOnlyList<T> ?? new ReadOnlyList<T>(list);
        }
    }

    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> list;

        public ReadOnlyList()
            : this(new List<T>())
        {
        }

        public ReadOnlyList(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            this.list = list;
        }

        #region IEnumerable implementation
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
        #endregion

        #region IReadOnlyList implementation
        public int Count { get { return list.Count; } }

        public T this[int index] { get { return list[index]; } }
        #endregion
    }
}
