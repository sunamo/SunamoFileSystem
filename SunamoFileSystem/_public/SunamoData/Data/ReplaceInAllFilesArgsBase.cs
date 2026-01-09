namespace SunamoFileSystem._public.SunamoData.Data;

/// <summary>
/// Base arguments for replacing text in multiple files
/// </summary>
public class ReplaceInAllFilesArgsBase
{
    /// <summary>
    /// Action to remove Git files from the file list
    /// </summary>
    public Action<List<string>, bool, bool, bool>? DRemoveGitFiles { get; set; }

    /// <summary>
    /// Optional faster method for performing replacements
    /// </summary>
    public Func<StringBuilder, IList<string>, IList<string>, StringBuilder>? FasterMethodForReplacing { get; set; }

    /// <summary>
    /// List of files to process
    /// </summary>
    public List<string> Files { get; set; } = new();

    /// <summary>
    /// Whether to process files in downloaded folders
    /// </summary>
    public bool InDownloadedFolders { get; set; }

    /// <summary>
    /// Whether to process files in folders marked for deletion
    /// </summary>
    public bool InFoldersToDelete { get; set; }

    /// <summary>
    /// Whether to process files tracked by Git
    /// </summary>
    public bool InGitFiles { get; set; }

    /// <summary>
    /// Whether the replacement text is multiline with various indentation
    /// </summary>
    public bool IsMultilineWithVariousIndent { get; set; }

    /// <summary>
    /// Whether to write status for every file read
    /// </summary>
    public bool WriteEveryReadedFileAsStatus { get; set; }

    /// <summary>
    /// Whether to write status for every file written
    /// </summary>
    public bool WriteEveryWrittenFileAsStatus { get; set; }
}