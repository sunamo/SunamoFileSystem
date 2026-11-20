namespace SunamoFileSystem._sunamo.SunamoArgs;

/// <summary>
/// Represents the get files base args FS type.
/// </summary>
public class GetFilesBaseArgsFS
{
    /// <summary>
    /// The trim first path and leading backslashes.
    /// </summary>
    public bool TrimFirstPathAndLeadingBackslashes = false;
    internal Func<string, bool>? IsJunctionPoint = null;
    internal bool FollowJunctions = false;
}
