// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem.Enums;
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
