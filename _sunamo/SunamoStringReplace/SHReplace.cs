namespace SunamoFileSystem._sunamo.SunamoStringReplace;


internal class SHReplace
{
    internal static string ReplaceOnce(string input, string what, string zaco)
    {
        return new Regex(what).Replace(input, zaco, 1);
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

        var p = text.Split(whitespaceChar.whiteSpaceChars
            .ToArray()); //SHSplit.SplitMore(text, AllChars.whiteSpaceChars.ConvertAll(d => d.ToString()).ToArray());
        return string.Join(" ", p);
    }
}