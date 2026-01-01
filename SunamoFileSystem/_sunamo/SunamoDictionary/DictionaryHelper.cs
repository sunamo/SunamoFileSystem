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
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="sl"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
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
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <typeparam name="ColType"></typeparam>
    /// <param name="sl"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    internal static void AddOrCreate<Key, Value, ColType>(IDictionary<Key, List<Value>> dict, Key key, Value value,
        bool withoutDuplicitiesInValue = false, Dictionary<Key, List<string>>? stringDict = null) where Key : notnull
    {
        var compWithString = false;
        if (stringDict != null) compWithString = true;

        if (key is IList && typeof(ColType) != typeof(object))
        {
            var keyAsList = key as IList<ColType>;
            var contains = false;
            foreach (var item in dict)
            {
                var dictionaryKeyAsList = item.Key as IList<ColType>;
                if (dictionaryKeyAsList.SequenceEqual(keyAsList)) contains = true;
            }

            if (contains)
            {
                foreach (var item in dict)
                {
                    var dictionaryKeyAsList = item.Key as IList<ColType>;
                    if (dictionaryKeyAsList.SequenceEqual(keyAsList))
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
                dict.Add(key, valueList);

                if (compWithString)
                {
                    List<string> stringList = new();
                    stringList.Add(value.ToString());
                    stringDict.Add(key, stringList);
                }
            }
        }
        else
        {
            var add = true;
            lock (dict)
            {
                if (dict.ContainsKey(key))
                {
                    if (withoutDuplicitiesInValue)
                    {
                        if (dict[key].Contains(value))
                            add = false;
                        else if (compWithString)
                            if (stringDict[key].Contains(value.ToString()))
                                add = false;
                    }

                    if (add)
                    {
                        var existingValues = dict[key];

                        if (existingValues != null) existingValues.Add(value);

                        if (compWithString)
                        {
                            var existingStringValues = stringDict[key];

                            if (existingValues != null) existingStringValues.Add(value.ToString());
                        }
                    }
                }
                else
                {
                    if (!dict.ContainsKey(key))
                    {
                        List<Value> valueList = new();
                        valueList.Add(value);
                        dict.Add(key, valueList);
                    }
                    else
                    {
                        dict[key].Add(value);
                    }

                    if (compWithString)
                    {
                        if (!stringDict.ContainsKey(key))
                        {
                            List<string> stringList = new();
                            stringList.Add(value.ToString());
                            stringDict.Add(key, stringList);
                        }
                        else
                        {
                            stringDict[key].Add(value.ToString());
                        }
                    }
                }
            }
        }
    }

    internal static void AddOrCreate<Key, Value>(IDictionary<Key, List<Value>> dict, Key key, Value value,
        bool withoutDuplicitiesInValue = false, Dictionary<Key, List<string>>? stringDict = null) where Key : notnull
    {
        AddOrCreate<Key, Value, object>(dict, key, value, withoutDuplicitiesInValue, stringDict);
    }

    #endregion
}