namespace SunamoFileSystem._sunamo.SunamoCollections;

internal class CA
{
    internal static void InitFillWith<T>(List<T> list, int count)
    {
        for (var i = 0; i < count; i++) list.Add(default!);
    }

    internal enum SearchStrategyCA
    {
        FixedSpace,
        AnySpaces,
        ExactlyName
    }

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

    // Direct editor - modifies the list in place
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

    internal static void RemoveWhichContainsList(List<string> list, List<string> patterns, bool isWildcard,
        Func<string, string, bool>? wildcardIsMatch = null)
    {
        foreach (var pattern in patterns) RemoveWhichContains(list, pattern, isWildcard, wildcardIsMatch!);
    }
}
