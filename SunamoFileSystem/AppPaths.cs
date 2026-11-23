namespace SunamoFileSystem;

/// <summary>
/// Represents the app paths type.
/// </summary>
public class AppPaths
{
    /// <summary>
    /// Get startup path.
    /// </summary>
    /// <returns>The get startup path result.</returns>
    public static string GetStartupPath()
    {
        return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;
    }

    /// <summary>
    /// Get file in startup path.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The get file in startup path result.</returns>
    public static string GetFileInStartupPath(string fileName)
    {
        return Path.Combine(GetStartupPath(), fileName);
    }
}
