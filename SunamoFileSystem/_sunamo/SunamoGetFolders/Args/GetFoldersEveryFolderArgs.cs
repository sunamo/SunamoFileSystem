namespace SunamoFileSystem._sunamo.SunamoGetFolders.Args;

/// <summary>
/// Arguments for getting folders from every folder recursively
/// </summary>
internal class GetFoldersEveryFolderArgs : GetFilesArgsFS
{
    /// <summary>
    /// Auto call WithEndSlash
    /// </summary>
    internal new bool TrimFirstPathAndLeadingBackslashes;

    internal new List<string> ExcludeFromLocationsContains = null;

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