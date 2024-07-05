namespace SunamoFileSystem._sunamo.SunamoArgs;


internal class GetFilesMoreMascArgs : GetFilesBaseArgsFS
{
    internal bool LoadFromFileWhenDebug = false;
    internal string path;
    internal string masc = "*";
    internal SearchOption searchOption = SearchOption.TopDirectoryOnly;
    internal bool deleteFromDriveWhenCannotBeResolved = false;
}