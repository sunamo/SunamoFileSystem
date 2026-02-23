namespace SunamoFileSystem.Enums;

/// <summary>
/// Specifies how to handle collisions when moving a file to a location where a file already exists.
/// </summary>
public enum FileMoveCollisionOption
{
    /// <summary>
    /// Add a numeric series suffix to the file name to avoid collision.
    /// </summary>
    AddSerie,

    /// <summary>
    /// Add the file size to the file name to differentiate it.
    /// </summary>
    AddFileSize,

    /// <summary>
    /// Overwrite the existing file at the destination.
    /// </summary>
    Overwrite,

    /// <summary>
    /// Discard the source file and keep the existing destination file.
    /// </summary>
    DiscardFrom,

    /// <summary>
    /// Keep the larger file and discard the smaller one.
    /// </summary>
    LeaveLarger,

    /// <summary>
    /// Do not perform any manipulation; leave both files as they are.
    /// </summary>
    DontManipulate,

    /// <summary>
    /// Throw an exception when a collision is detected.
    /// </summary>
    ThrowEx
}