namespace SunamoFileSystem._public.SunamoEnums.Enums;

/// <summary>
/// Specifies how to handle collisions when creating a directory that already exists.
/// </summary>
public enum DirectoryCreateCollisionOption
{
    /// <summary>
    /// Delete the existing directory before creating the new one.
    /// </summary>
    Delete,

    /// <summary>
    /// Overwrite the existing directory contents.
    /// </summary>
    Overwrite,

    /// <summary>
    /// Add a numeric series suffix to the directory name to avoid collision.
    /// </summary>
    AddSerie
}