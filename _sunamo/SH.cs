namespace SunamoFileSystem;

//namespace SunamoFileSystem;

public class SH
{
    public static string FirstCharUpper(ref string result)
    {
        result = FirstCharUpper(result);
        return result;
    }

    public static string WrapWithQm(string commitMessage)
    {
        return WrapWithQm(commitMessage, true);
    }
    public static string WrapWithQm(string item, bool? forceNotIncludeQm)
    {
        if (item.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault())
        {
            return SH.WrapWithQm(item);
        }
        return item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string WrapWith(string value, string h)
    {
        return h + value + h;
    }
    public static int OccurencesOfStringIn(string source, string p_2)
    {
        return source.Split(new string[] { p_2 }, StringSplitOptions.None).Length - 1;
    }
    public static bool IsContained(string item, string contains)
    {
        var (negation, contains2) = IsNegationTuple(contains);
        contains = contains2;

        if (negation && item.Contains(contains))
        {
            return false;
        }
        else if (!negation && !item.Contains(contains))
        {
            return false;
        }

        return true;
    }

    public static (bool, string) IsNegationTuple(string contains)
    {
        if (contains[0] == '!')
        {
            contains = contains.Substring(1);
            return (true, contains);
        }

        return (false, contains);
    }

    //    public static Func<string, string, int> OccurencesOfStringIn;
    //    public static Func<string, List<string>> GetLines;
    //    public static Func<string, string, bool, string> WrapWith;
    //    public static Func<int, string, string> JoinTimes;
    //    public static Func<string, bool, string> ReplaceAllDoubleSpaceToSingle2;
    //    //public static Func<string, int, Char[], List<string>> SplitToPartsFromEnd;
    //    //public static Func<string, string, List<string>> Split;
    //    public static Func<IList<string>, IList<string>, bool, string, string> ReplaceAll3;
    //    public static Func<string, bool, string> ReplaceAllDoubleSpaceToSingle;
    //    public static Func<string, bool> ContainsDiacritic;
    //    public static Func<string, string> TextWithoutDiacritic;
    //    public static Func<string, string> WrapWithQm;
    //    public static Func<string, string, string, string> ReplaceOnce;
    //    public static Func<string, string, bool> IsContained;
    //    public static Func<string, bool, bool, bool> ContainsOnlyCase;

    //    public static Func<string, char[], bool> IsNumber;
    //    public static Func<string, int, int, string> GetTextBetweenTwoChars;
    //    public static Func<string, object, string> RemoveAfterLast;
    //    public static Func<string, string, string, string> ReplaceAll2;
    //    public static Func<string, string> FirstCharUpper;
    //    public static Func<string, List<char>, bool> ContainsOnly;
    //    public static Func<string, string, string, string> ReplaceAll;

    //public static string FirstCharUpper(string nazevPP, bool only = false)
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

    public static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1)
        {
            return nazevPP.ToUpper();
        }

        string sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }
}
