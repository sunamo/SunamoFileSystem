// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoArgs;

// todo nemělo by to dědit z GetFoldersEveryFolderArgs? ve vs2 to tak mám
internal class GetFilesArgsFS : GetFilesBaseArgsFS
{
    internal bool _trimA1AndLeadingBs = false;

    // todo s touhle třídou jsou jen problémy. udělat pořádek co tu má být a co tu nemám.
    internal bool _trimExt = false;
    internal bool byDateOfLastModifiedAsc = false;
    internal bool dontIncludeNewest = false;
    internal List<string> excludeFromLocationsCOntains = new();

    /// <summary>
    ///     Insert SunamoDevCodeHelper.RemoveTemporaryFilesVS etc.
    /// </summary>
    internal Action<List<string>> excludeWithMethod = null;

    internal Func<string, DateTime?> LastModifiedFromFn;

    /// <summary>
    ///     1-7-2020 changed to false, stil forget to mention and method is bad
    /// </summary>
    internal bool useMascFromExtension = false;

    internal bool wildcard = false;
}