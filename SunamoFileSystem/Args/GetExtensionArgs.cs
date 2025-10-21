// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem.Args;

/// <summary>
/// In original exists only returnOriginalCase
/// </summary>
public class GetExtensionArgs
{
    /// <summary>
    /// If not, everything will be lower
    /// </summary>
    public bool returnOriginalCase = false;
    public bool filesWoExtReturnAsIs = false;
}