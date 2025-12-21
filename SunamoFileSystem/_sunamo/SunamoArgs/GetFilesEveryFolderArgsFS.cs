namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesEveryFolderArgsFS : GetFilesBaseArgsFS
{
    internal Action Done;

    internal Action DoneOnePercent;

    // return false if no to indexed. otherwise true
    internal Func<string, bool> FilterFoundedFiles;
    internal Func<string, bool> FilterFoundedFolders;
    internal int getNullIfThereIsMoreThanXFiles = -1;
    internal Action<double> InsertPb = null;
    internal Action<double> InsertPbTime = null;
    internal Action<string> UpdateTbPb = null;
    internal bool usePb = false;
    internal bool usePbTime = false;
}