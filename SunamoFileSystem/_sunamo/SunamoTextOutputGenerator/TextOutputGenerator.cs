namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

/// <summary>
/// Generator for text output.
/// EN: Generator for text output.
/// CZ: Generátor pro textový výstup.
/// </summary>
internal class TextOutputGenerator
{
    internal StringBuilder StringBuilder = new();

    /// <summary>
    /// Returns the string representation of the generated text.
    /// </summary>
    /// <returns>The generated text.</returns>
    public override string ToString()
    {
        var result = StringBuilder.ToString();
        return result;
    }

    /// <summary>
    /// Undoes the last operation.
    /// </summary>
    internal void Undo()
    {
        ThrowEx.NotImplementedMethod();
    }
}