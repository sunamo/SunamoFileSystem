namespace SunamoFileSystem._sunamo.SunamoArgs;

public class GetFilesBaseArgsFS
{
    public bool TrimFirstPathAndLeadingBackslashes = false;
    internal Func<string, bool>? IsJunctionPoint = null;
    internal bool FollowJunctions = false;
}
