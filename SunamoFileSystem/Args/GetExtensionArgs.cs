namespace SunamoFileSystem.Args;

/// <summary>
/// Arguments for getting file extension
/// In original version exists only ReturnOriginalCase
/// </summary>
public class GetExtensionArgs
{
    /// <summary>
    /// If true, returns extension with original casing; otherwise returns lowercase
    /// </summary>
    public bool ReturnOriginalCase { get; set; } = false;

    /// <summary>
    /// If true, files without extension are returned as-is; otherwise returns empty string
    /// </summary>
    public bool FilesWithoutExtensionReturnAsIs { get; set; } = false;
}