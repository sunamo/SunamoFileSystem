namespace SunamoFileSystem;

public partial class FS
{
    /// <summary>
    /// With end bs.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The with end bs result.</returns>
    public static string WithEndBs(string value)
    {
        return value.EndsWith("\\") ? value : value + "\\";
    }
}
