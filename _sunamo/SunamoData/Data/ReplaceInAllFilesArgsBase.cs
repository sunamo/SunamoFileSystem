namespace SunamoFileSystem;


public class ReplaceInAllFilesArgsBase
{
    internal List<string> files;
    internal bool isMultilineWithVariousIndent;
    internal bool writeEveryReadedFileAsStatus;
    internal bool writeEveryWrittenFileAsStatus;
    internal Func<StringBuilder, IList<string>, IList<string>, StringBuilder> fasterMethodForReplacing;
    internal bool inGitFiles;
    internal bool inDownloadedFolders;
    internal bool inFoldersToDelete;
    internal Action<List<string>, bool, bool, bool> dRemoveGitFiles;
}