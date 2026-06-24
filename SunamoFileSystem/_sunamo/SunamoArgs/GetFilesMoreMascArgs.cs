namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesMoreMascArgs : GetFilesBaseArgsFS
{
    internal bool DeleteFromDriveWhenCannotBeResolved = false;
    internal bool LoadFromFileWhenDebug = false;
    internal string Masc = "*";
    internal string Path = string.Empty;
    internal SearchOption SearchOption = SearchOption.TopDirectoryOnly;
}
