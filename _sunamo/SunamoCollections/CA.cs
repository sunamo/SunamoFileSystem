namespace SunamoFileSystem._sunamo.SunamoCollections;

internal class CA
{

    internal static bool IsListStringWrappedInArray<T>(List<T> v2)
    {
        var first = v2.First().ToString();
        if (v2.Count == 1 && (first == "System.Collections.Generic.List`1[System.String]" ||
        first == "System.Collections.Generic.List`1[System.Object]")) return true;
        return false;
    }
    internal static void InitFillWith(List<string> datas, int pocet, string initWith = Consts.stringEmpty)
    {
        InitFillWith<string>(datas, pocet, initWith);
    }
    internal static void InitFillWith<T>(List<T> datas, int pocet, T initWith)
    {
        for (int i = 0; i < pocet; i++)
        {
            datas.Add(initWith);
        }
    }
    internal static void InitFillWith<T>(List<T> arr, int columns)
    {
        for (int i = 0; i < columns; i++)
        {
            arr.Add(default);
        }
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
    /// Direct editr
    /// </summary>
    /// <param name="files1"></param>
    /// <param name="item"></param>
    /// <param name="wildcard"></param>
    internal static void RemoveWhichContains(List<string> files1, string item, bool wildcard, Func<string, string, bool> WildcardIsMatch)
    {
        if (wildcard)
        {
            //item = SH.WrapWith(item, AllChars.asterisk);
            for (int i = files1.Count - 1; i >= 0; i--)
            {
                if (WildcardIsMatch(files1[i], item))
                {
                    files1.RemoveAt(i);
                }
            }
        }
        else
        {
            for (int i = files1.Count - 1; i >= 0; i--)
            {
                if (files1[i].Contains(item))
                {
                    files1.RemoveAt(i);
                }
            }
        }
    }

    internal static void RemoveWhichContainsList(List<string> files, List<string> list, bool wildcard, Func<string, string, bool> WildcardIsMatch = null)
    {
        foreach (var item in list)
        {
            RemoveWhichContains(files, item, wildcard, WildcardIsMatch);
        }
    }



}
