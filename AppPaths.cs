namespace SunamoFileSystem;




/// <summary>
/// Must be here because is use in SunamoIni and others
/// </summary>
public class AppPaths
{
    public static string GetStartupPath()
    {
        return FS.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    }

    public static string GetFileInStartupPath(string file)
    {
        return Path.Combine(GetStartupPath(), file);
    }
}
