namespace SunamoFileSystem._sunamo.SunamoGetFolders.Args;

internal class GetFoldersEveryFolderArgs : GetFilesArgsFS
{
    // Auto call WithEndSlash
    internal new bool TrimFirstPathAndLeadingBackslashes;

    internal new List<string>? ExcludeFromLocationsContains = null;

    internal bool WriteToDebugEveryLoadedFolder = false;

    internal GetFoldersEveryFolderArgs(GetFilesEveryFolderArgsFS e)
    {
        TrimFirstPathAndLeadingBackslashes = e.TrimFirstPathAndLeadingBackslashes;
        FollowJunctions = e.FollowJunctions;
        IsJunctionPoint = e.IsJunctionPoint;
    }

    internal GetFoldersEveryFolderArgs()
    {
    }
}
