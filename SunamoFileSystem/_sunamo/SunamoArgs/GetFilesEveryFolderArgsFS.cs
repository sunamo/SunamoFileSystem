namespace SunamoFileSystem._sunamo.SunamoArgs;

public class GetFilesEveryFolderArgsFS : GetFilesBaseArgsFS
{
    internal Action? Done = null;
    internal Action? DoneOnePercent = null;

    // Returns false if file should not be indexed, otherwise true
    internal Func<string, bool>? FilterFoundedFiles = null;

    // Returns false if folder should not be indexed, otherwise true
    internal Func<string, bool>? FilterFoundedFolders = null;

    internal int GetNullIfThereIsMoreThanXFiles = -1;
    internal Action<double>? InsertProgressBar = null;
    internal Action<double>? InsertProgressBarTime = null;
    internal Action<string>? UpdateProgressBarText = null;
    internal bool UseProgressBar = false;
    internal bool UseProgressBarTime = false;
}
