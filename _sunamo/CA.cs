namespace SunamoFileSystem._sunamo;

internal class CA
{
    internal static Func<char, List<string>, List<string>> TrimStartChar;
    internal static Func<String[], List<string>> ToListMoreString;
    internal static Func<String, List<string>> ToListString;
    internal static Func<List<string>, string, List<string>> AddOrCreateInstance;
    internal static Func<string, IList<string>, List<int>> ContainsAnyFromElement;
    internal static Action<List<string>> RemoveStringsEmpty2;
    internal static Action<string, List<string>> Prepend;
    internal static Func<List<string>, List<string>> FirstCharUpper;
    internal static Action<string, List<string>> PostfixIfNotEnding;
    internal static Action<List<string>, Char[]> TrimEnd;
    internal static Action<List<string>, string, string> Replace;
    internal static Func<ChangeContentArgs, List<string>, Func<string, string>, List<string>> ChangeContent0;
    internal static Action<List<string>, string, bool> RemoveWhichContains;
    internal static Action<List<string>, List<string>, bool> RemoveWhichContainsList;


}
