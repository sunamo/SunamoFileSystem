namespace SunamoFileSystem._sunamo.SunamoStringGetLines;

/// <summary>
/// Helper class for splitting strings into lines.
/// EN: Helper class for splitting strings into lines.
/// CZ: Pomocná třída pro rozdělení stringů na řádky.
/// </summary>
internal class SHGetLines
{
    /// <summary>
    /// Splits text into lines, handling various newline formats.
    /// </summary>
    /// <param name="text">The text to split into lines.</param>
    /// <returns>List of lines.</returns>
    internal static List<string> GetLines(string text)
    {
        var parts = text.Split(new[] { "\r\n", "\n\r" }, StringSplitOptions.None).ToList();
        SplitByUnixNewline(parts);
        return parts;
    }

    /// <summary>
    /// Splits lines by Unix-style newline characters.
    /// </summary>
    /// <param name="lines">The list of lines to split.</param>
    private static void SplitByUnixNewline(List<string> lines)
    {
        SplitBy(lines, "\r");
        SplitBy(lines, "\n");
    }

    /// <summary>
    /// Splits lines by the specified delimiter.
    /// </summary>
    /// <param name="lines">The list of lines to split.</param>
    /// <param name="delimiter">The delimiter to split by.</param>
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

    /// <summary>
    /// Inserts split parts into the lines list at the specified index.
    /// </summary>
    /// <param name="lines">The list of lines.</param>
    /// <param name="splitParts">The split parts to insert.</param>
    /// <param name="index">The index where to insert.</param>
    private static void InsertOnIndex(List<string> lines, List<string> splitParts, int index)
    {
        splitParts.Reverse();

        lines.RemoveAt(index);

        foreach (var item in splitParts) lines.Insert(index, item);
    }
}