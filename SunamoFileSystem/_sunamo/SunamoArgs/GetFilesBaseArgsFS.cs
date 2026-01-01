namespace SunamoFileSystem._sunamo.SunamoArgs;

/// <summary>
/// Base arguments for getting files
/// </summary>
internal class GetFilesBaseArgsFS
{
    internal bool TrimFirstPathAndLeadingBackslashes = false;
    internal Func<string, bool> IsJunctionPoint = null;
    internal bool FollowJunctions = false;
}