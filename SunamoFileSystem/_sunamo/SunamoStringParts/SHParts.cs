namespace SunamoFileSystem._sunamo.SunamoStringParts;

internal class SHParts
{
    internal static string RemoveAfterLast(string nameSolution, object delimiter)
    {
        var dex = nameSolution.LastIndexOf(delimiter.ToString());
        if (dex != -1)
        {
            var text = nameSolution.Substring(0, dex); //SHSubstring.Substring(, 0, dex, new SubstringArgs());
            return text;
        }

        return nameSolution;
    }

    internal static string? GetTextBetweenTwoChars(string p, char begin, char end)
    {
        return GetTextBetweenTwoCharsInts(p, p.IndexOf(begin), p.IndexOf(end));
    }

    internal static string? GetTextBetweenTwoCharsInts(string p, int begin, int end)
    {
        if (end > begin)
            return p.Substring(begin + 1, end - begin - 1);
        return null;
    }
}