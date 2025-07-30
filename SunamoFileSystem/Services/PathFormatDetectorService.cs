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
            // Obsahuje oba oddělovače - může být složitější případ (např. UNC cesty ve Windows,
            // nebo záměrná kombinace). Zde můžeme upřednostnit Windows, nebo vrátit neurčito.
            // Záleží na konkrétním případu použití.
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