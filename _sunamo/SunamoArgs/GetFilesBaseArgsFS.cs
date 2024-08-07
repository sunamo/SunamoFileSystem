namespace SunamoFileSystem._sunamo.SunamoArgs;

/*
dříve dědila z GetFoldersEveryFolderArgs a ji dědil GetFilesArgs
Protože potřebuji univerzání data pro získávání souborů skrze různé metody (GetFiles, GetFilesMoreMasc atd.), vypadá teď takto
dávalo smysl i to co jsem měl, jelikož GetFilesEveryFolder
volalo GetFoldersEveryFolder takže jsem si z toho vzal jen subset z bázové třídy
*/
internal class GetFilesBaseArgsFS /*: GetFoldersEveryFolderArgs - nevracet - číst koment výše*/
{
    internal bool _trimA1AndLeadingBs = false;
    internal Func<string, bool> dIsJunctionPoint = null;
    internal bool followJunctions = false;
}