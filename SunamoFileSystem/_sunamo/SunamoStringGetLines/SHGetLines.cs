namespace SunamoFileSystem._sunamo.SunamoStringGetLines;

internal class SHGetLines
{
    internal static List<string> GetLines(string text)
    {
        var parts = text.Split(new[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(parts);
        return parts;
    }

    private static void SplitByUnixNewline(List<string> lines)
    {
        SplitBy(lines, "\r");
        SplitBy(lines, "\n");
    }

    private static void SplitBy(List<string> lines, string delimiter)
    {
        for (var i = lines.Count - 1; i >= 0; i--)
        {
            if (delimiter == "\r")
            {
                var windowsNewlineParts = lines[i].Split(new[] { "\r\n" }, StringSplitOptions.None);
                var unixNewlineParts = lines[i].Split(new[] { "\n\r" }, StringSplitOptions.None);

                if (windowsNewlineParts.Length > 1)
                    ThrowEx.Custom("cannot contain any \r\name, pass already split by this pattern");
                else if (unixNewlineParts.Length > 1) ThrowEx.Custom("cannot contain any \n\r, pass already split by this pattern");
            }

            var splitResult = lines[i].Split(new[] { delimiter }, StringSplitOptions.None);

            if (splitResult.Length > 1) InsertOnIndex(lines, splitResult.ToList(), i);
        }
    }

    private static void InsertOnIndex(List<string> lines, List<string> splitParts, int index)
    {
        splitParts.Reverse();

        lines.RemoveAt(index);

        foreach (var item in splitParts) lines.Insert(index, item);
    }
}
