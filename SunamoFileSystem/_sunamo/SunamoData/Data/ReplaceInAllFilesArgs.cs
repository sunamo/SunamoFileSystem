namespace SunamoFileSystem._sunamo.SunamoData.Data;

/// <summary>
///     Is passed into ReplaceInAllFilesWorker
/// </summary>
internal class ReplaceInAllFilesArgs : ReplaceInAllFilesArgsBase
{
    internal string from = string.Empty;
    internal bool isNotReplaceInTemporaryFiles;
    internal bool pairLinesInFromAndTo;
    internal bool replaceWithEmpty;
    internal string to = string.Empty;

    internal ReplaceInAllFilesArgs()
    {
    }

    internal ReplaceInAllFilesArgs(ReplaceInAllFilesArgsBase b)
    {
        Files = b.Files;
        IsMultilineWithVariousIndent = b.IsMultilineWithVariousIndent;
        WriteEveryReadedFileAsStatus = b.WriteEveryReadedFileAsStatus;
        WriteEveryWrittenFileAsStatus = b.WriteEveryWrittenFileAsStatus;
        FasterMethodForReplacing = b.FasterMethodForReplacing;
        InGitFiles = b.InGitFiles;
        InDownloadedFolders = b.InDownloadedFolders;
        InFoldersToDelete = b.InFoldersToDelete;
        DRemoveGitFiles = b.DRemoveGitFiles;
    }
}