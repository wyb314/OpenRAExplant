using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Primitives
{
    public interface IReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        int Count { get; }
        TValue this[TKey key] { get; }
        ICollection<TKey> Keys { get; }
        ICollection<TValue> Values { get; }

        bool ContainsKey(TKey key);
        bool TryGetValue(TKey key, out TValue value);
    }


    public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> dict;

        public ReadOnlyDictionary()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dict)
        {
            if (dict == null)
                throw new ArgumentNullException("dict");

            this.dict = dict;
        }

        #region IReadOnlyDictionary implementation
        public bool ContainsKey(TKey key)
        {
            return dict.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dict.TryGetValue(key, out value);
        }

        public int Count { get { return dict.Count; } }

        public TValue this[TKey key] { get { return dict[key]; } }

        public ICollection<TKey> Keys { get { return dict.Keys; } }

        public ICollection<TValue> Values { get { return dict.Values; } }
        #endregion

        #region IEnumerable implementation
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        #endregion

        #region IEnumerable implementation
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        #endregion
    }

    public static class ReadOnlyDictionary
    {
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict as IReadOnlyDictionary<TKey, TValue> ?? new ReadOnlyDictionary<TKey, TValue>(dict);
        }
    }
}
