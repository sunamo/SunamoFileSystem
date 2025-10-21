// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesMoreMascArgs : GetFilesBaseArgsFS
{
    internal bool deleteFromDriveWhenCannotBeResolved = false;
    internal bool LoadFromFileWhenDebug = false;
    internal string masc = "*";
    internal string path;
    internal SearchOption searchOption = SearchOption.TopDirectoryOnly;
}