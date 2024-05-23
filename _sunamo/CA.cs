namespace SunamoFileSystem;

//namespace SunamoFileSystem;

public class CA
{
    //    public static Func<char, List<string>, List<string>> TrimStartChar;
    //    public static Func<String[], List<string>> ToListMoreString;
    //    public static Func<String, List<string>> ToListString;
    //    public static Func<List<string>, string, List<string>> AddOrCreateInstance;
    //    public static Func<string, IList<string>, List<int>> ContainsAnyFromElement;
    //    public static Action<List<string>> RemoveStringsEmpty2;
    //    public static Action<string, List<string>> Prepend;
    //    public static Func<List<string>, List<string>> FirstCharUpper;
    //    public static Action<string, List<string>> PostfixIfNotEnding;
    //    public static Action<List<string>, Char[]> TrimEnd;
    //    public static Action<List<string>, string, string> Replace;
    //    public static Func<ChangeContentArgs, List<string>, Func<string, string>, List<string>> ChangeContent0;
    //    public static Action<List<string>, string, bool> RemoveWhichContains;
    //    public static Action<List<string>, List<string>, bool> RemoveWhichContainsList;


    /// <summary>
    /// Direct editr
    /// </summary>
    /// <param name="files1"></param>
    /// <param name="item"></param>
    /// <param name="wildcard"></param>
    public static void RemoveWhichContains(List<string> files1, string item, bool wildcard, Func<string, string, bool> WildcardIsMatch)
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

    public static void RemoveWhichContainsList(List<string> files, List<string> list, bool wildcard, Func<string, string, bool> WildcardIsMatch = null)
    {
        foreach (var item in list)
        {
            RemoveWhichContains(files, item, wildcard, WildcardIsMatch);
        }
    }



}
