namespace SunamoFileSystem.Enums;

public enum DirectoryMoveCollisionOption
{
    // Add a numeric series suffix to the directory name to avoid collision.
    AddSerie,

    // Overwrite the existing directory at the destination.
    Overwrite,

    // Discard the source directory and keep the existing destination.
    DiscardFrom,

    // Throw an exception when a collision is detected.
    ThrowEx
}
