namespace SunamoFileSystem.Enums;

// variables names: ok
// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
public enum FileMoveCollisionOption
{
    AddSerie,
    AddFileSize,
    Overwrite,
    DiscardFrom,
    LeaveLarger,
    DontManipulate,
    ThrowEx
}