namespace SunamoFileSystem;


public class ReplaceInAllFilesArgsBase
{
    public List<string> files;
    public bool isMultilineWithVariousIndent;
    public bool writeEveryReadedFileAsStatus;
    public bool writeEveryWrittenFileAsStatus;
    public Func<StringBuilder, IList<string>, IList<string>, StringBuilder> fasterMethodForReplacing;
    public bool inGitFiles;
    public bool inDownloadedFolders;
    public bool inFoldersToDelete;
    public Action<List<string>, bool, bool, bool> dRemoveGitFiles;
}