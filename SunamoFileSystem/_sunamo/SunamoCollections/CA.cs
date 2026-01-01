namespace SunamoFileSystem._sunamo.SunamoCollections;

/// <summary>
/// Collection Array helper methods
/// </summary>
internal class CA
{
    /// <summary>
    /// Initializes and fills a list with default values
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="list">The list to fill</param>
    /// <param name="count">Number of default elements to add</param>
    internal static void InitFillWith<T>(List<T> list, int count)
    {
        for (var i = 0; i < count; i++) list.Add(default);
    }

    internal enum SearchStrategyCA
    {
        FixedSpace,
        AnySpaces,
        ExactlyName
    }

    /// <summary>
    /// Returns indexes of terms that are contained in the text
    /// </summary>
    /// <param name="text">The text to search in</param>
    /// <param name="terms">The terms to search for</param>
    /// <returns>List of indexes where terms were found</returns>
    internal static List<int> ReturnWhichContainsIndexes(string text, IList<string> terms)
    {
        var result = new List<int>();
        var currentIndex = 0;
        foreach (var term in terms)
        {
            if (text.Contains(term)) result.Add(currentIndex);
            currentIndex++;
        }

        return result;
    }

    /// <summary>
    /// Removes items from list that contain the specified pattern
    /// Direct editor - modifies the list in place
    /// </summary>
    /// <param name="list">The list to modify</param>
    /// <param name="pattern">The pattern to search for</param>
    /// <param name="isWildcard">Whether to use wildcard matching</param>
    /// <param name="wildcardIsMatch">Wildcard matching function</param>
    internal static void RemoveWhichContains(List<string> list, string pattern, bool isWildcard,
        Func<string, string, bool> wildcardIsMatch)
    {
        if (isWildcard)
        {
            for (var i = list.Count - 1; i >= 0; i--)
                if (wildcardIsMatch(list[i], pattern))
                    list.RemoveAt(i);
        }
        else
        {
            for (var i = list.Count - 1; i >= 0; i--)
                if (list[i].Contains(pattern))
                    list.RemoveAt(i);
        }
    }

    /// <summary>
    /// Removes items from list that contain any of the specified patterns
    /// </summary>
    /// <param name="list">The list to modify</param>
    /// <param name="patterns">The patterns to search for</param>
    /// <param name="isWildcard">Whether to use wildcard matching</param>
    /// <param name="wildcardIsMatch">Wildcard matching function</param>
    internal static void RemoveWhichContainsList(List<string> list, List<string> patterns, bool isWildcard,
        Func<string, string, bool> wildcardIsMatch = null)
    {
        foreach (var pattern in patterns) RemoveWhichContains(list, pattern, isWildcard, wildcardIsMatch);
    }
}