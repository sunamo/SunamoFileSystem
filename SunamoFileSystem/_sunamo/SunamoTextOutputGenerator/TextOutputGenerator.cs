namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

internal class TextOutputGenerator
{
    internal StringBuilder StringBuilder = new();

    public override string ToString()
    {
        var result = StringBuilder.ToString();
        return result;
    }

    internal void Undo()
    {
        ThrowEx.NotImplementedMethod();
    }
}
