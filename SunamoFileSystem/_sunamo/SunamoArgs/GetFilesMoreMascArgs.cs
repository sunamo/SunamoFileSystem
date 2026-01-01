namespace SunamoFileSystem._sunamo.SunamoArgs;

/// <summary>
/// Arguments for getting files with multiple mask patterns
/// </summary>
internal class GetFilesMoreMascArgs : GetFilesBaseArgsFS
{
    internal bool DeleteFromDriveWhenCannotBeResolved = false;
    internal bool LoadFromFileWhenDebug = false;
    internal string Masc = "*";
    internal string Path = string.Empty;
    internal SearchOption SearchOption = SearchOption.TopDirectoryOnly;
}