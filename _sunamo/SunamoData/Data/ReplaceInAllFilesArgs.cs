namespace SunamoFileSystem;


/// <summary>
/// Is passed into ReplaceInAllFilesWorker
/// </summary>
public class ReplaceInAllFilesArgs : ReplaceInAllFilesArgsBase
{
    public string from;
    public string to;
    public bool pairLinesInFromAndTo;
    public bool replaceWithEmpty;
    public bool isNotReplaceInTemporaryFiles;
    public ReplaceInAllFilesArgs()
    {
    }
    public ReplaceInAllFilesArgs(ReplaceInAllFilesArgsBase b)
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