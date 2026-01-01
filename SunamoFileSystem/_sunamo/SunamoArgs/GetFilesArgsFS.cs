namespace SunamoFileSystem._sunamo.SunamoArgs;

/// <summary>
/// Arguments for getting files with advanced filtering options
/// TODO: Should this inherit from GetFoldersEveryFolderArgs?
/// TODO: This class has issues - need to clean up what should be here
/// </summary>
internal class GetFilesArgsFS : GetFilesBaseArgsFS
{
    internal new bool TrimFirstPathAndLeadingBackslashes = false;
    internal bool TrimExtension = false;
    internal bool ByDateOfLastModifiedAsc = false;
    internal bool DontIncludeNewest = false;
    internal List<string> ExcludeFromLocationsContains = new();

    /// <summary>
    /// Insert methods like SunamoDevCodeHelper.RemoveTemporaryFilesVS etc.
    /// </summary>
    internal Action<List<string>>? ExcludeWithMethod = null;

    internal Func<string, DateTime?>? LastModifiedFromFn;

    /// <summary>
    /// Changed to false on 1-7-2020, still forget to mention and method is problematic
    /// </summary>
    internal bool UseMascFromExtension = false;

    internal bool Wildcard = false;
}