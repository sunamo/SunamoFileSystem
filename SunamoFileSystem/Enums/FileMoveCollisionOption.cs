namespace SunamoFileSystem.Enums;

public enum FileMoveCollisionOption
{
    // Add a numeric series suffix to the file name to avoid collision.
    AddSerie,

    // Add the file size to the file name to differentiate it.
    AddFileSize,

    // Overwrite the existing file at the destination.
    Overwrite,

    // Discard the source file and keep the existing destination file.
    DiscardFrom,

    // Keep the larger file and discard the smaller one.
    LeaveLarger,

    // Do not perform any manipulation; leave both files as they are.
    DontManipulate,

    // Throw an exception when a collision is detected.
    ThrowEx
}
