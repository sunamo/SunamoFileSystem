namespace SunamoFileSystem._sunamo.SunamoStringReplace;

internal class SHReplace
{
    /// <summary>
    /// Replaces the first occurrence of a pattern in the input string
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="what">The pattern to search for</param>
    /// <param name="replacement">The replacement string</param>
    /// <returns>String with first occurrence replaced</returns>
    internal static string ReplaceOnce(string input, string what, string replacement)
    {
        return new Regex(what).Replace(input, replacement, 1);
    }

    internal static string ReplaceAllDoubleSpaceToSingle2(string text, bool alsoHtml = false)
    {
        if (alsoHtml)
        {
            text = text.Replace(" &nbsp;", " ");
            text = text.Replace("&nbsp; ", " ");
            text = text.Replace("&nbsp;", " ");
        }

        WhitespaceCharService whitespaceChar = new WhitespaceCharService();

        var parameter = text.Split(whitespaceChar.WhiteSpaceChars
            .ToArray()); //SHSplit.Split(text, AllChars.WhiteSpaceChars.ConvertAll(d => d.ToString()).ToArray());
        return string.Join(" ", parameter);
    }
}