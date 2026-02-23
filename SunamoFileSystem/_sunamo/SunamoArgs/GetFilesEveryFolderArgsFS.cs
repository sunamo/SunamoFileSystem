namespace SunamoFileSystem._sunamo.SunamoArgs;

/// <summary>
/// Arguments for getting files from every folder recursively
/// </summary>
internal class GetFilesEveryFolderArgsFS : GetFilesBaseArgsFS
{
    internal Action? Done = null;
    internal Action? DoneOnePercent = null;

    /// <summary>
    /// Returns false if file should not be indexed, otherwise true
    /// </summary>
    internal Func<string, bool>? FilterFoundedFiles = null;

    /// <summary>
    /// Returns false if folder should not be indexed, otherwise true
    /// </summary>
    internal Func<string, bool>? FilterFoundedFolders = null;

    internal int GetNullIfThereIsMoreThanXFiles = -1;
    internal Action<double>? InsertProgressBar = null;
    internal Action<double>? InsertProgressBarTime = null;
    internal Action<string>? UpdateProgressBarText = null;
    internal bool UseProgressBar = false;
    internal bool UseProgressBarTime = false;
}