namespace SunamoFileSystem._public.SunamoData.Data;

/// <summary>
/// Represents the replace in all files args base type.
/// </summary>
public class ReplaceInAllFilesArgsBase
{
    /// <summary>
    /// Gets or sets the d remove git files.
    /// </summary>
    public Action<List<string>, bool, bool, bool>? DRemoveGitFiles { get; set; }

    /// <summary>
    /// Gets or sets the faster method for replacing.
    /// </summary>
    public Func<StringBuilder, IList<string>, IList<string>, StringBuilder>? FasterMethodForReplacing { get; set; }

    /// <summary>
    /// Gets or sets the files.
    /// </summary>
    public List<string> Files { get; set; } = new();

    /// <summary>
    /// Gets or sets the in downloaded folders.
    /// </summary>
    public bool InDownloadedFolders { get; set; }

    /// <summary>
    /// Gets or sets the in folders to delete.
    /// </summary>
    public bool InFoldersToDelete { get; set; }

    /// <summary>
    /// Gets or sets the in git files.
    /// </summary>
    public bool InGitFiles { get; set; }

    /// <summary>
    /// Gets or sets the is multiline with various indent.
    /// </summary>
    public bool IsMultilineWithVariousIndent { get; set; }

    /// <summary>
    /// Gets or sets the write every readed file as status.
    /// </summary>
    public bool WriteEveryReadedFileAsStatus { get; set; }

    /// <summary>
    /// Gets or sets the write every written file as status.
    /// </summary>
    public bool WriteEveryWrittenFileAsStatus { get; set; }
}
