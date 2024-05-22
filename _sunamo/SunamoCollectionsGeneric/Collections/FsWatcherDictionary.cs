namespace SunamoFileSystem;


internal class FsWatcherDictionary<T, U> : IDictionary<T, U>
{
    static Type type = typeof(FsWatcherDictionary<T, U>);
    Dictionary<T, U> d = new Dictionary<T, U>();
    internal U this[T key]
    {
        get
        {
            if (d.ContainsKey(key))
            {
                return d[key];
            }
            return default;
        }
        set
        {
            d[key] = value;
        }
    }
    internal ICollection<T> Keys => d.Keys;
    internal ICollection<U> Values => d.Values;
    internal int Count => d.Count;
    internal bool IsReadOnly => false;
    internal void Add(T key, U value)
    {
        lock (d)
        {
            if (!d.ContainsKey(key))
            {
                d.Add(key, value);
            }
        }
    }
    internal void Add(KeyValuePair<T, U> item)
    {
        Add(item.Key, item.Value);
    }
    internal void Clear()
    {
        d.Clear();
    }
    internal bool Contains(KeyValuePair<T, U> item)
    {
        return d.Contains(item);
    }
    internal bool ContainsKey(T key)
    {
        return d.ContainsKey(key);
    }
    internal void CopyTo(KeyValuePair<T, U>[] array, int arrayIndex)
    {
        ThrowEx.NotImplementedMethod();
        //DictionaryHelper.CopyTo<T, U>(d, array, arrayIndex);
    }
    internal IEnumerator<KeyValuePair<T, U>> GetEnumerator()
    {
        return d.GetEnumerator();
    }
    internal bool Remove(T key)
    {
        return d.Remove(key);
    }
    internal bool Remove(KeyValuePair<T, U> item)
    {
        return d.Remove(item.Key);
    }
    internal bool TryGetValue(T key, out U value)
    {
        bool vr = d.TryGetValue(key, out value);
        return vr;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return d.GetEnumerator();
    }
}