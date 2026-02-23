namespace SunamoFileSystem.Services;

/// <summary>
/// Service for detecting path format (Windows vs Unix).
/// </summary>
/// <param name="logger">Logger instance for diagnostic messages.</param>
public class PathFormatDetectorService(ILogger logger)
{
    /// <summary>
    /// Determines whether the specified path uses Windows path format (e.g., C:\folder).
    /// </summary>
    /// <param name="path">The file system path to check.</param>
    /// <returns>True if the path uses Windows path format; otherwise, false.</returns>
    public bool IsWindowsPathFormat(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        var badFormat = false;
        if (path.Length < 3) return badFormat;
        if (!char.IsLetter(path[0])) badFormat = true;
        if (char.IsLetter(path[1])) badFormat = true;
        if (path.Length > 2)
            if (path[1] != '\\' && path[2] != '\\')
                badFormat = true;
        return !badFormat;
    }

    /// <summary>
    /// Return true if Windows, false if Unix
    /// </summary>
    /// <param name="path"></param>
    /// <param name="logIfIsNotUnixOrWindowsPath"></param>
    /// <returns></returns>
    public bool? DetectPathType(string path, bool logIfIsNotUnixOrWindowsPath = false)
    {
        if (IsWindowsPathFormat(path))
        {
            return true;
        }

        if (path.Contains('\\') && !path.Contains('/'))
        {
            return true;
        }
        else if (!path.Contains('\\') && path.StartsWith('/'))
        {
            return false;
        }
        else if (path.Contains('\\') && path.Contains('/'))
        {
            // Contains both separators - might be a complex case (e.g., UNC paths in Windows,
            // or intentional combination). Here we can prioritize Windows or return undetermined.
            // Depends on specific use case.
            if (logIfIsNotUnixOrWindowsPath)
            {
                logger.LogError(path + " - Contains both \\ and /");
            }

            return null;
        }
        else
        {
            if (logIfIsNotUnixOrWindowsPath)
            {
                logger.LogError(path + " - Invalid path or does not contain separators");
            }

            return null;
        }
    }

}