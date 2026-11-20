namespace SunamoFileSystem.Enums;

/// <summary>
/// Represents the file move collision option type.
/// </summary>
public enum FileMoveCollisionOption
{
    // Add a numeric series suffix to the file name to avoid collision.
    /// <summary>
    /// The add serie value.
    /// </summary>
    AddSerie,

    // Add the file size to the file name to differentiate it.
    /// <summary>
    /// The add file size value.
    /// </summary>
    AddFileSize,

    // Overwrite the existing file at the destination.
    /// <summary>
    /// The overwrite value.
    /// </summary>
    Overwrite,

    // Discard the source file and keep the existing destination file.
    /// <summary>
    /// The discard from value.
    /// </summary>
    DiscardFrom,

    // Keep the larger file and discard the smaller one.
    /// <summary>
    /// The leave larger value.
    /// </summary>
    LeaveLarger,

    // Do not perform any manipulation; leave both files as they are.
    /// <summary>
    /// The dont manipulate value.
    /// </summary>
    DontManipulate,

    // Throw an exception when a collision is detected.
    /// <summary>
    /// The throw ex value.
    /// </summary>
    ThrowEx
}
