namespace SunamoFileSystem._sunamo.SunamoCollections;

internal class CA
{
    internal enum SearchStrategyCA
    {
        FixedSpace,
        AnySpaces,
        ExactlyName
    }

    internal static List<int> ReturnWhichContainsIndexes(string item, IList<string> terms/*,
        SearchStrategyCA searchStrategy = SearchStrategyCA.FixedSpace*/)
    {
        var result = new List<int>();
        var i = 0;
        foreach (var term in terms)
        {
            if (item.Contains(term) /*.Contains(item, term, searchStrategy)*/) result.Add(i);
            i++;
        }

        return result;
    }




    //    internal static Func<char, List<string>, List<string>> TrimStartChar;
    //    internal static Func<String[], List<string>> ToListMoreString;
    //    internal static Func<String, List<string>> ToListString;
    //    internal static Func<List<string>, string, List<string>> AddOrCreateInstance;
    //    internal static Func<string, IList<string>, List<int>> ContainsAnyFromElement;
    //    internal static Action<List<string>> RemoveStringsEmpty2;
    //    internal static Action<string, List<string>> Prepend;
    //    internal static Func<List<string>, List<string>> FirstCharUpper;
    //    internal static Action<string, List<string>> PostfixIfNotEnding;
    //    internal static Action<List<string>, Char[]> TrimEnd;
    //    internal static Action<List<string>, string, string> Replace;
    //    internal static Func<ChangeContentArgs, List<string>, Func<string, string>, List<string>> ChangeContent0;
    //    internal static Action<List<string>, string, bool> RemoveWhichContains;
    //    internal static Action<List<string>, List<string>, bool> RemoveWhichContainsList;


    /// <summary>
    ///     Direct editr
    /// </summary>
    /// <param name="files1"></param>
    /// <param name="item"></param>
    /// <param name="wildcard"></param>
    internal static void RemoveWhichContains(List<string> files1, string item, bool wildcard,
        Func<string, string, bool> WildcardIsMatch)
    {
        if (wildcard)
        {
            //item = SH.WrapWith(item, '*');
            for (var i = files1.Count - 1; i >= 0; i--)
                if (WildcardIsMatch(files1[i], item))
                    files1.RemoveAt(i);
        }
        else
        {
            for (var i = files1.Count - 1; i >= 0; i--)
                if (files1[i].Contains(item))
                    files1.RemoveAt(i);
        }
    }

    internal static void RemoveWhichContainsList(List<string> files, List<string> list, bool wildcard,
        Func<string, string, bool> WildcardIsMatch = null)
    {
        foreach (var item in list) RemoveWhichContains(files, item, wildcard, WildcardIsMatch);
    }
}