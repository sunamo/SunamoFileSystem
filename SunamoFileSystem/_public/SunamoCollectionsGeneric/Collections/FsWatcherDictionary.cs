namespace SunamoFileSystem._public.SunamoCollectionsGeneric.Collections;

/// <summary>
/// Represents the fs watcher dictionary t u type.
/// </summary>
public class FsWatcherDictionary<T, U> : IDictionary<T, U> where T : notnull
{
    private static Type Type = typeof(FsWatcherDictionary<T, U>);
    private readonly Dictionary<T, U> d = new();

    /// <summary>
    /// The this t.
    /// </summary>
    public U this[T key]
    {
        get
        {
            if (d.ContainsKey(key)) return d[key];
            return default!;
        }
        set => d[key] = value;
    }

    /// <summary>
    /// Gets the keys.
    /// </summary>
    public ICollection<T> Keys => d.Keys;

    /// <summary>
    /// Gets the values.
    /// </summary>
    public ICollection<U> Values => d.Values;

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => d.Count;

    /// <summary>
    /// Gets the is read only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Add.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(T key, U value)
    {
        lock (d)
        {
            if (!d.ContainsKey(key)) d.Add(key, value);
        }
    }

    /// <summary>
    /// Add.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The add result.</returns>
    public void Add(KeyValuePair<T, U> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Clear.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The clear result.</returns>
    public void Clear()
    {
        d.Clear();
    }

    /// <summary>
    /// Contains.
    /// </summary>
    /// <param name="!d.ContainsKey(key)">The d contains key key.</param>
    /// <returns>The contains result.</returns>
    public bool Contains(KeyValuePair<T, U> item)
    {
        return d.Contains(item);
    }

    /// <summary>
    /// Contains key.
    /// </summary>
    /// <param name="item">The item.</param>
    public bool ContainsKey(T key)
    {
        return d.ContainsKey(key);
    }

    /// <summary>
    /// Copy to.
    /// </summary>
    public void CopyTo(KeyValuePair<T, U>[] array, int arrayIndex)
    {
        ThrowEx.NotImplementedMethod();
    }

    /// <summary>
    /// Get enumerator.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The get enumerator result.</returns>
    public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
    {
        return d.GetEnumerator();
    }

    /// <summary>
    /// Remove.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The remove result.</returns>
    public bool Remove(T key)
    {
        return d.Remove(key);
    }

    /// <summary>
    /// Remove.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="arrayIndex">The array index.</param>
    public bool Remove(KeyValuePair<T, U> item)
    {
        return d.Remove(item.Key);
    }

    /// <summary>
    /// Try get value.
    /// </summary>
    /// <returns>The try get value result.</returns>
    public bool TryGetValue(T key, out U value)
    {
        var result = d.TryGetValue(key, out value!);
        return result;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return d.GetEnumerator();
    }
}
