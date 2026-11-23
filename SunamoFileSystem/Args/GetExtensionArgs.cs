namespace SunamoFileSystem.Args;

// In original version exists only ReturnOriginalCase
/// <summary>
/// Represents the get extension args type.
/// </summary>
public class GetExtensionArgs
{
    // If true, returns extension with original casing; otherwise returns lowercase
    /// <summary>
    /// Gets or sets the return original case.
    /// </summary>
    public bool ReturnOriginalCase { get; set; } = false;

    // If true, files without extension are returned as-is; otherwise returns empty string
    /// <summary>
    /// Gets or sets the files without extension return as is.
    /// </summary>
    public bool FilesWithoutExtensionReturnAsIs { get; set; } = false;
}
