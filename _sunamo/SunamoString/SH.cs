namespace SunamoFileSystem._sunamo.SunamoString;

internal class SH
{
    internal static string FirstCharUpper(ref string result)
    {
        result = FirstCharUpper(result);
        return result;
    }

    internal static string WrapWithQm(string commitMessage)
    {
        return WrapWithQm(commitMessage, true);
    }

    internal static string WrapWithQm(string item, bool? forceNotIncludeQm)
    {
        if (item.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return WrapWithQm(item);
        return item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWith(string value, string h)
    {
        return h + value + h;
    }

    internal static int OccurencesOfStringIn(string source, string p_2)
    {
        return source.Split(new[] { p_2 }, StringSplitOptions.None).Length - 1;
    }

    internal static bool IsContained(string item, string contains)
    {
        var (negation, contains2) = IsNegationTuple(contains);
        contains = contains2;

        if (negation && item.Contains(contains))
            return false;
        if (!negation && !item.Contains(contains)) return false;

        return true;
    }

    internal static (bool, string) IsNegationTuple(string contains)
    {
        if (contains[0] == '!')
        {
            contains = contains.Substring(1);
            return (true, contains);
        }

        return (false, contains);
    }

    //    internal static Func<string, string, int> OccurencesOfStringIn;
    //    internal static Func<string, List<string>> GetLines;
    //    internal static Func<string, string, bool, string> WrapWith;
    //    internal static Func<int, string, string> JoinTimes;
    //    internal static Func<string, bool, string> ReplaceAllDoubleSpaceToSingle2;
    //    //internal static Func<string, int, Char[], List<string>> SplitToPartsFromEnd;
    //    //internal static Func<string, string, List<string>> Split;
    //    internal static Func<IList<string>, IList<string>, bool, string, string> ReplaceAll3;
    //    internal static Func<string, bool, string> ReplaceAllDoubleSpaceToSingle;
    //    internal static Func<string, bool> ContainsDiacritic;
    //    internal static Func<string, string> TextWithoutDiacritic;
    //    internal static Func<string, string> WrapWithQm;
    //    internal static Func<string, string, string, string> ReplaceOnce;
    //    internal static Func<string, string, bool> IsContained;
    //    internal static Func<string, bool, bool, bool> ContainsOnlyCase;

    //    internal static Func<string, char[], bool> IsNumber;
    //    internal static Func<string, int, int, string> GetTextBetweenTwoChars;
    //    internal static Func<string, object, string> RemoveAfterLast;
    //    internal static Func<string, string, string, string> ReplaceAll2;
    //    internal static Func<string, string> FirstCharUpper;
    //    internal static Func<string, List<char>, bool> ContainsOnly;
    //    internal static Func<string, string, string, string> ReplaceAll;

    //internal static string FirstCharUpper(string nazevPP, bool only = false)
    //{
    //    if (nazevPP != null)
    //    {
    //        string sb = nazevPP.Substring(1);
    //        if (only)
    //        {
    //            sb = sb.ToLower();
    //        }

    //        return nazevPP[0].ToString().ToUpper() + sb;
    //    }

    //    return null;
    //}

    internal static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1) return nazevPP.ToUpper();

        var sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }

    internal static string JoinNL(List<string> l)
    {
        StringBuilder sb = new();
        foreach (var item in l) sb.AppendLine(item);
        var r = string.Empty;
        r = sb.ToString();
        return r;
    }

    internal static List<string> SplitCharMore(string s, params char[] dot)
    {
        return s.Split(dot, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    internal static List<string> SplitMore(string s, params string[] dot)
    {
        return s.Split(dot, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    internal static List<string> SplitNone(string text, params string[] deli)
    {
        return text.Split(deli, StringSplitOptions.None).ToList();
    }

    internal static string NullToStringOrDefault(object n)
    {
        return n == null ? " " + "(null)" : "" + n;
    }

    internal static string TrimEnd(string name, string ext)
    {
        while (name.EndsWith(ext)) return name.Substring(0, name.Length - ext.Length);
        return name;
    }
}