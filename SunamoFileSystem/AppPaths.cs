namespace SunamoFileSystem;

/// <summary>
/// Provides methods for getting application startup paths
/// Must be here because is used in SunamoIni and others
/// </summary>
public class AppPaths
{
    /// <summary>
    /// Gets the startup path of the current process
    /// </summary>
    /// <returns>The directory path of the main module</returns>
    public static string GetStartupPath()
    {
        return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    }

    /// <summary>
    /// Combines the startup path with the specified file name
    /// </summary>
    /// <param name="fileName">The file name to combine with the startup path</param>
    /// <returns>The full path combining startup path and file name</returns>
    public static string GetFileInStartupPath(string fileName)
    {
        return Path.Combine(GetStartupPath(), fileName);
    }
}