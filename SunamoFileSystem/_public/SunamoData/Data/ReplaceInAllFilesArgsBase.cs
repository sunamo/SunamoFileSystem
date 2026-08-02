namespace SunamoFileSystem._public.SunamoData.Data;

public class ReplaceInAllFilesArgsBase
{
    public Action<List<string>, bool, bool, bool>? DRemoveGitFiles { get; set; }

    public Func<StringBuilder, IList<string>, IList<string>, StringBuilder>? FasterMethodForReplacing { get; set; }

    public List<string> Files { get; set; } = new();

    public bool InDownloadedFolders { get; set; }

    public bool InFoldersToDelete { get; set; }

    public bool InGitFiles { get; set; }

    public bool IsMultilineWithVariousIndent { get; set; }

    public bool WriteEveryReadedFileAsStatus { get; set; }

    public bool WriteEveryWrittenFileAsStatus { get; set; }
}
