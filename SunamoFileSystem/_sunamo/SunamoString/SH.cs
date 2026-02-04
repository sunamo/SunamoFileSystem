namespace SunamoFileSystem._sunamo.SunamoString;

internal class SH
{
    internal static string FirstCharUpper(ref string text)
    {
        text = FirstCharUpper(text);
        return text;
    }

    /// <summary>
    /// Wraps text with quote marks if needed.
    /// </summary>
    /// <param name="text">The text to wrap</param>
    /// <returns>Text wrapped with quote marks</returns>
    internal static string WrapWithQm(string text)
    {
        return WrapWithQm(text, true);
    }

    /// <summary>
    /// Wraps text with quote marks if needed.
    /// </summary>
    /// <param name="text">The text to wrap</param>
    /// <param name="forceNotIncludeQm">Force not to include quote marks</param>
    /// <returns>Text wrapped with quote marks if needed</returns>
    internal static string WrapWithQm(string text, bool? forceNotIncludeQm)
    {
        if (text.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return WrapWithQm(text);
        return text;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWith(string text, string wrapper)
    {
        return wrapper + text + wrapper;
    }

    internal static int OccurencesOfStringIn(string text, string searchString)
    {
        return text.Split(new[] { searchString }, StringSplitOptions.None).Length - 1;
    }

    /// <summary>
    /// Checks if text contains a substring (supports negation with ! prefix).
    /// </summary>
    /// <param name="text">The text to search in</param>
    /// <param name="contains">The substring to search for (prefix with ! for negation)</param>
    /// <returns>True if contained (or not contained with negation)</returns>
    internal static bool IsContained(string text, string contains)
    {
        var (negation, contains2) = IsNegationTuple(contains);
        contains = contains2;

        if (negation && text.Contains(contains))
            return false;
        if (!negation && !text.Contains(contains)) return false;

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

    internal static string FirstCharUpper(string text)
    {
        if (text.Length == 1) return text.ToUpper();

        var substring = text.Substring(1);
        return text[0].ToString().ToUpper() + substring;
    }






}