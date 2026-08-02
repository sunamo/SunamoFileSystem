namespace SunamoFileSystem._sunamo.SunamoStringReplace;

internal class SHReplace
{
    internal static string ReplaceOnce(string text, string what, string replacement)
        => new Regex(what).Replace(text, replacement, 1);

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
