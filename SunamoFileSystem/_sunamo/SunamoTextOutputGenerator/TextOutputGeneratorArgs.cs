namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

/// <summary>
/// Arguments for text output generation.
/// EN: Arguments for text output generation.
/// CZ: Argumenty pro generování textového výstupu.
/// </summary>
internal class TextOutputGeneratorArgs
{
    /// <summary>
    /// Gets or sets the delimiter between items.
    /// </summary>
    internal string Delimiter { get; set; } = Environment.NewLine;

    /// <summary>
    /// Gets or sets whether header is wrapped with empty lines.
    /// </summary>
    internal bool IsHeaderWrappedWithEmptyLines { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to insert count.
    /// </summary>
    internal bool IsInsertingCount { get; set; }

    /// <summary>
    /// Gets or sets the text to display when there are no entries.
    /// </summary>
    internal string WhenNoEntries { get; set; } = "No entries";

    /// <summary>
    /// Initializes a new instance of the TextOutputGeneratorArgs class.
    /// </summary>
    internal TextOutputGeneratorArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified settings.
    /// </summary>
    /// <param name="isHeaderWrappedWithEmptyLines">Whether header is wrapped with empty lines.</param>
    /// <param name="isInsertingCount">Whether to insert count.</param>
    internal TextOutputGeneratorArgs(bool isHeaderWrappedWithEmptyLines, bool isInsertingCount)
    {
        this.IsHeaderWrappedWithEmptyLines = isHeaderWrappedWithEmptyLines;
        this.IsInsertingCount = isInsertingCount;
    }
}