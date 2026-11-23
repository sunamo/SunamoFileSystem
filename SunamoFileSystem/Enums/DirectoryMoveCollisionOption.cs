namespace SunamoFileSystem.Enums;

/// <summary>
/// Represents the directory move collision option type.
/// </summary>
public enum DirectoryMoveCollisionOption
{
    // Add a numeric series suffix to the directory name to avoid collision.
    /// <summary>
    /// The add serie value.
    /// </summary>
    AddSerie,

    // Overwrite the existing directory at the destination.
    /// <summary>
    /// The overwrite value.
    /// </summary>
    Overwrite,

    // Discard the source directory and keep the existing destination.
    /// <summary>
    /// The discard from value.
    /// </summary>
    DiscardFrom,

    // Throw an exception when a collision is detected.
    /// <summary>
    /// The throw ex value.
    /// </summary>
    ThrowEx
}
