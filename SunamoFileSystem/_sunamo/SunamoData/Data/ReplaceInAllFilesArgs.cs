namespace SunamoFileSystem._sunamo.SunamoData.Data;

/// <summary>
///     Is passed into ReplaceInAllFilesWorker
/// </summary>
internal class ReplaceInAllFilesArgs : ReplaceInAllFilesArgsBase
{
    internal string From = string.Empty;
    internal bool IsNotReplaceInTemporaryFiles = false;
    internal bool PairLinesInFromAndTo = false;
    internal bool ReplaceWithEmpty = false;
    internal string To = string.Empty;

    internal ReplaceInAllFilesArgs()
    {
    }

    internal ReplaceInAllFilesArgs(ReplaceInAllFilesArgsBase baseArgs)
    {
        Files = baseArgs.Files;
        IsMultilineWithVariousIndent = baseArgs.IsMultilineWithVariousIndent;
        WriteEveryReadedFileAsStatus = baseArgs.WriteEveryReadedFileAsStatus;
        WriteEveryWrittenFileAsStatus = baseArgs.WriteEveryWrittenFileAsStatus;
        FasterMethodForReplacing = baseArgs.FasterMethodForReplacing;
        InGitFiles = baseArgs.InGitFiles;
        InDownloadedFolders = baseArgs.InDownloadedFolders;
        InFoldersToDelete = baseArgs.InFoldersToDelete;
        DRemoveGitFiles = baseArgs.DRemoveGitFiles;
    }
}