namespace SunamoFileSystem;

public class AppPaths
{
    public static string GetStartupPath()
    {
        return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;
    }

    public static string GetFileInStartupPath(string fileName)
    {
        return Path.Combine(GetStartupPath(), fileName);
    }
}
