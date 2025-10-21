// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._public.SunamoData.Data;

public class ReplaceInAllFilesArgsBase
{
    public Action<List<string>, bool, bool, bool> dRemoveGitFiles;
    public Func<StringBuilder, IList<string>, IList<string>, StringBuilder> fasterMethodForReplacing;
    public List<string> files;
    public bool inDownloadedFolders;
    public bool inFoldersToDelete;
    public bool inGitFiles;
    public bool isMultilineWithVariousIndent;
    public bool writeEveryReadedFileAsStatus;
    public bool writeEveryWrittenFileAsStatus;
}