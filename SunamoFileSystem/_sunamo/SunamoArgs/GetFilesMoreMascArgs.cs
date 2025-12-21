namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesMoreMascArgs : GetFilesBaseArgsFS
{
    internal bool deleteFromDriveWhenCannotBeResolved = false;
    internal bool LoadFromFileWhenDebug = false;
    internal string masc = "*";
    internal string path;
    internal SearchOption searchOption = SearchOption.TopDirectoryOnly;
}