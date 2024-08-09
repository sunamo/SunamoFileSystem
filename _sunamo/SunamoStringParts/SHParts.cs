namespace SunamoFileSystem._sunamo.SunamoStringParts;

internal class SHParts
{
    internal static string RemoveAfterLast(string nameSolution, object delimiter)
    {
        var dex = nameSolution.LastIndexOf(delimiter.ToString());
        if (dex != -1)
        {
            var s = nameSolution.Substring(0, dex); //SHSubstring.Substring(, 0, dex, new SubstringArgs());
            return s;
        }

        return nameSolution;
    }
}