namespace SunamoFileSystem._public.SunamoCollectionsGeneric.Collections;

/// <summary>
/// Thread-safe dictionary wrapper for file system watcher operations that prevents duplicate key additions.
/// </summary>
/// <typeparam name="T">The type of keys in the dictionary.</typeparam>
/// <typeparam name="U">The type of values in the dictionary.</typeparam>
public class FsWatcherDictionary<T, U> : IDictionary<T, U> where T : notnull
{
    private static Type Type = typeof(FsWatcherDictionary<T, U>);
    private readonly Dictionary<T, U> d = new();

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// Returns default if the key does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key, or default if not found.</returns>
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
    /// Gets a collection containing the keys of the dictionary.
    /// </summary>
    public ICollection<T> Keys => d.Keys;

    /// <summary>
    /// Gets a collection containing the values of the dictionary.
    /// </summary>
    public ICollection<U> Values => d.Values;

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    public int Count => d.Count;

    /// <summary>
    /// Gets a value indicating whether the dictionary is read-only. Always returns false.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds the specified key and value to the dictionary in a thread-safe manner.
    /// If the key already exists, the value is not updated.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add.</param>
    public void Add(T key, U value)
    {
        lock (d)
        {
            if (!d.ContainsKey(key)) d.Add(key, value);
        }
    }

    /// <summary>
    /// Adds the specified key/value pair to the dictionary.
    /// </summary>
    /// <param name="item">The key/value pair to add.</param>
    public void Add(KeyValuePair<T, U> item)
    {
        Add(item.Key, item.Value);
    }

    /// <summary>
    /// Removes all keys and values from the dictionary.
    /// </summary>
    public void Clear()
    {
        d.Clear();
    }

    /// <summary>
    /// Determines whether the dictionary contains the specified key/value pair.
    /// </summary>
    /// <param name="item">The key/value pair to locate.</param>
    /// <returns>True if the key/value pair is found; otherwise, false.</returns>
    public bool Contains(KeyValuePair<T, U> item)
    {
        return d.Contains(item);
    }

    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>True if the dictionary contains the key; otherwise, false.</returns>
    public bool ContainsKey(T key)
    {
        return d.ContainsKey(key);
    }

    /// <summary>
    /// Copies the dictionary elements to an array. Not implemented.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index at which copying begins.</param>
    public void CopyTo(KeyValuePair<T, U>[] array, int arrayIndex)
    {
        ThrowEx.NotImplementedMethod();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the dictionary.
    /// </summary>
    /// <returns>An enumerator for the dictionary.</returns>
    public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
    {
        return d.GetEnumerator();
    }

    /// <summary>
    /// Removes the value with the specified key from the dictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>True if the element was successfully removed; otherwise, false.</returns>
    public bool Remove(T key)
    {
        return d.Remove(key);
    }

    /// <summary>
    /// Removes the specified key/value pair from the dictionary.
    /// </summary>
    /// <param name="item">The key/value pair to remove.</param>
    /// <returns>True if the element was successfully removed; otherwise, false.</returns>
    public bool Remove(KeyValuePair<T, U> item)
    {
        return d.Remove(item.Key);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if found.</param>
    /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
    public bool TryGetValue(T key, out U value)
    {
        var result = d.TryGetValue(key, out value!);
        return result;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the dictionary.
    /// </summary>
    /// <returns>An enumerator for the dictionary.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return d.GetEnumerator();
    }
}