namespace SunamoFileSystem.Services;

public class PathFormatDetectorService(ILogger logger)
{
    public bool IsWindowsPathFormat(string argValue)
    {
        if (string.IsNullOrWhiteSpace(argValue)) return false;
        var badFormat = false;
        if (argValue.Length < 3) return badFormat;
        if (!char.IsLetter(argValue[0])) badFormat = true;
        if (char.IsLetter(argValue[1])) badFormat = true;
        if (argValue.Length > 2)
            if (argValue[1] != '\\' && argValue[2] != '\\')
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