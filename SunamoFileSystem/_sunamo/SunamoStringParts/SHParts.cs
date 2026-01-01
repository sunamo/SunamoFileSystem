namespace SunamoFileSystem._sunamo.SunamoStringParts;

internal class SHParts
{
    internal static string RemoveAfterLast(string text, object delimiter)
    {
        var dex = text.LastIndexOf(delimiter.ToString());
        if (dex != -1)
        {
            var result = text.Substring(0, dex); //SHSubstring.Substring(, 0, dex, new SubstringArgs());
            return result;
        }

        return text;
    }

    internal static string? GetTextBetweenTwoChars(string text, char begin, char end)
    {
        return GetTextBetweenTwoCharsInts(text, text.IndexOf(begin), text.IndexOf(end));
    }

    internal static string? GetTextBetweenTwoCharsInts(string text, int begin, int end)
    {
        if (end > begin)
            return text.Substring(begin + 1, end - begin - 1);
        return null;
    }
}