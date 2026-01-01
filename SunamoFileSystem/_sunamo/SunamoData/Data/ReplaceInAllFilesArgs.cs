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
        files = b.files;
        isMultilineWithVariousIndent = b.isMultilineWithVariousIndent;
        writeEveryReadedFileAsStatus = b.writeEveryReadedFileAsStatus;
        writeEveryWrittenFileAsStatus = b.writeEveryWrittenFileAsStatus;
        fasterMethodForReplacing = b.fasterMethodForReplacing;
        inGitFiles = b.inGitFiles;
        inDownloadedFolders = b.inDownloadedFolders;
        inFoldersToDelete = b.inFoldersToDelete;
        dRemoveGitFiles = b.dRemoveGitFiles;
    }
}