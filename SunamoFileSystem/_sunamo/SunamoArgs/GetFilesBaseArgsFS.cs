namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesBaseArgsFS
{
    internal bool TrimFirstPathAndLeadingBackslashes = false;
    internal Func<string, bool>? IsJunctionPoint = null;
    internal bool FollowJunctions = false;
}
