namespace SunamoFileSystem._sunamo.SunamoDictionary;

// potřebuji celý SunamoDictionary nuget kvůli genericitě
internal class DictionaryHelper
{
    internal static void AddOrSet<T1, T2>(IDictionary<T1, T2> dictionary, T1 key, T2 value) where T1 : notnull
    {
        if (dictionary.ContainsKey(key))
            dictionary[key] = value;
        else
            dictionary.Add(key, value);
    }


    internal static Dictionary<T, List<U>> GroupByValues<U, T, ColType>(Dictionary<U, T> dictionary) where T : notnull where U : notnull
    {
        var result = new Dictionary<T, List<U>>();
        foreach (var item in dictionary) AddOrCreate<T, U, ColType>(result, item.Value, item.Key);

        return result;
    }

    /// <summary>
    ///     In addition to method AddOrCreate, more is checking whether value in collection does not exists
    /// </summary>
    /// <typeparam name="Key">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="Value">The type of values in the lists.</typeparam>
    /// <param name="dictionary">The dictionary to add to.</param>
    /// <param name="key">The key to add or update.</param>
    /// <param name="value">The value to add if not already present.</param>
    internal static void AddOrCreateIfDontExists<Key, Value>(Dictionary<Key, List<Value>> dictionary, Key key, Value value) where Key : notnull
    {
        if (dictionary.ContainsKey(key))
        {
            if (!dictionary[key].Contains(value)) dictionary[key].Add(value);
        }
        else
        {
            var valueList = new List<Value>();
            valueList.Add(value);
            dictionary.Add(key, valueList);
        }
    }

    #region AddOrCreate

    /// <summary>
    ///     A3 is inner type of collection entries
    ///     dictS => is comparing with string
    ///     As inner must be List, not IList etc.
    ///     From outside is not possible as inner use other class based on IList
    /// </summary>
    /// <typeparam name="Key">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="Value">The type of values in the lists.</typeparam>
    /// <typeparam name="ColType">The inner type of collection entries for sequence comparison.</typeparam>
    /// <param name="dictionary">The dictionary to add to or create entry in.</param>
    /// <param name="key">The key to add or update.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="withoutDuplicitiesInValue">Whether to prevent duplicate values in the list.</param>
    /// <param name="stringDict">Optional parallel string dictionary for string comparison.</param>
    internal static void AddOrCreate<Key, Value, ColType>(IDictionary<Key, List<Value>> dictionary, Key key, Value value,
        bool withoutDuplicitiesInValue = false, Dictionary<Key, List<string>>? stringDict = null) where Key : notnull
    {
        var compWithString = false;
        if (stringDict != null) compWithString = true;

        if (key is IList && typeof(ColType) != typeof(object))
        {
            var keyAsList = key as IList<ColType>;
            var contains = false;
            foreach (var item in dictionary)
            {
                var dictionaryKeyAsList = item.Key as IList<ColType>;
                if (dictionaryKeyAsList!.SequenceEqual(keyAsList!)) contains = true;
            }

            if (contains)
            {
                foreach (var item in dictionary)
                {
                    var dictionaryKeyAsList = item.Key as IList<ColType>;
                    if (dictionaryKeyAsList!.SequenceEqual(keyAsList!))
                    {
                        if (withoutDuplicitiesInValue)
                            if (item.Value.Contains(value))
                                return;
                        item.Value.Add(value);
                    }
                }
            }
            else
            {
                List<Value> valueList = new();
                valueList.Add(value);
                dictionary.Add(key, valueList);

                if (compWithString)
                {
                    List<string> stringList = new();
                    stringList.Add(value!.ToString()!);
                    stringDict!.Add(key, stringList);
                }
            }
        }
        else
        {
            var add = true;
            lock (dictionary)
            {
                if (dictionary.ContainsKey(key))
                {
                    if (withoutDuplicitiesInValue)
                    {
                        if (dictionary[key].Contains(value))
                            add = false;
                        else if (compWithString)
                            if (stringDict![key].Contains(value!.ToString()!))
                                add = false;
                    }

                    if (add)
                    {
                        var existingValues = dictionary[key];

                        if (existingValues != null) existingValues.Add(value);

                        if (compWithString)
                        {
                            var existingStringValues = stringDict![key];

                            if (existingValues != null) existingStringValues!.Add(value!.ToString()!);
                        }
                    }
                }
                else
                {
                    if (!dictionary.ContainsKey(key))
                    {
                        List<Value> valueList = new();
                        valueList.Add(value);
                        dictionary.Add(key, valueList);
                    }
                    else
                    {
                        dictionary[key].Add(value);
                    }

                    if (compWithString)
                    {
                        if (!stringDict!.ContainsKey(key))
                        {
                            List<string> stringList = new();
                            stringList.Add(value!.ToString()!);
                            stringDict.Add(key, stringList);
                        }
                        else
                        {
                            stringDict[key].Add(value!.ToString()!);
                        }
                    }
                }
            }
        }
    }

    internal static void AddOrCreate<Key, Value>(IDictionary<Key, List<Value>> dictionary, Key key, Value value,
        bool withoutDuplicitiesInValue = false, Dictionary<Key, List<string>>? stringDict = null) where Key : notnull
    {
        AddOrCreate<Key, Value, object>(dictionary, key, value, withoutDuplicitiesInValue, stringDict);
    }

    #endregion
}