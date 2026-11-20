namespace SunamoFileSystem._sunamo.SunamoString;

internal class SH
{
    internal static string FirstCharUpper(ref string text)
    {
        text = FirstCharUpper(text);
        return text;
    }

    internal static string WrapWithQm(string text) => WrapWithQm(text, true);

    internal static string WrapWithQm(string text, bool? forceNotIncludeQm)
    {
        if (text.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return WrapWithQm(text);
        return text;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string WrapWith(string text, string wrapper) => wrapper + text + wrapper;

    internal static int OccurencesOfStringIn(string text, string searchString)
        => text.Split(new[] { searchString }, StringSplitOptions.None).Length - 1;

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






    internal static string FirstCharUpper(string text)
    {
        if (text.Length == 1) return text.ToUpper();

        var substring = text.Substring(1);
        return text[0].ToString().ToUpper() + substring;
    }
}
