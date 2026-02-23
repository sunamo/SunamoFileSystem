namespace SunamoFileSystem.Enums;

/// <summary>
/// Specifies how to handle collisions when moving a directory to a location where a directory already exists.
/// </summary>
public enum DirectoryMoveCollisionOption
{
    /// <summary>
    /// Add a numeric series suffix to the directory name to avoid collision.
    /// </summary>
    AddSerie,

    /// <summary>
    /// Overwrite the existing directory at the destination.
    /// </summary>
    Overwrite,

    /// <summary>
    /// Discard the source directory and keep the existing destination.
    /// </summary>
    DiscardFrom,

    /// <summary>
    /// Throw an exception when a collision is detected.
    /// </summary>
    ThrowEx
}