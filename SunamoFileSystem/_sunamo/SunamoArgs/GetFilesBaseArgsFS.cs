// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesBaseArgsFS /*: GetFoldersEveryFolderArgs - nevracet - číst koment výše*/
{
    internal bool _trimA1AndLeadingBs = false;
    internal Func<string, bool> dIsJunctionPoint = null;
    internal bool followJunctions = false;
}