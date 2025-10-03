namespace SunamoFileSystem._sunamo.SunamoArgs;

internal class GetFilesBaseArgsFS /*: GetFoldersEveryFolderArgs - nevracet - číst koment výše*/
{
    internal bool _trimA1AndLeadingBs = false;
    internal Func<string, bool> dIsJunctionPoint = null;
    internal bool followJunctions = false;
}