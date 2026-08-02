namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

internal class TextOutputGeneratorArgs
{
    internal string Delimiter { get; set; } = Environment.NewLine;

    internal bool IsHeaderWrappedWithEmptyLines { get; set; } = true;

    internal bool IsInsertingCount { get; set; }

    internal string WhenNoEntries { get; set; } = "No entries";

    internal TextOutputGeneratorArgs()
    {
    }

    internal TextOutputGeneratorArgs(bool isHeaderWrappedWithEmptyLines, bool isInsertingCount)
    {
        this.IsHeaderWrappedWithEmptyLines = isHeaderWrappedWithEmptyLines;
        this.IsInsertingCount = isInsertingCount;
    }
}
