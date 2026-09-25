namespace SunamoFileSystem;

public partial class FS
{
    public static string WithEndBs(string value)
    {
        return value.EndsWith("\\") ? value : value + "\\";
    }
}
