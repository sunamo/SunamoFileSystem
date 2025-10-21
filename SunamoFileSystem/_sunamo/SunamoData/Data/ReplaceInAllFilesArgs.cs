// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoData.Data;

/// <summary>
///     Is passed into ReplaceInAllFilesWorker
/// </summary>
internal class ReplaceInAllFilesArgs : ReplaceInAllFilesArgsBase
{
    internal string from;
    internal bool isNotReplaceInTemporaryFiles;
    internal bool pairLinesInFromAndTo;
    internal bool replaceWithEmpty;
    internal string to;

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