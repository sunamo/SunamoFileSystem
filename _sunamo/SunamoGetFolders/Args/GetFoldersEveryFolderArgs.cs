namespace SunamoFileSystem._sunamo.SunamoGetFolders.Args;

internal class GetFoldersEveryFolderArgs : GetFilesArgsFS
{
    /// <summary>
    ///     Auto call WithEndSlash
    /// </summary>
    internal bool _trimA1AndLeadingBs;

    internal List<string> excludeFromLocationsCOntains = null;

    // nevím k čemu to je ale zdá se nesmysl, ověřovat můžu přes excludeFromLocationsCOntains != null
    //internal bool excludeFromLocationsCOntainsBool = false;
    internal bool writeToDebugEveryLoadedFolder = false;

    internal GetFoldersEveryFolderArgs(GetFilesEveryFolderArgsFS e)
    {
        _trimA1AndLeadingBs = e._trimA1AndLeadingBs;
        followJunctions = e.followJunctions;
        dIsJunctionPoint = e.dIsJunctionPoint;
    }

    internal GetFoldersEveryFolderArgs()
    {
    }
}