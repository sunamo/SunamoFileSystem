namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

internal class TextOutputGeneratorArgs
{
    internal string delimiter = Environment.NewLine;
    internal bool headerWrappedEmptyLines = true;
    internal bool insertCount;
    internal string whenNoEntries = "No entries";

    internal TextOutputGeneratorArgs()
    {
    }

    internal TextOutputGeneratorArgs(bool headerWrappedEmptyLines, bool insertCount)
    {
        this.headerWrappedEmptyLines = headerWrappedEmptyLines;
        this.insertCount = insertCount;
    }
}