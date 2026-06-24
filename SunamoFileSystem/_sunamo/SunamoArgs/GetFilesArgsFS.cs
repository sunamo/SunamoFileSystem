namespace SunamoFileSystem._sunamo.SunamoArgs;

// TODO: Should this inherit from GetFoldersEveryFolderArgs?
// TODO: This class has issues - need to clean up what should be here
internal class GetFilesArgsFS : GetFilesBaseArgsFS
{
    internal new bool TrimFirstPathAndLeadingBackslashes = false;
    internal bool TrimExtension = false;
    internal bool ByDateOfLastModifiedAsc = false;
    internal bool DontIncludeNewest = false;
    internal List<string> ExcludeFromLocationsContains = new();

    // Insert methods like SunamoDevCodeHelper.RemoveTemporaryFilesVS etc.
    internal Action<List<string>>? ExcludeWithMethod = null;

    internal Func<string, DateTime?>? LastModifiedFromFn = null;

    // Changed to false on 1-7-2020, still forget to mention and method is problematic
    internal bool UseMascFromExtension = false;

    internal bool Wildcard = false;
}
