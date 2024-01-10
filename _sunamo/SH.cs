namespace SunamoFileSystem._sunamo;

internal class SH
{
    internal static Func<string, string, int> OccurencesOfStringIn;
    internal static Func<string, List<string>> GetLines;
    internal static Func<string, string, bool, string> WrapWith;
    internal static Func<int, string, string> JoinTimes;
    internal static Func<string, bool, string> ReplaceAllDoubleSpaceToSingle2;
    //internal static Func<string, int, Char[], List<string>> SplitToPartsFromEnd;
    //internal static Func<string, string, List<string>> Split;
    internal static Func<IList<string>, IList<string>, bool, string, string> ReplaceAll3;
    internal static Func<string, bool, string> ReplaceAllDoubleSpaceToSingle;
    internal static Func<string, bool> ContainsDiacritic;
    internal static Func<string, string> TextWithoutDiacritic;
    internal static Func<string, string> WrapWithQm;
    internal static Func<string, string, string, string> ReplaceOnce;
    internal static Func<string, string, bool> IsContained;
    internal static Func<string, bool, bool, bool> ContainsOnlyCase;

    internal static Func<string, char[], bool> IsNumber;
    internal static Func<string, int, int, string> GetTextBetweenTwoChars;
    internal static Func<string, object, string> RemoveAfterLast;
    internal static Func<string, string, string, string> ReplaceAll2;
    internal static Func<string, string> FirstCharUpper;
    internal static Func<string, List<char>, bool> ContainsOnly;
    internal static Func<string, string, string, string> ReplaceAll;
}
