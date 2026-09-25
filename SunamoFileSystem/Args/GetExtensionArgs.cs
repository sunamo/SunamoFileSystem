namespace SunamoFileSystem.Args;

// In original version exists only ReturnOriginalCase
public class GetExtensionArgs
{
    // If true, returns extension with original casing; otherwise returns lowercase
    public bool ReturnOriginalCase { get; set; } = false;

    // If true, files without extension are returned as-is; otherwise returns empty string
    public bool FilesWithoutExtensionReturnAsIs { get; set; } = false;
}
