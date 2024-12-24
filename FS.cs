namespace SunamoFileSystem;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;
using SunamoFileSystem._sunamo;
using PathMs = Path;

/// <summary>
///     FSXlf - postfixy jsou píčovina. volám v tom metody stejné třídy. Můžu nahradit FS. v SunExc ale musel bych to
///     zkopírovat zpět. to nese riziko že jsem přidal novou metodu kterou bych překopírováním ztratil. Krom toho to nedrží
///     konvenci. V názvu souboru to nechám ať vidím na první dobrou co je co.
/// </summary>
public class FS
{
    public const string dEndsWithReplaceInFile = "SubdomainHelperSimple.cs";

    protected static readonly List<char> invalidFileNameChars = Path.GetInvalidFileNameChars().ToList();
    protected static readonly List<string> invalidFileNameStrings;

    public static bool IsAbsolutePath(string path)
    {
        return !String.IsNullOrWhiteSpace(path)
            && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1
            && Path.IsPathRooted(path)
            && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
    }

    /// <summary>
    /// Use CopyAllFilesRecursively instead
    /// </summary>
    /// <param name="sourceDir"></param>
    /// <param name="targetDir"></param>
    public static void CopyFolder(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);

        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

        foreach (var directory in Directory.GetDirectories(sourceDir))
            CopyFolder(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
    }

    protected static List<char> s_invalidPathChars;

    /// <summary>
    ///     Field as string because I dont have array and must every time ToArray() to construct string
    /// </summary>
    public static string s_invalidFileNameCharsString;

    public static List<char> s_invalidFileNameChars;
    protected static List<char> s_invalidCharsForMapPath;
    protected static List<char> s_invalidFileNameCharsWithoutDelimiterOfFolders;

    public static string replaceIncorrectFor = string.Empty;

    public static Action<string> DeleteFileMaybeLocked;


    public static Func<string, bool, List<Process>> fileUtilWhoIsLocking = null;

    private static Type type = typeof(FS);


    static FS()
    {
        invalidFileNameStrings = new List<string>(invalidFileNameChars.Count);
        foreach (var item in invalidFileNameChars) invalidFileNameStrings.Add(item.ToString());

        s_invalidPathChars = new List<char>(Path.GetInvalidPathChars());
        if (!s_invalidPathChars.Contains('/')) s_invalidPathChars.Add('/');
        if (!s_invalidPathChars.Contains('\\')) s_invalidPathChars.Add('\\');


        s_invalidFileNameChars = new List<char>(invalidFileNameChars);
        s_invalidFileNameCharsString = string.Join("", invalidFileNameChars);
        for (var i = (char)65529; i < 65534; i++) s_invalidFileNameChars.Add(i);

        s_invalidCharsForMapPath = new List<char>();
        s_invalidCharsForMapPath.AddRange(s_invalidFileNameChars.ToArray());
        foreach (var item in invalidFileNameChars)
            if (!s_invalidCharsForMapPath.Contains(item))
                s_invalidCharsForMapPath.Add(item);

        s_invalidCharsForMapPath.Remove('/');

        s_invalidFileNameCharsWithoutDelimiterOfFolders = new List<char>(s_invalidFileNameChars.ToArray());

        s_invalidFileNameCharsWithoutDelimiterOfFolders.Remove('\\');
        s_invalidFileNameCharsWithoutDelimiterOfFolders.Remove('/');
    }

    public static void CreateUpfoldersPsysicallyUnlessThere(string nad)
    {
        CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(nad));
    }

    public static bool ExistsDirectory(string p)
    {
        return Directory.Exists(p);
    }

    /// <summary>
    ///     Create all upfolders of A1 with, if they dont exist
    /// </summary>
    /// <param name="nad"></param>
    public static void CreateFoldersPsysicallyUnlessThere(string nad)
    {
        ThrowEx.IsNullOrEmpty("nad", nad);
        //ThrowEx.IsNotWindowsPathFormat("nad", nad);


        if (Directory.Exists(nad)) return;

        var slozkyKVytvoreni = new List<string>
        {
            nad
        };

        while (true)
        {
            nad = Path.GetDirectoryName(nad);

            // TODO: Tady to nefunguje pro UWP/UAP apps protoze nemaji pristup k celemu disku. Zjistit co to je UWP/UAP/... a jak v nem ziskat/overit jakoukoliv slozku na disku
            if (Directory.Exists(nad)) break;

            var kopia = nad;
            slozkyKVytvoreni.Add(kopia);
        }

        slozkyKVytvoreni.Reverse();
        foreach (var item in slozkyKVytvoreni)
        {
            var folder = item;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        }
    }

    /// <summary>
    ///     All occurences Path's method in sunamo replaced
    /// </summary>
    /// <param name="v"></param>
    public static void CreateDirectory(string v)
    {
        try
        {
            Directory.CreateDirectory(v);
        }
        catch (NotSupportedException)
        {
        }
    }


    public static void CreateDirectoryIfNotExists(string p)
    {
        MakeUncLongPath(ref p);

        if (!ExistsDirectory(p)) Directory.CreateDirectory(p);
    }

    public static string WithEndSlash(string v)
    {
        return WithEndSlash(ref v);
    }

    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static string WithEndSlash(ref string v)
    {
        if (v != string.Empty) v = v.TrimEnd('\\') + '\\';

        FirstCharUpper(ref v);
        return v;
    }

    public static List<string> FoldersWithSubfolder(string solutionFolder, string folderName)
    {
        var subFolders = Directory.GetDirectories(solutionFolder, "*", SearchOption.AllDirectories);
        var result = new List<string>();

        foreach (var item in subFolders)
        {
            /*
Zde mám chybu:
System.IO.DirectoryNotFoundException: 'Could not find a part of the path
            'E:\vs\Projects\PlatformIndependentNuGetPackages.net\Clients\node_modules\napi-wasm'.'

            to musí být nějaká <|>, protože zde se mi to má dostat jen při sunamo nebo swod
            nikoliv při sunamo.net
            */

            var subf = Directory.GetDirectories(item, folderName, SearchOption.TopDirectoryOnly).ToList();
            if (subf.Count == 1) result.Add(item);
        }

        return result;
    }


    public static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1) return nazevPP.ToUpper();

        var sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }

    public static bool TryDeleteFile(string item)
    {
        // TODO: To all code message logging as here

        try
        {
            // If file won't exists, wont throw any exception
            File.Delete(item);
            return true;
        }
        catch
        {
            //ThisApp.Error(sess.i18n(XlfKeys.FileCanTBeDeleted) + ": " + item);
            return false;
        }
    }

    public static async Task WriteAllTextWithExc(string file, string obsah)
    {
        try
        {
            await File.WriteAllTextAsync(file, obsah);
        }
        catch (Exception ex)
        {
            //TypedSunamoLogger.Instance.Error//(Exceptions.TextOfExceptions(ex));
        }
    }

    public static async void CreateFileIfDoesntExists(string path)
    {
        //CreateFileIfDoesntExists<string, string>(path, null);


        if (!File.Exists(path))
            //TF.WriteAllBytes<StorageFolder, StorageFile>(path, CAG.ToList<byte>(), ac);
            File.WriteAllText(path, "");
    }
    //public static async Task CreateFileIfDoesntExists<StorageFolder, StorageFile>(StorageFile path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    await File.WriteAllBytesAsync(path.ToString(), new Byte[] { });

    //    //if (!ExistsFile<StorageFolder, StorageFile>(path, ac))
    //    //{
    //    //    TF.WriteAllBytes<StorageFolder, StorageFile>(path, CAG.ToList<byte>(), ac);
    //    //}
    //}

    public static string InsertBetweenFileNameAndExtension(string orig, string whatInsert)
    {
        //return InsertBetweenFileNameAndExtension<string, string>(orig, whatInsert, null);

        // Cesta by se zde hodila kvůli FS.CiStorageFile
        // nicméně StorageFolder nevím zda se používá, takže to bude umět i bez toho

        var origS = orig;

        var fn = Path.GetFileNameWithoutExtension(origS);
        var e = GetExtension(origS);

        if (origS.Contains('/') || origS.Contains('\\'))
        {
            var p = Path.GetDirectoryName(origS);

            return Path.Combine(p, fn + whatInsert + e);
        }

        return fn + whatInsert + e;
    }

    /// <summary>
    ///     ReplaceIncorrectCharactersFile - can specify char for replace with
    ///     ReplaceInvalidFileNameChars - all wrong chars skip
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string ReplaceInvalidFileNameChars(string filename, params char[] ch)
    {
        var sb = new StringBuilder();
        foreach (var item in filename)
            if (!s_invalidFileNameChars.Contains(item) || ch.Contains(item))
                sb.Append(item);
        return sb.ToString();
    }

    /// <summary>
    /// Vrátí vč. cesty
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="whatInsert"></param>
    //public static StorageFile InsertBetweenFileNameAndExtension<StorageFolder, StorageFile>(StorageFile orig, string whatInsert, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    // Cesta by se zde hodila kvůli FS.CiStorageFile
    //    // nicméně StorageFolder nevím zda se používá, takže to bude umět i bez toho

    //    var origS = orig.ToString();

    //    string fn = Path.GetFileNameWithoutExtension(origS);
    //    string e = GetExtension(origS);

    //    if (origS.Contains('/') || origS.Contains('\\'))
    //    {
    //        string p = Path.GetDirectoryName(origS);

    //        return CiStorageFile<StorageFolder, StorageFile>(Path.Combine(p, fn + whatInsert + e), ac);
    //    }
    //    return CiStorageFile<StorageFolder, StorageFile>(fn + whatInsert + e, ac);
    //}

    /// <summary>
    ///     .babelrc etc. return as is. but files which not contains only alphanumeric will be returned when A3 (and A2 is then
    ///     ignored)
    ///     ALL EXT. HAVE TO BE ALWAYS LOWER
    ///     Return in lowercase
    /// </summary>
    /// <param name="v"></param>
    public static string GetExtension(string v, GetExtensionArgs a = null)
    {
        if (a == null) a = new GetExtensionArgs();

        var result = "";
        var lastDot = v.LastIndexOf('.');
        if (lastDot == -1) return string.Empty;
        var lastSlash = v.LastIndexOf('/');
        var lastBs = v.LastIndexOf('\\');
        if (lastSlash > lastDot) return string.Empty;
        if (lastBs > lastDot) return string.Empty;
        result = v.Substring(lastDot);

        if (!IsExtension(result))
        {
            if (a.filesWoExtReturnAsIs) return result;
            return string.Empty;
        }

        if (!a.returnOriginalCase) result = result.ToLower();


        return result;
    }

    public static bool IsExtension(string result)
    {
        if (string.IsNullOrWhiteSpace(result)) return false;
        if (!result.TrimStart('.').ToLower()
                .All(c => (char.IsLetter(c) && char.IsLower(c)) || char.IsDigit(c))) return false;
        return true;
    }

    //public static StorageFile CiStorageFile<StorageFolder, StorageFile>(string path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        return (dynamic)path.ToString();
    //    }
    //    return ac.fs.ciStorageFile.Invoke(path);
    //}

    public static bool ExistsFile(string p)
    {
        return File.Exists(p);
    }


    public static void MoveSubfoldersToFolder(List<string> subfolderNames, string from, string to,
        FileMoveCollisionOption fo)
    {
        foreach (var item in subfolderNames)
        {
            var f = Path.Combine(from, item);
            var t = Path.Combine(to, item);

            MoveAllRecursivelyAndThenDirectory(f, t, fo);
        }
    }

    public static void TrimBasePathAndTrailingBs(List<string> s, string basePath)
    {
        for (var i = 0; i < s.Count; i++)
        {
            s[i] = s[i].Substring(basePath.Length);

            s[i] = s[i].TrimEnd('\\');
        }
    }

    public static string GetFileNameWithoutOneExtension(string path)
    {
        return SHParts.RemoveAfterLast(path, "\\");
    }

    public static string GetActualDateTime()
    {
        var dt = DateTime.Now;
        return ReplaceIncorrectCharactersFile(dt.ToString());
    }

    public static
#if ASYNC
        async Task<List<string>>
#else
List<string>
#endif
        KeepOnlyWhichIsNotInFiles(List<string> opts, List<string> paths)
    {
        var c = new CollectionWithoutDuplicates<string>();
        foreach (var item in paths)
            c.AddRange(SHGetLines.GetLines(
#if ASYNC
                await
#endif
                    File.ReadAllTextAsync(item)).ToList());

        CAG.CompareList(opts, c.c);

        return opts;
    }

    /// <summary>
    ///     C:\repos\EOM-7\Marvin\Module.VBtO\Clients\src\apps\vbto\src\pages\Administration\Administration.test.tsx
    ///     ../../../../../../../node_modules/@mui/material/Switch/Switch
    ///     => C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@mui\material\Switch\Switch
    ///     => OK
    /// </summary>
    /// <param name="fullPathToSecondFile"></param>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string RelativeToAbsolutePath(string fullPathToSecondFile, string relativePath)
    {
        var fullPathToFirstFile =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(fullPathToSecondFile), relativePath));
        return fullPathToFirstFile;
    }

    // Proč to volám zde? Má se to volat v aplikacích kde to potřebuji
    //static AllExtensionsHelper()
    //{
    //    // Must call Initialize here, not in Loaded of Window. when I run auto code in debug, it wont be initialized as is needed.
    //    Initialize();
    //}

    //public static void Initialize(bool callAlsoAllExtensionsHelperWithoutDotInitialize = false)
    //{
    //    if (callAlsoAllExtensionsHelperWithoutDotInitialize)
    //    {
    //        AllExtensionsHelperWithoutDot.Initialize();
    //    }
    //}

    /// <summary>
    ///     Usage: SunamoFubuCsprojFileHelper.GetProjectsInSlnFile
    ///     Cant name GetAbsolutePath because The call is ambiguous between the following methods or properties:
    ///     'CAChangeContent.ChangeContent0(null,List
    ///     <string>
    ///         , Func
    ///         <string, string, string>)' and 'CAChangeContent.ChangeContent0(null,List<string>, Func<string, string>)'
    /// </summary>
    /// <param name="a"></param>
    public static string AbsoluteFromCombinePath(string a)
    {
        var r = Path.GetFullPath(new Uri(a).LocalPath);
        return r;
    }


    public static string WrapWithQm(string item, bool? forceNotIncludeQm)
    {
        if (item.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return SH.WrapWithQm(item);
        return item;
    }

    public static List<string> FilterInRootAndInSubFolder(string rf, List<string> fs)
    {
        WithEndSlash(ref rf);

        var c = rf.Length;

        var subFolder = new List<string>(fs.Count);

        for (var i = fs.Count - 1; i >= 0; i--)
        {
            var item = fs[i];
            if (item.Substring(c).Contains("\""))
            {
                subFolder.Add(item);
                fs.RemoveAt(i);
            }
        }

        return subFolder;
    }

    public static void OnlyNames(List<string> subfolders)
    {
        for (var i = 0; i < subfolders.Count; i++) subfolders[i] = Path.GetFileName(subfolders[i]);
    }


    public static List<string> FilesWhichContainsAll(object sunamo, string masc, params string[] mustContains)
    {
        return FilesWhichContainsAll(sunamo, masc, mustContains);
    }


    public static string PathSpecialAndLevel(string basePath, string item, int v)
    {
        basePath = basePath.Trim('\\');

        item = item.Trim('\\');

        item = item.Replace(basePath, string.Empty);
        var pBasePath = SHSplit.SplitMore(basePath, "\"");
        var basePathC = pBasePath.Count;

        var p = SHSplit.SplitMore(item, "\"");
        var i = 0;
        for (; i < p.Count; i++)
            if (p[i].StartsWith("_"))
                pBasePath.Add(p[i]);
            else
                //i--;
                break;
        for (var y = 0; y < i; y++) p.RemoveAt(0);

        var h = p.Count - i + basePathC;
        var to = Math.Min(v, h);
        i = 0;
        for (; i < to; i++) pBasePath.Add(p[i]);

        return string.Join("\"", pBasePath);
    }

    public static string GetDirectoryNameIfIsFile(string f)
    {
        if (File.Exists(f)) return Path.GetDirectoryName(f);
        return f;
    }

    public static string MaskFromExtensions(List<string> allExtensions)
    {
        for (var i = 0; i < allExtensions.Count; i++) allExtensions[i] = "*" + allExtensions[i];

        //CA.Prepend("*", allExtensions);
        return string.Join(",", allExtensions);
    }


    //public static string GetRelativePath(string relativeTo, string path)
    //{
    //    return SunamoExceptions.Path.GetRelativePath(relativeTo, path);
    //}

    //public static bool IsAbsolutePath(string path)
    //{
    //    return SunamoExceptions.FS.IsAbsolutePath(path);
    //}


    /// <summary>
    ///     RenameNumberedSerieFiles - Rename files by linear names - 0,1,...,x
    /// </summary>
    /// <param name="d"></param>
    /// <param name="p"></param>
    /// <param name="startFrom"></param>
    /// <param name="ext"></param>
    public static void RenameNumberedSerieFiles(List<string> d, string p, int startFrom, string ext)
    {
        var masc = MascFromExtension(ext);
        var f = FSGetFiles.GetFiles(p, masc, SearchOption.TopDirectoryOnly);
        RenameNumberedSerieFiles(d, f, startFrom, ext);
    }

    /// <summary>
    ///     A1 is new names of files without extension. Can use LinearHelper
    /// </summary>
    /// <param name="d"></param>
    /// <param name="p"></param>
    /// <param name="startFrom"></param>
    /// <param name="ext"></param>
    public static void RenameNumberedSerieFiles(List<string> d, List<string> f, int startFrom, string ext)
    {
        var p = Path.GetDirectoryName(f[0]);

        if (f.Count >= d.Count)
        {
            var fCountMinusONe = f.Count - 1;

            //var r = f.First();
            for (var i = startFrom; ; i++)
            {
                if (fCountMinusONe < i) break;
                var r = f[i];
                var t = p + i + ext;
                if (f.Contains(t))
                    //break;
                    continue;
                // AddSerie is useless coz file never will be exists
                //FS.RenameFile(t, d[i - startFrom] + ext, FileMoveCollisionOption.AddSerie);
                RenameFile(r, t, FileMoveCollisionOption.AddSerie);
            }
        }
    }

    /// <summary>
    ///     Get path A2/name folder of file A1/name A1
    /// </summary>
    /// <param name="var"></param>
    /// <param name="zmenseno"></param>
    public static string PlaceInFolder(string var, string zmenseno)
    {
        //return Slozka.ci.PridejNadslozku(var, zmenseno);
        var nad = Path.GetDirectoryName(var);
        var naz = Path.GetFileName(nad);
        return Path.Combine(zmenseno, Path.Combine(naz, Path.GetFileName(var)));
    }


    /// <summary>
    ///     Všechny soubory které se podaří přesunout vymažu z A1
    ///     A1 MUST BE WITH EXTENSION
    ///     A4 can be null if !A5
    ///     In A1 will keep files which doesnt exists in A3
    ///     A4 is files from A1 which wasnt founded in A2
    ///     A7 is files
    /// </summary>
    /// <param name="filesFrom"></param>
    /// <param name="folderFrom"></param>
    /// <param name="folderTo"></param>
    /// <param name="wasntExistsInFrom"></param>
    /// <param name="mustExistsInTarget"></param>
    /// <param name="copy"></param>
    public static void CopyMoveFilesInList(List<string> filesFrom, string folderFrom, string folderTo,
        List<string> wasntExistsInFrom, bool mustExistsInTarget, bool copy, Dictionary<string, List<string>> files,
        bool overwrite = true)
    {
        WithoutEndSlash(folderFrom);
        WithoutEndSlash(folderTo);


        //CA.RemoveStringsEmpty2(filesFrom);
        var existsFileTo = false;
        for (var i = filesFrom.Count - 1; i >= 0; i--)
        {
            filesFrom[i] = filesFrom[i].Replace(folderFrom, string.Empty);
            var oldPath = folderFrom + filesFrom[i];
            if (files != null)
            {
                var oldPath2 = files[filesFrom[i]].FirstOrDefault();
                if (oldPath2 != null) oldPath = oldPath2;
            }
#if DEBUG
            ///DebugLogger.DebugWriteLine("Taken: " + oldPath);
#endif
            var newPath = folderTo + filesFrom[i];
            if (!File.Exists(oldPath))
            {
                if (wasntExistsInFrom != null) wasntExistsInFrom.Add(filesFrom[i]);
                filesFrom.RemoveAt(i);
                continue;
            }

            if (!File.Exists(newPath) && mustExistsInTarget) continue;

            existsFileTo = File.Exists(newPath);
            if ((existsFileTo && overwrite) || !existsFileTo)
            {
                if (copy)
                    CopyFile(oldPath, newPath, FileMoveCollisionOption.Overwrite);
                else
                    MoveFile(oldPath, newPath, FileMoveCollisionOption.Overwrite);
            }

            filesFrom.RemoveAt(i);
        }
    }

    public static void CopyMoveFilesInListSimple(List<string> f, string basePathCjHtml1, string basePathCjHtml2,
        bool copy, bool overwrite = true)
    {
        List<string> wasntExistsInFrom = null;
        var mustExistsInTarget = false;
        CopyMoveFilesInList(f, basePathCjHtml1, basePathCjHtml2, wasntExistsInFrom, mustExistsInTarget, copy, null,
            false);
    }

    public static void CreateInOtherLocationSameFolderStructure(string from, string to)
    {
        WithEndSlash(from);
        WithEndSlash(to);
        var folders = Directory.GetDirectories(from, "*", SearchOption.AllDirectories);
        foreach (var item in folders)
        {
            var nf = item.Replace(from, to);
            CreateFoldersPsysicallyUnlessThere(nf);
        }
    }

    /// <summary>
    ///     A1 must be with extensions!
    /// </summary>
    /// <param name="files"></param>
    /// <param name="folderFrom"></param>
    /// <param name="folderTo"></param>
    public static void CopyMoveFromMultiLocationIntoOne(List<string> files, string folderFrom, string folderTo)
    {
        var wasntExists = new List<string>();

        var files2 = new Dictionary<string, List<string>>();
        var getFiles = FSGetFiles.GetFiles(folderFrom, "*.cs", SearchOption.AllDirectories,
            new GetFilesArgsFS { excludeFromLocationsCOntains = new List<string>(["TestFiles"]) });
        foreach (var item in files) files2.Add(item, getFiles.Where(d => Path.GetFileName(d) == item).ToList());
        CopyMoveFilesInList(files, folderFrom, folderTo, wasntExists, false, true, files2);
        ////DebugLogger.Instance.WriteList(wasntExists);
    }


    //public static string StorageFilePath<StorageFolder, StorageFile>(StorageFile item, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        ac.fs.storageFilePath.Invoke(item);
    //    }
    //    return item.ToString();
    //}

    //public static List<StorageFile> GetFilesOfExtensionCaseInsensitiveRecursively<StorageFolder, StorageFile>(StorageFolder sf, string ext, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{

    //    if (ac != null)
    //    {
    //        return ac.GetFilesOfExtensionCaseInsensitiveRecursively.Invoke(sf, ext);
    //    }
    //    List<StorageFile> files = new List<StorageFile>();
    //    files = GetFilesInterop<StorageFolder, StorageFile>(sf, "*", true, ac);
    //    for (int i = files.Count - 1; i >= 0; i--)
    //    {
    //        dynamic file = files[i];
    //        if (!file.ToLower().EndsWith(ext))
    //        {
    //            files.RemoveAt(i);
    //        }
    //    }
    //    return files;
    //}
    //public static List<StorageFile> GetFilesInterop<StorageFolder, StorageFile>(StorageFolder folder, string mask, bool recursive, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.GetFiles.Invoke(folder, mask, recursive);
    //    }
    //    // folder is StorageFolder
    //    var files = GetFiles(folder.ToString(), mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    //    return CAG.ToList<StorageFile>((dynamic)files);
    //}

    //public static Stream OpenStreamForReadAsync<StorageFolder, StorageFile>(StorageFile file, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.fs.openStreamForReadAsync.Invoke(file);
    //    }
    //    return FS.OpenStream(file.ToString());
    //}

    private static Stream OpenStream(string v)
    {
        return new FileStream(v, FileMode.OpenOrCreate);
    }

    //public static bool IsFoldersEquals<StorageFolder, StorageFile>(StorageFolder parent, StorageFolder path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.fs.isFoldersEquals.Invoke(parent, path);
    //    }
    //    var f1 = parent.ToString();
    //    var f2 = path.ToString();
    //    return f1 == f2;
    //}

    //public static string GetFileName<StorageFolder, StorageFile>(StorageFile item, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.Path.GetFileName.Invoke(item);
    //    }
    //    return Path.GetFileName(item.ToString());
    //}

    /// <summary>
    ///     A1 must be sunamo.Data.StorageFolder or uwp StorageFolder
    ///     Return fixed string is here right
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="v"></param>
    //public static StorageFile GetStorageFile<StorageFolder, StorageFile>(StorageFolder folder, string v, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ((dynamic)ac.fs.getStorageFile(folder, v)).Path;
    //    }
    //    return (dynamic)Path.Combine(folder.ToString(), v);
    //}
    public static
#if ASYNC
        async Task
#else
void
#endif
        DeleteEmptyFiles(string folder, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(folder, "*.*", so);
        foreach (var item in files)
        {
            var fs = new FileInfo(item).Length;
            if (fs == 0)
                TryDeleteFile(item);
            else if (fs < 4)
                if ((
#if ASYNC
                        await
#endif
                            File.ReadAllTextAsync(item)).Trim() == string.Empty)
                    TryDeleteFile(item);
        }
    }

    private static async Task ReplaceInAllFilesWorker(object o, Func<string, bool> EncodingHelperIsBinary)
    {
        var p = (ReplaceInAllFilesArgs)o;

        #region ReplaceInAllFilesArgsBase - Zkopírovat i do ReplaceInAllFilesWorker. Viz comment níže

        // musím to rozdělit na jednotlivé proměnné abych viděl co se používá a co ne. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
        var fasterMethodForReplacing = p.fasterMethodForReplacing;
        var files = p.files;
        var inDownloadedFolders = p.inDownloadedFolders;
        var inFoldersToDelete = p.inFoldersToDelete;
        var inGitFiles = p.inGitFiles;
        var isMultilineWithVariousIndent = p.isMultilineWithVariousIndent;
        var writeEveryReadedFileAsStatus = p.writeEveryReadedFileAsStatus;
        var writeEveryWrittenFileAsStatus = p.writeEveryWrittenFileAsStatus;

        #endregion

        #region ReplaceInAllFilesArgs

        var from = p.from;
        var to = p.to;
        var pairLinesInFromAndTo = p.pairLinesInFromAndTo;
        var replaceWithEmpty = p.replaceWithEmpty;
        var isNotReplaceInTemporaryFiles = p.isNotReplaceInTemporaryFiles;

        #endregion

        if (isMultilineWithVariousIndent)
        {
            from = SHReplace.ReplaceAllDoubleSpaceToSingle2(from);
            to = SHReplace.ReplaceAllDoubleSpaceToSingle2(to);
        }

        if (pairLinesInFromAndTo)
        {
            var from2 = SHSplit.SplitMore(from, Environment.NewLine);
            var to2 = SHSplit.SplitMore(to, Environment.NewLine);

            if (replaceWithEmpty)
            {
                to2.Clear();
                foreach (var item in from2) to2.Add(string.Empty);
            }

            ThrowEx.DifferentCountInLists("from2", from2, "to2", to2);
            await ReplaceInAllFiles(from2, to2, o as ReplaceInAllFilesArgsBase, EncodingHelperIsBinary);
        }
        else
        {
            await ReplaceInAllFiles(new List<string>([from]), new List<string>([to])
                , o as ReplaceInAllFilesArgsBase, EncodingHelperIsBinary);
        }
    }


    public static async Task ReplaceInAllFiles(string from, string to, ReplaceInAllFilesArgsBase o,
        Func<string, bool> EncodingHelperIsBinary)
    {
        var r = new ReplaceInAllFilesArgs(o);
        r.from = from;
        r.to = to;

        await ReplaceInAllFilesWorker(r, EncodingHelperIsBinary);

        //Thread t = new Thread(new ParameterizedThreadStart(ReplaceInAllFilesWorker));
        //t.Start(r);
    }


    public static async Task ReplaceInAllFiles(string folder, string extension, List<string> replaceFrom,
        List<string> replaceTo, bool isMultilineWithVariousIndent, Func<string, bool> EncodingHelperIsBinary)
    {
        var files = FSGetFiles.GetFiles(folder, MascFromExtension(extension), SearchOption.AllDirectories);
        ThrowEx.DifferentCountInLists("replaceFrom", replaceFrom, "replaceTo", replaceTo);
        Func<StringBuilder, IList<string>, IList<string>, StringBuilder> fasterMethodForReplacing = null;
        await ReplaceInAllFiles(replaceFrom, replaceTo,
            new ReplaceInAllFilesArgsBase
            {
                files = files,
                isMultilineWithVariousIndent = isMultilineWithVariousIndent,
                fasterMethodForReplacing = fasterMethodForReplacing
            }, EncodingHelperIsBinary);
    }


    /// <summary>
    ///     A4 - whether use s.Contains. A4 - SHReplace.ReplaceAll2
    /// </summary>
    /// <param name="replaceFrom"></param>
    /// <param name="replaceTo"></param>
    /// <param name="files"></param>
    /// <param name="dontReplaceAll"></param>
    public static
#if ASYNC
        async Task
#else
void
#endif
        ReplaceInAllFiles(IList<string> replaceFrom, IList<string> replaceTo, ReplaceInAllFilesArgsBase p,
            Func<string, bool> EncodingHelperIsBinary)
    {
        #region ReplaceInAllFilesArgsBase - Zkopírovat i do ReplaceInAllFilesWorker. Viz comment níže

        // musím to rozdělit na jednotlivé proměnné abych viděl co se používá a co ne. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
        var fasterMethodForReplacing = p.fasterMethodForReplacing;
        var files = p.files;
        var inDownloadedFolders = p.inDownloadedFolders;
        var inFoldersToDelete = p.inFoldersToDelete;
        var inGitFiles = p.inGitFiles;
        var isMultilineWithVariousIndent = p.isMultilineWithVariousIndent;
        var writeEveryReadedFileAsStatus = p.writeEveryReadedFileAsStatus;
        var writeEveryWrittenFileAsStatus = p.writeEveryWrittenFileAsStatus;
        var dRemoveGitFiles = p.dRemoveGitFiles;

        #endregion

        if (!inGitFiles || !inFoldersToDelete || !inDownloadedFolders)
            dRemoveGitFiles(files, inGitFiles, inDownloadedFolders, inFoldersToDelete);

        foreach (var item in files)
        {
#if DEBUG
            if (item.EndsWith(dEndsWithReplaceInFile))
            {
            }
#endif

            if (!EncodingHelperIsBinary(item))
            {
                if (writeEveryReadedFileAsStatus)
                {
                    //SunamoTemplateLogger.Instance.LoadedFromStorage(item);
                }

                // File.ReadAllText is 20x faster than File.ReadAllText
                var content =
#if ASYNC
                    await
#endif
                        File.ReadAllTextAsync(item);
                var content2 = string.Empty;

                if (fasterMethodForReplacing == null)
                    for (var i = 0; i < replaceFrom.Count; i++)
                        content2 = content.Replace(replaceFrom[i], replaceTo[i]);
                //SHReplace.ReplaceAll3(replaceFrom, replaceTo, isMultilineWithVariousIndent, content);
                else
                    content2 = fasterMethodForReplacing.Invoke(new StringBuilder(content), replaceFrom, replaceTo)
                        .ToString();

                if (content != content2)
                {
                    //PpkOnDrive ppk = PpkOnDrive.WroteOnDrive;
                    //ppk.Add(DateTime.Now.ToString() + " " + item);

                    await File.WriteAllTextAsync(item, content2);

                    if (writeEveryReadedFileAsStatus)
                    {
                        //SunamoTemplateLogger.Instance.SavedToDrive(item);
                    }
                }
            }
            //ThisApp.Warning(sess.i18n(XlfKeys.ContentOf) + " " + item + " couldn't be replaced - contains control chars.");
        }
    }

    /// <summary>
    ///     Jen kvuli starým aplikacím, at furt nenahrazuji.
    /// </summary>
    /// <param name="v"></param>
    public static string GetFileInStartupPath(string v)
    {
        return AppPaths.GetFileInStartupPath(v);
    }

    public static
#if ASYNC
        async Task
#else
void
#endif
        RemoveDiacriticInFileContents(string folder, string mask)
    {
        var files = FSGetFiles.GetFiles(folder, mask, SearchOption.AllDirectories);
        foreach (var item in files)
        {
            var df2 =
#if ASYNC
                await
#endif
                    File.ReadAllTextAsync(item, Encoding.Default);
            if (true) //SH.ContainsDiacritic(df2))
            {
#if ASYNC
                await
#endif
                    File.WriteAllTextAsync(item, df2.RemoveDiacritics());
                df2 = SHReplace.ReplaceOnce(df2, "\u010F\u00BB\u017C", "");

#if ASYNC
                await
#endif
                    File.WriteAllTextAsync(item, df2);
            }
        }
    }

    //public static List<string> PathsOfStorageFiles<StorageFolder, StorageFile>(IList<StorageFile> files1, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    List<string> d = new List<string>(files1.Count());

    //    foreach (var item in files1)
    //    {
    //        d.Add(FS.StorageFilePath(item, ac));
    //    }

    //    return d;
    //}

    public static string RemoveFile(string fullPathCsproj)
    {
        // Most effecient way to handle csproj and dir
        var ext = Path.GetExtension(fullPathCsproj);
        if (ext != string.Empty) fullPathCsproj = Path.GetDirectoryName(fullPathCsproj);
        var result = WithoutEndSlash(fullPathCsproj);
        SH.FirstCharUpper(ref result);
        return result;
    }


    public static string MakeFromLastPartFile(string fullPath, string ext)
    {
        WithoutEndSlash(ref fullPath);
        return fullPath + ext;
    }

    /// <summary>
    ///     Remove all extensions, not only one
    /// </summary>
    /// <param name="item"></param>
    public static string GetFileNameWithoutExtensions(string item)
    {
        while (Path.HasExtension(item)) item = Path.GetFileNameWithoutExtension(item);
        return item;
    }

    public static void CopyAs0KbFilesSubfolders
        (string pathDownload, string pathVideos0Kb)
    {
        WithEndSlash(ref pathDownload);
        WithEndSlash(ref pathVideos0Kb);
        var folders = Directory.GetDirectories(pathDownload);
        foreach (var item in folders) CopyAs0KbFiles(item, item.Replace(pathDownload, pathVideos0Kb));
    }

    public static void CopyAs0KbFiles(string pathDownload, string pathVideos0Kb)
    {
        WithEndSlash(ref pathDownload);
        WithEndSlash(ref pathVideos0Kb);
        var files = GetFiles(pathDownload, true);
        foreach (var item in files)
        {
            var path = item.Replace(pathDownload, pathVideos0Kb);
            CreateUpfoldersPsysicallyUnlessThere(path);
            File.WriteAllText(path, string.Empty);
        }
    }

    public static string ShrinkLongPath(string actualFilePath)
    {
        // .NET 4.7.1
        // Originally - 265 chars, 254 also too long: E:\Documents\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\Facebook.cs
        // 4+265 - OK: @"\\?\D:\_NewlyRecovered\Visual Studio 2020\Projects\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\Facebook.cs"
        // 216 - OK: D:\Recovered data 03-23 12_11_44012345678901234567890123456\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\
        // for many API is different limits: https://stackoverflow.com/questions/265769/maximum-filename-length-in-ntfs-windows-xp-and-windows-vista
        // 237+11 - bad
        return @"\\?\" + actualFilePath;
    }

    public static string CreateNewFolderPathWithEndingNextTo(string folder, string ending)
    {
        var pathToFolder = Path.GetDirectoryName(folder.TrimEnd('\\')) + "\"";
        var folderWithCaretFiles = pathToFolder + Path.GetFileName(folder.TrimEnd('\\')) + ending;
        var result = folderWithCaretFiles;
        SH.FirstCharUpper(ref result);
        return result;
    }

    public static void CopyFilesOfExtensions(string folderFrom, string FolderTo, params string[] extensions)
    {
        folderFrom = WithEndSlash(folderFrom);
        FolderTo = WithEndSlash(FolderTo);
        var filesOfExtension = FSGetFiles.FilesOfExtensions(folderFrom, extensions);
        foreach (var item in filesOfExtension)
            foreach (var path in item.Value)
            {
                var newPath = path.Replace(folderFrom, FolderTo);
                CreateUpfoldersPsysicallyUnlessThere(newPath);
                File.Copy(path, newPath);
            }
    }

    /// <summary>
    ///     Kromě jmen také zbavuje diakritiky složky.
    /// </summary>
    /// <param name="folder"></param>
    public static void RemoveDiacriticInFileSystemEntryNames(string folder)
    {
        var folders =
            new List<string>(Directory.GetDirectories(folder, "*", SearchOption.AllDirectories));
        folders.Reverse();
        foreach (var item in folders)
        {
            var directory = Path.GetDirectoryName(item);
            var filename = Path.GetFileName(item);
            if (filename.HasDiacritics())
            {
                filename = filename.RemoveDiacritics();
                var newpath = Path.Combine(directory, filename);
                var realnewpath = newpath.TrimEnd('\\');
                var realnewpathcopy = realnewpath;
                var i = 0;
                while (Directory.Exists(realnewpath))
                {
                    realnewpath = realnewpathcopy + i;
                    i++;
                }

                Directory.Move(item, realnewpath);
            }
        }

        var files = FSGetFiles.GetFiles(folder, "*", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            var directory = Path.GetDirectoryName(item);
            var filename = Path.GetFileName(item);
            if (filename.HasDiacritics())
            {
                filename = filename.RemoveDiacritics();
                string newpath = null;
                try
                {
                    newpath = Path.Combine(directory, filename);
                }
                catch (Exception ex)
                {
                    ThrowEx.Custom(ex);
                    File.Delete(item);
                    continue;
                }

                var realNewPath = string.Copy(newpath);
                var vlozeno = 0;
                while (File.Exists(realNewPath))
                {
                    realNewPath = InsertBetweenFileNameAndExtension(newpath, vlozeno.ToString());
                    vlozeno++;
                }

                File.Move(item, realNewPath);
            }
        }
    }

    public static string GetUpFolderWhichContainsExtension(string path, string fileExt)
    {
        while (FSGetFiles.FilesOfExtension(path, fileExt).Count == 0)
        {
            if (path.Length < 4) return null;
            path = Path.GetDirectoryName(path);
        }

        return path;
    }

    public static void TrimContentInFilesOfFolder(string slozka, string searchPattern, SearchOption searchOption)
    {
        var files = FSGetFiles.GetFiles(slozka, searchPattern, searchOption);
        foreach (var item in files)
        {
            var fs = new FileStream(item, FileMode.Open);
            var sr = new StreamReader(fs, true);
            var content = sr.ReadToEnd();
            var enc = sr.CurrentEncoding;
            //sr.Close();
            sr.Dispose();
            sr = null;
            var contentTrim = content.Trim();
            File.WriteAllText(item, contentTrim, enc);
            //}
        }
    }

    /// <summary>
    ///     Náhrada za metodu ReplaceFileName se stejnými parametry
    /// </summary>
    /// <param name="oldPath"></param>
    /// <param name="what"></param>
    /// <param name="forWhat"></param>
    public static string ReplaceInFileName(string oldPath, string what, string forWhat)
    {
        string p2, fn;
        GetPathAndFileName(oldPath, out p2, out fn);
        var vr = p2 + "\"" + fn.Replace(what, forWhat);
        SH.FirstCharUpper(ref vr);
        return vr;
    }

    public static long GetSizeIn(long value, ComputerSizeUnits b, ComputerSizeUnits to)
    {
        if (b == to) return value;
        var toLarger = (byte)b < (byte)to;
        if (toLarger)
        {
            value = ConvertToSmallerComputerUnitSize(value, b, ComputerSizeUnits.B);
            if (to == ComputerSizeUnits.Auto)
                throw new Exception(
                    "Byl specifikov\u00E1n v\u00FDstupn\u00ED ComputerSizeUnit, nem\u016F\u017Eu toto nastaven\u00ED zm\u011Bnit");
            if (to == ComputerSizeUnits.KB && b != ComputerSizeUnits.KB)
                value /= 1024;
            else if (to == ComputerSizeUnits.MB && b != ComputerSizeUnits.MB)
                value /= 1024 * 1024;
            else if (to == ComputerSizeUnits.GB && b != ComputerSizeUnits.GB)
                value /= 1024 * 1024 * 1024;
            else if (to == ComputerSizeUnits.TB && b != ComputerSizeUnits.TB) value /= 1024L * 1024L * 1024L * 1024L;
        }
        else
        {
            value = ConvertToSmallerComputerUnitSize(value, b, to);
        }

        return value;
    }

    /// <summary>
    ///     Zjistí všechny složky rekurzivně z A1 a prvně maže samozřejmě ty, které mají více tokenů
    /// </summary>
    /// <param name="v"></param>
    public static void DeleteAllEmptyDirectories(string v, bool deleteAlsoA1, params string[] doNotDeleteWhichContains)
    {
        var dirs = DirectoriesWithToken(v, AscDesc.Desc);
        foreach (var item in dirs)
            if (IsDirectoryEmpty(item.t, true, true))
            {
                if (doNotDeleteWhichContains.Length > 0)
                {
                    if (!doNotDeleteWhichContains.Any(d =>
                            item.t.Contains(d))) //CANewSH.ContainsAnyFromArray(item.t, doNotDeleteWhichContains))
                        TryDeleteDirectory(item.t);
                }
                else
                {
                    TryDeleteDirectory(item.t);
                }
            }

        if (IsDirectoryEmpty(v, false, true) && !doNotDeleteWhichContains.Any()) TryDeleteDirectory(v);
    }

    //private static List<TWithInt<string>> DirectoriesWithToken(string v, AscDesc desc)
    //{
    //    ThrowEx.NotImplementedMethod();
    //}

    public static int CompareTWithInt<T>(TWithInt<T> t, TWithInt<T> u)
    {
        if (t.count > u.count)
            return 1;
        if (t.count < u.count) return -1;
        return 0;
    }

    public static List<TWithInt<string>> DirectoriesWithToken(string v, AscDesc sb)
    {
        var dirs = Directory.GetDirectories(v, "*", SearchOption.AllDirectories);
        var vr = new List<TWithInt<string>>();
        foreach (var item in dirs)
            vr.Add(new TWithInt<string>
            {
                t = item,
                count = SH.OccurencesOfStringIn(item, "\"")
            });

        vr.Sort(CompareTWithInt);

        if (sb == AscDesc.Desc) vr.Reverse();
        //vr.Sort(new SunamoComparerICompare.TWithIntComparer.Asc<string>(new SunamoComparer.TWithIntSunamoComparer<string>()));
        //else if (sb == AscDesc.Desc)
        //{
        //    vr.Sort(new SunamoComparerICompare.TWithIntComparer.Desc<string>(new SunamoComparer.TWithIntSunamoComparer<string>()));
        //}
        return vr;
    }


    /// <summary>
    ///     A1 i A2 musí končit backslashem
    ///     Může vyhodit výjimku takže je nutné to odchytávat ve volající metodě
    ///     If destination folder exists, source folder without files keep
    ///     Return message if success, or null
    ///     A5 false
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    /// <param name="directoryMoveCollisionOption"></param>
    public static string MoveDirectoryNoRecursive(string from, string to, DirectoryMoveCollisionOption directoryMoveCollisionOption,
        FileMoveCollisionOption fileMoveCollisionOption)
    {
        string vr = null;
        if (Directory.Exists(to))
        {
            if (directoryMoveCollisionOption == DirectoryMoveCollisionOption.AddSerie)
            {
                var serie = 1;
                while (true)
                {
                    var newFn = to + " (" + serie + ")";
                    if (!Directory.Exists(newFn))
                    {
                        vr = sess.i18n(XlfKeys.FolderHasBeenRenamedTo) + " " + Path.GetFileName(newFn);
                        to = newFn;
                        break;
                    }

                    serie++;
                }
            }
            else if (directoryMoveCollisionOption == DirectoryMoveCollisionOption.DiscardFrom)
            {
                Directory.Delete(from, true);
                return vr;
            }
            else if (directoryMoveCollisionOption == DirectoryMoveCollisionOption.Overwrite)
            {
            }
            else if (directoryMoveCollisionOption == DirectoryMoveCollisionOption.ThrowEx)
            {
                ThrowEx.Custom($"Directory {to} already exists");
            }
        }

        var files = FSGetFiles.GetFiles(from, "*", SearchOption.AllDirectories);
        CreateFoldersPsysicallyUnlessThere(to);
        foreach (var item2 in files)
        {
            var fileTo = to + item2.Substring(from.Length);
            MoveFile(item2, fileTo, fileMoveCollisionOption);
        }

        try
        {
            Directory.Move(from, to);
        }
        catch (Exception ex)
        {
            //ThrowEx.CannotMoveFolder(item, nova, ex);
        }

        DeleteAllEmptyDirectories(from, true);
        return vr;
    }

    private static bool IsDirectoryEmpty(string item, bool folders, bool files)
    {
        var fse = 0;
        if (folders) fse += Directory.GetDirectories(item, "*", SearchOption.TopDirectoryOnly).Length;
        if (files) fse += FSGetFiles.GetFiles(item, "*", SearchOption.TopDirectoryOnly).Count;
        return fse == 0;
    }

    /// <summary>
    ///     Vyhazuje výjimky, takže musíš volat v try-catch bloku
    ///     A2 is root of target folder
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    public static void MoveAllRecursivelyAndThenDirectory(string p, string to, FileMoveCollisionOption co)
    {
        MoveAllFilesRecursively(p, to, co);
        var dirs = Directory.GetDirectories(p, "*", SearchOption.AllDirectories);
        for (var i = dirs.Length - 1; i >= 0; i--) TryDeleteDirectory(dirs[i]);
        TryDeleteDirectory(p);
    }

    [Obsolete("Use MoveDirectoryNoRecursive instead")]
    public static void MoveAllFilesRecursively(string p, string to, FileMoveCollisionOption co, string contains = null)
    {
        CopyMoveAllFilesRecursively(p, to, co, true, contains, SearchOption.AllDirectories);
    }

    /// <summary>
    ///     Unit tests = OK
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteFilesWithSameContentBytes(List<string> files)
    {
        DeleteFilesWithSameContentWorking<List<byte>, byte>(files, TF.ReadAllBytesSync);
    }

    /// <summary>
    ///     Unit tests = OK
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteDuplicatedImages(List<string> files)
    {
        throw new Exception(sess.i18n(XlfKeys.OnlyForTestFilesForAnotherApps) + ". ");
    }

    /// <summary>
    ///     zde to zatím nechám jako sync
    ///     ¨Func má jen Invoke, nemohl bych užívat výhody asyncu
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="ColType"></typeparam>
    /// <param name="files"></param>
    /// <param name="readFunc"></param>
    public static void DeleteFilesWithSameContentWorking<T, ColType>(List<string> files, Func<string, T> readFunc)
    {
        var dictionary = new Dictionary<string, T>(files.Count);
        foreach (var item in files) dictionary.Add(item, readFunc.Invoke(item));
        var sameContent = DictionaryHelper.GroupByValues<string, T, ColType>(dictionary);
        foreach (var item in sameContent)
            if (item.Value.Count > 1)
            {
                item.Value.RemoveAt(0);
                item.Value.ForEach(d => File.Delete(d));
            }
    }

    /// <summary>
    ///     Working fine, verified by Unit tests
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteFilesWithSameContent(List<string> files)
    {
        DeleteFilesWithSameContentWorking<string, object>(files, File.ReadAllText);
    }

    /// <summary>
    ///     Normally: 11,12,1,2,...
    ///     This: 1,2,...,11,12
    ///     non direct edit
    ///     working with full paths or just filenames
    /// </summary>
    /// <param name="l"></param>
    public static List<string> OrderByNaturalNumberSerie(List<string> l)
    {
        var filenames = new List<Tuple<string, int, string>>();
        var dontHaveNumbersOnBeginning = new List<string>();
        string path = null;
        for (var i = l.Count - 1; i >= 0; i--)
        {
            var backup = l[i];
            var p = SHSplit.SplitToPartsFromEnd(l[i], 2, '\\');
            if (p.Count == 1)
            {
                path = string.Empty;
            }
            else
            {
                path = p[0];
                l[i] = p[1];
            }

            var fn = l[i];
            //var (sh, fnNew) = NH.NumberIntUntilWontReachOtherChar(fn);
            var sh = int.Parse(Regex.Match(fn, @"\d+").Value);
            var fnNew = fn.Replace(sh.ToString(), string.Empty);

            fn = fnNew;
            if (sh == int.MaxValue)
                dontHaveNumbersOnBeginning.Add(backup);
            else
                filenames.Add(new Tuple<string, int, string>(path, sh, fn));
        }

        var sorted = filenames.OrderBy(d => d.Item2);
        var result = new List<string>(l.Count);
        foreach (var item in sorted) result.Add(Path.Combine(item.Item1, item.Item2 + item.Item3));
        result.AddRange(dontHaveNumbersOnBeginning);
        return result;
    }

    public static Dictionary<string, List<string>> SortPathsByFileName(List<string> allCsFilesInFolder,
        bool onlyOneExtension)
    {
        var vr = new Dictionary<string, List<string>>();
        foreach (var item in allCsFilesInFolder)
        {
            string fn = null;
            if (onlyOneExtension)
                fn = Path.GetFileNameWithoutExtension(item);
            else
                fn = Path.GetFileName(item);
            DictionaryHelper.AddOrCreate(vr, fn, item);
        }

        return vr;
    }

    public static void DeleteAllRecursively(string p, bool rootDirectoryToo = false)
    {
        if (Directory.Exists(p))
        {
            var files = FSGetFiles.GetFiles(p, "*", SearchOption.AllDirectories);
            foreach (var item in files) TryDeleteFile(item);
            var dirs = Directory.GetDirectories(p, "*", SearchOption.AllDirectories);
            for (var i = dirs.Length - 1; i >= 0; i--) TryDeleteDirectory(dirs[i]);
            if (rootDirectoryToo) TryDeleteDirectory(p);
            // Commented due to NI
            //FS.DeleteFoldersWhichNotContains(BasePathsHelper.vs + @"", "bin", new List<string>( "node_modules"));
        }
    }

    public static void DeleteFoldersWhichNotContains(string v, string folder, IList<string> v2)
    {
        //var f = Directory.GetDirectories(v, folder, SearchOption.AllDirectories);
        //for (int i = f.Count - 1; i >= 0; i--)
        //{
        //    if (CA.ReturnWhichContainsIndexes( f[i], v2).Count != 0)
        //    {
        //        f.RemoveAt(i);
        //    }
        //}
        //ClipboardHelper.SetLines(f);
        //foreach (var item in f)
        //{
        //    //FS.DeleteF
        //}
    }

    /// <summary>
    ///     Vyhazuje výjimky, takže musíš volat v try-catch bloku
    /// </summary>
    /// <param name="p"></param>
    public static void DeleteAllRecursivelyAndThenDirectory(string p)
    {
        DeleteAllRecursively(p, true);
    }

    public static List<string> OnlyExtensions(List<string> cesta)
    {
        var vr = new List<string>(cesta.Count);
        //CA.InitFillWith(vr, cesta.Count);
        for (var i = 0; i < vr.Count; i++) vr[i] = Path.GetExtension(cesta[i]);
        return vr;
    }

    /// <summary>
    ///     Both filenames and extension convert to lowercase
    ///     Filename is without extension
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    /// <param name="searchOption"></param>
    public static Dictionary<string, List<string>> GetDictionaryByExtension(string folder, string mask,
        SearchOption searchOption)
    {
        var extDict = new Dictionary<string, List<string>>();
        foreach (var item in FSGetFiles.GetFiles(folder, mask, searchOption))
        {
            var ext = Path.GetExtension(item);
            var fn = Path.GetFileNameWithoutExtension(item).ToLower();

            if (fn == string.Empty)
            {
                fn = ext;
                ext = "";
            }

            DictionaryHelper.AddOrCreate(extDict, ext, fn);
        }

        return extDict;
    }

    public static List<string> OnlyExtensionsToLower(List<string> cesta, GetExtensionArgs a = null)
    {
        if (a == null) a = new GetExtensionArgs();

        a.returnOriginalCase = false;

        var vr = new List<string>(cesta.Count);
        //CA.InitFillWith(vr, cesta.Count);
        for (var i = 0; i < vr.Count; i++) vr[i] = Path.GetExtension(cesta[i]).ToLower();
        return vr;
    }

    public static List<string> OnlyExtensionsToLowerWithPath(List<string> cesta)
    {
        var vr = new List<string>(cesta.Count);
        //CA.InitFillWith(vr, cesta.Count);
        for (var i = 0; i < vr.Count; i++) vr[i] = OnlyExtensionToLowerWithPath(cesta[i]);
        return vr;
    }

    public static string OnlyExtensionToLowerWithPath(string d)
    {
        string path, fn, ext;
        GetPathAndFileName(d, out path, out fn, out ext);
        var result = path + fn + ext.ToLower();
        return result;
    }


    public static List<string> AllExtensionsInFolders(SearchOption so, params string[] folders)
    {
        ThrowEx.NoPassedFolders(folders);

        List<string> filesFull = FSGetFiles.AllFilesInFolders(folders.ToList(), new List<string>(["*"]), so);

        return AllExtensionsInFolders(filesFull);
    }

    /// <summary>
    ///     files as .bowerrc return whole
    /// </summary>
    /// <param name="so"></param>
    /// <param name="folders"></param>
    public static List<string> AllExtensionsInFolders(List<string> filesFull, GetExtensionArgs gea = null)
    {
        var vr = new List<string>();


#if DEBUG

        //var dx = filesFull.IndexOf(".babelrc");
#endif
        var files = new List<string>(OnlyExtensionsToLower(filesFull, gea));

#if DEBUG
        //var dxs = CA.IndexesWithValue(files, "");

        //List<string> c = CA.GetIndexes(filesFull, dxs);

        //ClipboardHelper.SetLines(c);

        //var dx2 = files.IndexOf(".babelrc");
#endif
        foreach (var item in files)
            if (!vr.Contains(item))
                vr.Add(item);
        return vr;
    }

    public static string ExpandEnvironmentVariables(EnvironmentVariables environmentVariable)
    {
        return Environment.ExpandEnvironmentVariables(SH.WrapWith(environmentVariable.ToString(), "%"));
    }

    /// <summary>
    ///     Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    /// </summary>
    /// <param name="s"></param>
    public static string GetFileNameWithoutExtensionLower(string s)
    {
        return GetFileNameWithoutExtension(s).ToLower();
    }

    public static string AddUpfoldersToRelativePath(int i2, string file, char delimiter)
    {
        var jumpUp = ".." + delimiter;
        var sb = new StringBuilder();
        for (var i = 0; i < i2; i++) sb.Append(jumpUp);
        sb.Append(file);
        return sb.ToString();
        //return SHJoin.JoinTimes(i, jumpUp) + file;
    }

    /// <summary>
    ///     convert to lowercase and remove first dot - to už asi neplatí. Use NormalizeExtension2 for that
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension(string item)
    {
        return "." + item.TrimStart('.');
    }


    public static string GetNormalizedExtension(string filename)
    {
        return NormalizeExtension(filename);
    }

    public static long ModifiedinUnix(string dsi)
    {
        return (long)File.GetLastWriteTimeUtc(dsi).Subtract(DTConstants.UnixFsStart).TotalSeconds;
    }

    public static void ReplaceDiacriticRecursive(string folder, bool dirs, bool files, DirectoryMoveCollisionOption fo,
        FileMoveCollisionOption co)
    {
        if (dirs)
        {
            var dires = DirectoriesWithToken(folder, AscDesc.Desc);
            foreach (var item in dires)
            {
                var dirPath = WithoutEndSlash(item.t);
                var dirName = Path.GetFileName(dirPath);
                if (dirName.HasDiacritics())
                {
                    var dirNameWithoutDiac = dirName.RemoveDiacritics(); //SH.TextWithoutDiacritic(dirName);
                    RenameDirectory(item.t, dirNameWithoutDiac, fo, co);
                }
            }
        }

        if (files)
        {
            var files2 = FSGetFiles.GetFiles(folder, "*", SearchOption.AllDirectories);
            foreach (var item in files2)
            {
                var filePath = item;
                var fileName = Path.GetFileName(filePath);
                if (fileName.HasDiacritics())
                {
                    var dirNameWithoutDiac = fileName.RemoveDiacritics();
                    RenameFile(item, dirNameWithoutDiac, co);
                }
            }
        }
    }

    /// <summary>
    ///     A1,2 = with ext
    ///     Physically rename file, this method is different from ChangeFilename in FileMoveCollisionOption A3 which can
    ///     control advanced collision solution
    /// </summary>
    /// <param name="oldFn"></param>
    /// <param name="newFnWithoutPath"></param>
    /// <param name="co"></param>
    public static void RenameFile(string oldFn, string newFnWithoutPath, FileMoveCollisionOption co)
    {
        var to = ChangeFilename(oldFn, newFnWithoutPath, false);
        MoveFile(oldFn, to, co);
    }

    /// <summary>
    ///     Může výhodit výjimku, proto je nutné používat v try-catch bloku
    ///     Vrátí řetězec se zprávou kterou vypsat nebo null
    /// </summary>
    /// <param name="path"></param>
    /// <param name="newname"></param>
    public static string RenameDirectory(string path, string newname, DirectoryMoveCollisionOption co,
        FileMoveCollisionOption fo)
    {
        string vr = null;
        path = WithoutEndSlash(path);
        var cesta = Path.GetDirectoryName(path);
        var nova = Path.Combine(cesta, newname);
        vr = MoveDirectoryNoRecursive(path, nova, co, fo);
        return vr;
    }

    /// <summary>
    ///     convert to lowercase and remove first dot
    /// </summary>
    /// <param name="extension"></param>
    public static void NormalizeExtensions(List<string> extension)
    {
        for (var i = 0; i < extension.Count; i++) extension[i] = NormalizeExtension(extension[i]);
    }

    /// <summary>
    ///     A1 může obsahovat celou cestu, vrátí jen název sobuoru bez připony a příponu
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    public static void GetFileNameWithoutExtensionAndExtension(string fn, out string file, out string ext)
    {
        file = Path.GetFileNameWithoutExtension(fn);
        ext = Path.GetExtension(file);
    }

    public static void SaveStream(string path, Stream s)
    {
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            CopyStream(s, fs);
            fs.Flush();
        }
    }

    public static List<string> OnlyNamesWithoutExtensionCopy(List<string> p2)
    {
        var p = new List<string>(p2.Count);
        //CA.InitFillWith(p, p2.Count);
        for (var i = 0; i < p2.Count; i++) p[i] = Path.GetFileNameWithoutExtension(p2[i]);
        return p;
    }

    public static bool DirectoryExistsAndIsNotEmpty(string v)
    {
        if (Directory.Exists(v) && Directory.GetFiles(v, "*", SearchOption.AllDirectories).Length != 0) return true;
        return false;
    }

    public static List<string> OnlyNamesWithoutExtension(string appendToStart, List<string> fullPaths)
    {
        var ds = new List<string>(fullPaths.Count);
        //CA.InitFillWith(ds, fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++)
            ds[i] = appendToStart + Path.GetFileNameWithoutExtension(fullPaths[i]);
        return ds;
    }

    public static string Postfix(string aPath, string s)
    {
        var result = aPath.TrimEnd('\\') + s;
        WithEndSlash(ref result);
        return result;
    }


    public static
#if ASYNC
        async Task<string>
#else
string
#endif
        ReadAllText(string path)
    {
        return
#if ASYNC
            await
#endif
                File.ReadAllTextAsync(path);
    }


    public static string GetFileNameWithoutExtension(string s)
    {
        return PathMs.GetFileNameWithoutExtension(s.TrimEnd(PathMs.DirectorySeparatorChar));
    }

    /// <summary>
    ///     Problémová metoda
    ///     Píše že nemůže najít SunamoValues, přitom v nugetech je
    /// </summary>
    /// <typeparam name="StorageFile"></typeparam>
    /// <param name="s"></param>
    /// <returns></returns>
    public static StorageFile GetFileNameWithoutExtensionNoAc<StorageFile>(StorageFile s)
    {
        var ss = s.ToString();
        var vr = Path.GetFileNameWithoutExtension(ss.TrimEnd('\\'));
        var ext = Path.GetExtension(ss).TrimStart('.');

        LetterAndDigitCharService letterAndDigitChar = new LetterAndDigitCharService();

        if (!ext.All(d =>
                letterAndDigitChar.allCharsWithoutSpecial.Contains(d)) /*SH.ContainsOnly(ext, AllChars.allCharsWithoutSpecial)*/)
            if (ext != string.Empty)
                return (dynamic)vr + "." + ext;

        return (dynamic)vr;
    }

    ///// <summary>
    /////     Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    /////     if have more extension, remove just one
    ///// </summary>
    ///// <param name="s"></param>
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile s,
    //AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        return GetFileNameWithoutExtension<StorageFolder, StorageFile>(s, null)
    //    }

    //    ThrowNotImplementedUwp();
    //    return s;
    //}


    public static void ThrowNotImplementedUwp()
    {
        throw new Exception("Not implemented in UWP");
    }


    public static bool IsFileOlderThanXHours(string path, int hours, bool mustFileExists = false)
    {
        var exf = File.Exists(path);

        if (mustFileExists)
        {
            if (!exf) ThrowEx.FileDoesntExists(path);
        }
        else
        {
            if (!exf) return true;
        }

        var lm = LastModified(path);
        if (lm > DateTime.Now.AddHours(hours * -1)) return false;
        return true;
    }

    public static List<string> GetFileNamesWoExtension(List<string> jpgcka)
    {
        var dd = new List<string>(jpgcka.Count);
        for (var i = 0; i < jpgcka.Count; i++) dd.Add(Path.GetFileNameWithoutExtension(jpgcka[i]));

        return dd;
    }


    /// <summary>
    ///     path + file
    /// </summary>
    public static string GetTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
    }


    /// <summary>
    ///     Copy file A1 into A2
    /// </summary>
    /// <param name="v"></param>
    /// <param name="nad"></param>
    public static void CopyTo(string v, string nad, FileMoveCollisionOption o)
    {
        var fileTo = Path.Combine(nad, Path.GetFileName(v));
        CopyFile(v, fileTo, o);
    }


    //public static StorageFolder GetDirectoryNameFolder<StorageFolder, StorageFile>(StorageFolder rp2, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.Path.GetDirectoryNameFolder.Invoke(rp2);
    //    }
    //    //throw new Exception("GetDirectoryName");
    //    var rp = rp2.ToString();
    //    return (dynamic)GetDirectoryName(rp);
    //}


    //public static void CreateFoldersPsysicallyUnlessThere<StorageFolder, StorageFile>(StorageFile nad, AbstractCatalog<StorageFolder, StorageFile> ac)
    //    {
    //        if (ac == null)
    //        {
    //            CreateFoldersPsysicallyUnlessThere(nad.ToString());
    //}
    //        else
    //        {
    //            ThrowNotImplementedUwp();
    //        }
    //    }

    /// <summary>
    ///     change all first (drive) letter to uppercase
    /// </summary>
    /// <param name="p"></param>
    /// <param name="folderWithProjectsFolders"></param>
    /// <param name="folderWithTemporaryMovedContentWithoutBackslash"></param>
    public static string ReplaceDirectoryThrowExceptionIfFromDoesntExists(string p, string folderWithProjectsFolders,
        string folderWithTemporaryMovedContentWithoutBackslash)
    {
        p = SH.FirstCharUpper(p);
        folderWithProjectsFolders = SH.FirstCharUpper(folderWithProjectsFolders);
        folderWithTemporaryMovedContentWithoutBackslash =
            SH.FirstCharUpper(folderWithTemporaryMovedContentWithoutBackslash);

        if (!ThrowEx.NotContains(p, folderWithProjectsFolders))
            // Here can never accomplish when exc was throwed
            return p;

        // Here can never accomplish when exc was throwed
        return p.Replace(folderWithProjectsFolders, folderWithTemporaryMovedContentWithoutBackslash);
    }

    /// <summary>
    ///     Direct edit
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static List<string> OnlyNamesWithoutExtension(List<string> p)
    {
        for (var i = 0; i < p.Count; i++) p[i] = Path.GetFileNameWithoutExtension(p[i]);
        return p;
    }


    /// <summary>
    ///     Vrátí cestu a název souboru s ext a ext
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    public static void GetPathAndFileName(string fn, out string path, out string file, out string ext)
    {
        path = WithEndSlash(Path.GetDirectoryName(fn));
        file = Path.GetFileNameWithoutExtension(fn);
        ext = Path.GetExtension(fn);
    }


    /// <summary>
    ///     Not working - see unit tests
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static string GetAbsolutePath2(string relativePath, string dir)
    {
        var ap = GetAbsolutePath(dir, relativePath);
        return Path.GetFullPath(ap);
    }

    /// <summary>
    ///     Working - see unit tests
    ///     if A1 not ending with \, GetDirectoryName
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetAbsolutePath(string dir, string relativePath)
    {
        FileToDirectory(ref dir);

        var ds = "./";
        var dds = "../";

        var dds2 = 0;
        while (true)
            if (relativePath.StartsWith(ds))
            {
                relativePath = relativePath.Substring(ds.Length);
            }
            else if (relativePath.StartsWith(dds))
            {
                dds2++;
                relativePath = relativePath.Substring(dds.Length);
            }
            else
            {
                break;
            }

        var tokens = GetTokens(relativePath);
        tokens = tokens.Skip(dds2).ToList();
        tokens.Insert(0, dir);

        var vr = Combine(tokens.ToArray());
        vr = GetFullPath(vr);
        return vr;
    }


    public static List<string> GetTokens(string relativePath)
    {
        var deli = "";
        if (relativePath.Contains("\""))
            deli = "\"";
        else if (relativePath.Contains("/")) deli = "/";
        else
        {
            ThrowEx.NotImplementedCase(relativePath);
        }

        return SHSplit.SplitMore(relativePath, deli);
    }


    public static void CopyStream(Stream input, Stream output)
    {
        var buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0) output.Write(buffer, 0, len);
    }


    /// <summary>
    ///     Cant return with end slash becuase is working also with files
    /// </summary>
    /// <param name="s"></param>
    public static string CombineWithoutFirstCharUpper(params string[] s)
    {
        return CombineWorker(false, true, s);
    }


    public static void SaveMemoryStream(MemoryStream mss, string path)
    {
        //SaveMemoryStream<string, string>(mss, path, null);
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            var matriz = mss.ToArray();
            fs.Write(matriz, 0, matriz.Length);
        }
    }

    //public static void SaveMemoryStream<StorageFolder, StorageFile>(System.IO.MemoryStream mss, StorageFile path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{

    //    if (!FS.ExistsFileAc(path, ac))
    //    {
    //        if (ac == null)
    //        {
    //            using (System.IO.FileStream fs = new System.IO.FileStream(path.ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write))
    //            {
    //                byte[] matriz = mss.ToArray();
    //                fs.Write(matriz, 0, matriz.Length);
    //            }
    //        }
    //        else
    //        {
    //            throw new Exception("SaveMemoryStream");
    //        }
    //    }
    //}


    //public static StorageFolder CiStorageFolder<StorageFolder, StorageFile>(string path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ps = path.ToString();
    //        ps = FS.WithEndSlash(ps);
    //        return (dynamic)ps;
    //    }
    //    return ac.fs.ciStorageFolder.Invoke(path);
    //}


    public static string DeleteWrongCharsInDirectoryName(string p)
    {
        var sb = new StringBuilder();
        foreach (var item in p)
            if (!s_invalidPathChars.Contains(item))
                sb.Append(item);
        var result = sb.ToString();
        SH.FirstCharUpper(ref result);
        return result;
    }

    public static string DeleteWrongCharsInFileName(string p, bool isPath)
    {
        List<char> invalidFileNameChars2 = null;

        if (isPath)
            invalidFileNameChars2 = s_invalidFileNameCharsWithoutDelimiterOfFolders;
        else
            invalidFileNameChars2 = s_invalidFileNameChars;

        var sb = new StringBuilder();
        foreach (var item in p)
            if (!invalidFileNameChars2.Contains(item))
                sb.Append(item);

        var result = sb.ToString();
        SH.FirstCharUpper(ref result);
        return result;
    }

    public static bool ContainsInvalidPathCharForPartOfMapPath(string p)
    {
        foreach (var item in s_invalidCharsForMapPath)
            if (p.IndexOf(item) != -1)
                return true;

        return false;
    }

    /// <summary>
    ///     Odstraňuje samozřejmě ve výjimce
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }

    /// <summary>
    ///     No direct edit
    /// </summary>
    /// <param name="files2"></param>
    /// <returns></returns>
    public static List<string> OnlyNamesNoDirectEdit(string[] files2)
    {
        var tl = files2.ToList();
        return OnlyNamesNoDirectEdit(tl);
    }

    /// <summary>
    ///     No direct edit
    ///     Returns with extension
    ///     POZOR: Na rozdíl od stejné metody v sunamo tato metoda vrací úplně nové pole a nemodifikuje A1
    /// </summary>
    /// <param name="files"></param>
    public static List<string> OnlyNamesNoDirectEdit(List<string> files2)
    {
        var files = new List<string>(files2.Count);
        for (var i = 0; i < files2.Count; i++) files.Add(Path.GetFileName(files2[i]));
        return files;
    }

    /// <summary>
    ///     No direct edit
    /// </summary>
    /// <param name="appendToStart"></param>
    /// <param name="fullPaths"></param>
    /// <returns></returns>
    public static List<string> OnlyNamesNoDirectEdit(string appendToStart, List<string> fullPaths)
    {
        var ds = new List<string>(fullPaths.Count);
        //CA.InitFillWith(ds, fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++) ds[i] = appendToStart + Path.GetFileName(fullPaths[i]);
        return ds;
    }


    /// <summary>
    ///     A2 is path of target file
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fileTo"></param>
    /// <param name="co"></param>
    //public static void CopyFile<StorageFolder, StorageFile>(string item, string fileTo2, FileMoveCollisionOption co, AbstractCatalog<StorageFolder, StorageFile> ac = null)
    //{
    //    if (ac == null)
    //    {
    //        CopyFile(item, fileTo2, co);
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //    }
    //}

    ///// <summary>
    ///// A1,2 isnt  working like ref
    ///// </summary>
    ///// <typeparam name="StorageFolder"></typeparam>
    ///// <typeparam name="StorageFile"></typeparam>
    ///// <param name="item"></param>
    ///// <param name="fileTo"></param>
    ///// <param name="co"></param>
    ///// <param name="ac"></param>
    //public static bool CopyMoveFilePrepare<StorageFolder, StorageFile>(ref StorageFile item, ref StorageFile fileTo, FileMoveCollisionOption co, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var item2 = item.ToString();
    //        var fileTo2 = fileTo.ToString();
    //        return CopyMoveFilePrepare(ref item2, ref fileTo2, co);
    //    }

    //    ThrowNotImplementedUwp();
    //    MakeUncLongPath(ref item, ac);
    //    MakeUncLongPath<StorageFolder, StorageFile>(ref fileTo, ac);
    //    //FS.CreateUpfoldersPsysicallyUnlessThereAc<StorageFolder, StorageFile>(fileTo, ac);
    //    FS.CreateUpfoldersPsysicallyUnlessThere()
    //    if (FS.ExistsFileAc<StorageFolder, StorageFile>(fileTo, ac))
    //    {
    //    }
    //    return false;
    //}

    //public static bool CopyMoveFilePrepare<StorageFolder, StorageFile>(ref string item, ref StorageFile fileTo2, FileMoveCollisionOption co, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var fileTo = fileTo2.ToString();
    //        return CopyMoveFilePrepare(ref item, ref fileTo, co);
    //    }

    //    ThrowNotImplementedUwp();
    //    return false;
    //}
    public static string ChangeExtension(string item, string newExt, bool physically)
    {
        //if (UH.HasHttpProtocol(item))
        //{
        //    return UH.ChangeExtension(item, Path.GetExtension(item, new GetExtensionArgs { returnOriginalCase = true }), newExt);
        //}

        var cesta = Path.GetDirectoryName(item);
        var fnwoe = Path.GetFileNameWithoutExtension(item);
        var nova = Path.Combine(cesta, fnwoe + newExt);

        if (physically)
            try
            {
                if (File.Exists(nova)) File.Delete(nova);
                File.Move(item, nova);
            }
            catch
            {
            }

        FirstCharUpper(ref nova);
        return nova;
    }


    public static string CreateDirectory(string v, DirectoryCreateCollisionOption whenExists, SerieStyleFS serieStyle,
        bool reallyCreate)
    {
        if (Directory.Exists(v))
        {
            bool hasSerie;
            var nameWithoutSerie = GetNameWithoutSeries(v, false, out hasSerie, serieStyle);
            if (hasSerie)
            {
            }

            if (whenExists == DirectoryCreateCollisionOption.AddSerie)
            {
                var serie = 1;
                while (true)
                {
                    var newFn = nameWithoutSerie + " (" + serie + ")";
                    if (!Directory.Exists(newFn))
                    {
                        v = newFn;
                        break;
                    }

                    serie++;
                }
            }
            else if (whenExists == DirectoryCreateCollisionOption.Delete)
            {
            }
            else if (whenExists == DirectoryCreateCollisionOption.Overwrite)
            {
            }
            else
            {
                ThrowEx.NotImplementedCase(whenExists);
            }
        }

        if (reallyCreate) Directory.CreateDirectory(v);

        return v;
    }


    //public static List<string> GetFilesEveryFolder(string folder, string mask, SearchOption searchOption, bool _trimA1 = false)
    //{


    //    var d = Task.Run<List<string>>(async () => await GetFilesEveryFolderAsync(folder, mask, searchOption, new GetFilesEveryFolderArgs {_trimA1 =  _trimA1 })).Result;
    //    return d;
    //}


    public static byte[] StreamToArrayBytes(Stream stream)
    {
        if (stream == null) return new byte[0];

        long originalPosition = 0;

        if (stream.CanSeek)
        {
            originalPosition = stream.Position;
            stream.Position = 0;
        }

        try
        {
            var readBuffer = new byte[4096];

            var totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    var nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        var temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            var buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }

            return buffer;
        }
        finally
        {
            if (stream.CanSeek) stream.Position = originalPosition;
        }
    }


    public static string AddExtensionIfDontHave(string file, string ext)
    {
        // For *.* and git paths {dir}/*
        if (file[file.Length - 1] == '*') return file;
        if (Path.GetExtension(file) == string.Empty) return file + ext;

        return file;
    }

    /// <summary>
    ///     Vratí bez cesty, pouze název souboru
    ///     Earlier name InsertBetweenFileNameAndExtension2
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="whatInsert"></param>
    public static string InsertBetweenFileNameAndExtensionRemovePath(string orig, string whatInsert)
    {
        var fn = Path.GetFileNameWithoutExtension(orig);
        var e = Path.GetExtension(orig);
        return Path.Combine(fn + whatInsert + e);
    }

    /// <summary>
    ///     In key are just filename, in value full path to all files
    /// </summary>
    /// <param name="linesFiles"></param>
    /// <param name="searchOnlyWithExtension"></param>
    public static Dictionary<string, List<string>> GetDictionaryByFileNameWithExtension(List<string> files)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var item in files)
        {
            var filename = Path.GetFileName(item);
            DictionaryHelper.AddOrCreateIfDontExists(result, filename, item);
        }

        return result;
    }


    public static string ChangeFilename(string item, string newFileNameWithoutPath, bool physically)
    {
        var cesta = Path.GetDirectoryName(item);
        var nova = Path.Combine(cesta, newFileNameWithoutPath);

        if (physically)
            try
            {
                if (File.Exists(nova)) File.Delete(nova);
                File.Move(item, nova);
            }
            catch
            {
            }

        return nova;
    }


    /// <summary>
    /// Zmeni nazev souboru na A2
    /// Pro A3 je výchozí z minulosti true - jakoby s false se chovala metoda ReplaceFileName
    /// Pokud nechci nazev souboru uplne menit, ale pouze v nem neco nahradit, pouziva se metoda ReplaceInFileName
    /// </summary>
    /// <param name="item"></param>
    /// <param name="newFileName"></param>
    /// <param name="onDrive"></param>
    //public static string ChangeFilename<StorageFolder, StorageFile>(StorageFile item, string newFileName, bool physically, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        ChangeFilename(item.ToString(), newFileName, physically);
    //    }
    //    ThrowNotImplementedUwp();
    //    return null;
    //}

    /// <summary>
    ///     A2 true - bs to slash. false - slash to bs
    /// </summary>
    /// <param name="path"></param>
    /// <param name="v"></param>
    public static string Slash(string path, bool slash)
    {
        string result = null;
        if (slash)
            result = path.Replace("\"",
                "/"); //SHReplace.ReplaceAll2(path, "/", "\"");
        else
            result = path.Replace("/",
                "\""); //SHReplace.ReplaceAll2(path, "\"", " / ");

        SH.FirstCharUpper(ref result);
        return result;
    }

    /// <summary>
    ///     Pokusí se max. 10x smazat soubor A1, pokud se nepodaří, GF, jinak GT
    /// </summary>
    /// <param name="item"></param>
    public static bool TryDeleteWithRepetition(string item)
    {
        var pokusyOSmazani = 0;
        while (true)
            try
            {
                File.Delete(item);
                return true;
            }
            catch
            {
                pokusyOSmazani++;
                if (pokusyOSmazani == 9) return false;
            }
    }

    public static bool TryDeleteFile(string item, out string message)
    {
        message = null;
        try
        {
            File.Delete(item);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    public static string GetSizeInAutoString(double size)
    {
        var unit = ComputerSizeUnits.B;
        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.KB;
            size /= NumConsts.kB;
        }

        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.MB;
            size /= NumConsts.kB;
        }

        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.GB;
            size /= NumConsts.kB;
        }

        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.TB;
            size /= NumConsts.kB;
        }

        return size + " " + unit;
    }

    public static string GetSizeInAutoString(long value, ComputerSizeUnits b)
    {
        return GetSizeInAutoString((double)value, b);
    }

    /// <summary>
    ///     A1 is input unit, not output
    /// </summary>
    /// <param name="value"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string GetSizeInAutoString(double value, ComputerSizeUnits b)
    {
        if (b != ComputerSizeUnits.B)
            // Získám hodnotu v bytech
            value = ConvertToSmallerComputerUnitSize(value, b, ComputerSizeUnits.B);


        if (value < 1024) return value + " B";

        var previous = value;
        value /= 1024;

        if (value < 1) return previous + " B";

        previous = value;
        value /= 1024;

        if (value < 1) return previous + " KB";

        previous = value;
        value /= 1024;
        if (value < 1) return previous + " MB";

        previous = value;
        value /= 1024;

        if (value < 1) return previous + " GB";

        return value + " TB";
    }

    private static long ConvertToSmallerComputerUnitSize(long value, ComputerSizeUnits b, ComputerSizeUnits to)
    {
        return ConvertToSmallerComputerUnitSize(value, b, to);
    }

    private static double ConvertToSmallerComputerUnitSize(double value, ComputerSizeUnits b, ComputerSizeUnits to)
    {
        if (to == ComputerSizeUnits.Auto)
            throw new Exception(
                "Byl specifikov\u00E1n v\u00FDstupn\u00ED ComputerSizeUnit, nem\u016F\u017Eu toto nastaven\u00ED zm\u011Bnit");
        if (to == ComputerSizeUnits.KB && b != ComputerSizeUnits.KB)
            value *= 1024;
        else if (to == ComputerSizeUnits.MB && b != ComputerSizeUnits.MB)
            value *= 1024 * 1024;
        else if (to == ComputerSizeUnits.GB && b != ComputerSizeUnits.GB)
            value *= 1024 * 1024 * 1024;
        else if (to == ComputerSizeUnits.TB && b != ComputerSizeUnits.TB) value *= 1024L * 1024L * 1024L * 1024L;

        return value;
    }


    /// <summary>
    ///     txt files (*.txt)|*.txt|All files (*.*)|*.*"
    /// </summary>
    /// <param name="filter"></param>
    public static string RepairFilter(string filter)
    {
        if (!filter.Contains("|"))
        {
            filter = filter.TrimStart('*');
            return "*" + filter + "|" + "*" + filter;
        }

        return filter;
    }


    /// <summary>
    ///     Replacement can be configured with replaceIncorrectFor
    /// </summary>
    /// <param name="p"></param>
    public static string ReplaceIncorrectCharactersFile(string p)
    {
        var t = p;
        foreach (var item in invalidFileNameChars)
        {
            var sb = new StringBuilder();
            foreach (var item2 in t)
                if (item != item2)
                    sb.Append(item2);
                else
                    sb.Append("");
            t = sb.ToString();
        }

        return t;
    }

    /// <summary>
    ///     ReplaceIncorrectCharactersFile - can specify char for replace with
    ///     ReplaceInvalidFileNameChars - all wrong chars skip
    ///     A2 - can specify more letter in one string
    ///     A3 is applicable only for A2. In general is use replaceIncorrectFor
    /// </summary>
    /// <param name="p"></param>
    /// <param name="replaceAllOfThisByA3"></param>
    /// <param name="replaceForThis"></param>
    public static string ReplaceIncorrectCharactersFile(string p, string replaceAllOfThisByA3, string replaceForThis)
    {
        var t = p;
        foreach (var item in invalidFileNameChars)
        {
            var sb = new StringBuilder();
            foreach (var item2 in t)
                if (item != item2)
                    sb.Append(item2);
                else
                    sb.Append(replaceForThis);
            t = sb.ToString();
        }

        if (!string.IsNullOrEmpty(replaceAllOfThisByA3))
            foreach (var item in replaceAllOfThisByA3)
                t = /*SHReplace.ReplaceAll*/ t.Replace(item.ToString(), replaceForThis)
                    ; //(t, replaceForThis, item.ToString());
        return t;
    }

    /// <summary>
    ///     Pro odstranění špatných znaků odstraní všechny výskyty A2 za mezery a udělá z více mezere jediné
    /// </summary>
    /// <param name="p"></param>
    /// <param name="replaceAllOfThisThen"></param>
    public static string ReplaceIncorrectCharactersFile(string p, string replaceAllOfThisThen)
    {
        var replaceFor = "";
        var t = p;
        foreach (var item in invalidFileNameChars)
        {
            var sb = new StringBuilder();
            foreach (var item2 in t)
                if (item != item2)
                    sb.Append(item2);
                else
                    sb.Append(replaceFor);
            t = sb.ToString();
        }

        if (!string.IsNullOrEmpty(replaceAllOfThisThen))
        {
            t = t.Replace(replaceAllOfThisThen,
                replaceFor); // SHReplace.ReplaceAll(t, replaceFor, replaceAllOfThisThen);
            t = t.Replace(" ",
                replaceFor); //SHReplace.ReplaceAll(t, replaceFor, "");
        }

        return t;
    }

    /// <summary>
    /// Toto vkládá jako novou složku.
    /// 
    ///     either A1 or A2 can be null
    ///     When A2 is null, will get from file path A1
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="parentFolder"></param>
    /// <param name="insert"></param>
    public static string InsertBetweenFileNameAndPath(string folder, string parentFolder, string insert)
    {
        ThrowEx.IsNotWindowsPathFormat(nameof(folder), folder, true, FS.IsWindowsPathFormat);

        if (parentFolder == null) parentFolder = Path.GetDirectoryName(folder);
        var outputFolder = Path.Combine(parentFolder, insert);
        CreateFoldersPsysicallyUnlessThere(outputFolder);
        return Path.Combine(outputFolder, Path.GetFileName(folder));
    }

    /// <summary>
    ///     Pokud hledáš metodu ReplacePathToFile, je to tato. Sloučeny protože dělali totéž.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="changeFolderTo"></param>
    public static string ChangeDirectory(string fileName, string changeFolderTo)
    {
        var p = Path.GetDirectoryName(fileName);
        var fn = Path.GetFileName(fileName);
        return Path.Combine(changeFolderTo, fn);
    }


    public static List<string> DirectoryListing(string path, string mask, SearchOption so)
    {
        var p = FSGetFiles.GetFiles(path, mask, so, new GetFilesArgsFS { _trimA1AndLeadingBs = true });

        return p;
    }

    public static string WithoutEndSlash(string v)
    {
        return WithoutEndSlash(ref v);
    }


    public static string WithoutEndSlash(ref string v)
    {
        v = v.TrimEnd('\\');
        return v;
    }

    public static string MascFromExtension(string ext2 = "*")
    {
        if (char.IsLetterOrDigit(ext2[0]))
            // For wildcard, dot (simply non letters) include .
            ext2 = "." + ext2;
        if (!ext2.StartsWith("*")) ext2 = "*" + ext2;
        if (!ext2.StartsWith("*.") && ext2.StartsWith(".")) ext2 = "*." + ext2;

        return ext2;

        //if (ext2 == ".*")
        //{
        //    return "*.*";
        //}


        //var ext = Path.GetExtension(ext2);
        //var fn = Path.GetFileNameWithoutExtension(ext2);
        //// isContained must be true, in BundleTsFile if false masc will be .ts, not *.ts and won't found any file
        //var isContained = AllExtensionsHelperSH.IsContained(ext);
        //ComplexInfoString cis = new ComplexInfoString(fn);

        ////Already tried
        ////(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0);
        //// (cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0); - in MoveClassElementIntoSharedFileUC
        //// !(!ext.Contains("*") && !ext.Contains("?") && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0)) - change due to MoveClassElementIntoSharedFileUC

        //// not working for *.aspx.cs
        ////var isNoMascEntered = !(!ext2.Contains("*") && !ext2.Contains("?") && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0));
        //// Is succifient one of inner condition false and whole is true

        //var isNoMascEntered = !((ext2.Contains("*") || ext2.Contains("?")));// && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0));
        //// From base of logic - isNoMascEntered must be without !. When something another wont working, edit only evaluate of condition above
        //if (!ext.StartsWith("*.") && isNoMascEntered && isContained && ext == Path.GetExtension( ext2))
        //{
        //    // Dont understand why, when I insert .aspx.cs, then return just .aspx without *
        //    //if (cis.QuantityUpperChars > 0 || cis.QuantityLowerChars > 0)
        //    //{
        //    //    return ext2;
        //    //}

        //    var vr = "*" + "." + ext2.TrimStart('.');
        //    return vr;
        //}

        //return ext2;
    }

    public static bool IsCountOfFilesMoreThan(string bpMb, string masc, int getNullIfThereIsMoreThanXFiles)
    {
        var f = FSGetFiles.GetFilesEveryFolder(bpMb, masc, SearchOption.AllDirectories,
            new GetFilesEveryFolderArgsFS { getNullIfThereIsMoreThanXFiles = getNullIfThereIsMoreThanXFiles });
        return f == null;
    }

    public static List<string> GetFiles(string folderPath, bool recursive)
    {
        return FSGetFiles.GetFiles(folderPath, "*.*",
            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
    }

    //    public static
    //#if ASYNC
    //    async Task<string>
    //#else
    //string
    //#endif
    //    ReadAllText(string filename)
    //    {
    //        return
    //#if ASYNC
    //        await
    //#endif
    //        File.ReadAllTextAsync(filename);
    //    }

    /// <summary>
    ///     A2 is path of target file
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fileTo"></param>
    /// <param name="co"></param>
    public static void MoveFile(string item, string fileTo, FileMoveCollisionOption co)
    {
        if (CopyMoveFilePrepare(ref item, ref fileTo, co))
            try
            {
                item = MakeUncLongPath(item);
                fileTo = MakeUncLongPath(fileTo);

                if (co == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo)) return;
                File.Move(item, fileTo);
            }
            catch (Exception ex)
            {
                //ThisApp.Error(item + " : " + ex.Message);
            }
    }

    public static bool CopyMoveFilePrepare(ref string item, ref string fileTo, FileMoveCollisionOption co)
    {
        //var fileTo = fileTo2.ToString();
        item = @"\\?\" + item;
        MakeUncLongPath(ref fileTo);
        CreateUpfoldersPsysicallyUnlessThere(fileTo);

        // Toto tu je důležité, nevím který kokot to zakomentoval
        if (File.Exists(fileTo))
        {
            if (co == FileMoveCollisionOption.AddFileSize)
            {
                var newFn = InsertBetweenFileNameAndExtension(fileTo, " " + new FileInfo(item).Length);
                if (File.Exists(newFn))
                {
                    File.Delete(item);
                    return true;
                }

                fileTo = newFn;
            }
            else if (co == FileMoveCollisionOption.AddSerie)
            {
                var serie = 1;
                while (true)
                {
                    var newFn = InsertBetweenFileNameAndExtension(fileTo, " (" + serie + ")");
                    if (!File.Exists(newFn))
                    {
                        fileTo = newFn;
                        break;
                    }

                    serie++;
                }
            }
            else if (co == FileMoveCollisionOption.DiscardFrom)
            {
                // Cant delete from because then is file deleting
                if (DeleteFileMaybeLocked != null)
                    DeleteFileMaybeLocked(item);
                else
                    File.Delete(item);
            }
            else if (co == FileMoveCollisionOption.Overwrite)
            {
                if (DeleteFileMaybeLocked != null)
                    DeleteFileMaybeLocked(fileTo);
                else
                    File.Delete(fileTo);
            }
            else if (co == FileMoveCollisionOption.LeaveLarger)
            {
                var fsFrom = new FileInfo(item).Length;
                var fsTo = new FileInfo(fileTo).Length;
                if (fsFrom > fsTo)
                    File.Delete(fileTo);
                else //if (fsFrom < fsTo)
                    File.Delete(item);
            }
            else if (co == FileMoveCollisionOption.DontManipulate)
            {
                if (File.Exists(fileTo)) return false;
            }
            else if (co == FileMoveCollisionOption.ThrowEx)
            {
                ThrowEx.Custom($"Directory {fileTo} already exists");
            }
        }

        return true;
    }

    public static long GetFileSize(string item)
    {
        FileInfo fi = null;
        try
        {
            fi = new FileInfo(item);
        }
        catch (Exception ex)
        {
            // Například příliš dlouhý název souboru
            return 0;
        }

        if (fi.Exists) return fi.Length;
        return 0;
    }


    public static void CopyAllFilesRecursively(string p, string to, FileMoveCollisionOption co, string contains = null)
    {
        CopyMoveAllFilesRecursively(p, to, co, false, contains, SearchOption.AllDirectories);
    }

    /// <summary>
    ///     A4 contains can use ! for negation
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    /// <param name="co"></param>
    /// <param name="contains"></param>
    public static void CopyAllFiles(string p, string to, FileMoveCollisionOption co, string contains = null)
    {
        CopyMoveAllFilesRecursively(p, to, co, false, contains, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    ///     If want use which not contains, prefix A4 with !
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    /// <param name="co"></param>
    /// <param name="mustContains"></param>
    private static void CopyMoveAllFilesRecursively(string p, string to, FileMoveCollisionOption co, bool move,
        string mustContains, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(p, "*", so);
        if (!string.IsNullOrEmpty(mustContains))
        {
            foreach (var item in files)
                if (SH.IsContained(item, mustContains))
                {
                    if (item.Contains("node_modules"))
                    {
                    }

                    MoveOrCopy(p, to, co, move, item);
                }
        }
        else
        {
            foreach (var item in files) MoveOrCopy(p, to, co, move, item);
        }
    }

    private static void MoveOrCopy(string p, string to, FileMoveCollisionOption co, bool move, string item)
    {
        var fileTo = to + item.Substring(p.Length);
        if (move)
            MoveFile(item, fileTo, co);
        else
            CopyFile(item, fileTo, co);
    }

    public static
        void
        CopyFile(string item2, string fileTo2, FileMoveCollisionOption co, bool terminateProcessIfIsInUsed = false)
    {
        var fileTo = fileTo2;
        var item = item2;

        var rr =
            CopyMoveFilePrepare(ref item, ref fileTo, co);

        if (rr)
        {
            if (co == FileMoveCollisionOption.DontManipulate &&
                File.Exists(fileTo))
                return;

            CopyFile(item, fileTo, terminateProcessIfIsInUsed);
        }
    }

    /// <summary>
    ///     Copy file by ordinal way
    ///     tady byly 2 metody CopyFile(string, string, bool)
    ///     jedna s A3 terminateProcessIfIsInUsed, druhá s overwrite
    ///     Ta druhá jen volala A3 s FileMoveCollisionOption.Overwrite
    /// </summary>
    /// <param name="jsFiles"></param>
    /// <param name="v"></param>
    public static void CopyFile(string jsFiles, string v, bool terminateProcessIfIsInUsed = false)
    {
        try
        {
            File.Copy(jsFiles, v, true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("because it is being used by another process") && terminateProcessIfIsInUsed)
            {
                if (fileUtilWhoIsLocking != null)
                {
                    var pr = fileUtilWhoIsLocking(jsFiles, true);
                    foreach (var item in pr) item.Kill();
                }

                // Používá se i ve web, musel bych tam includovat spoustu metod
                //PH.ShutdownProcessWhichOccupyFileHandleExe(jsFiles);
                try
                {
                    File.Copy(jsFiles, v, true);
                }
                catch (Exception)
                {
                    // Pokud se to ani na druhý pokus nepodaří, tak už to jebu
                }
            }
            else
            {
                throw;
            }
        }
    }

    public static void CopyFile(string item, string fileTo2, FileMoveCollisionOption co)
    {
        var fileTo = fileTo2;
        if (CopyMoveFilePrepare(ref item, ref fileTo, co))
        {
            if (co == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo)) return;

            File.Copy(item, fileTo);
        }
    }

    public static DateTime LastModified(string rel)
    {
        if (File.Exists(rel)) return File.GetLastWriteTime(rel);
        // FileInfo mi držel soubor a vznikali chyby The process cannot access the file
        //var f = new FileInfo(rel);
        //var r = f.LastWriteTime;
        //return r;
        return DateTime.MinValue;
    }


    public static bool TryDeleteDirectoryOrFile(string v)
    {
        if (!TryDeleteDirectory(v)) return TryDeleteFile(v);
        return true;
    }

    //public static Func<string, List<string>> InvokePs;

    /// <summary>
    ///     Before start you can create instance of PowershellRunner to try do it with PS
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static bool TryDeleteDirectory(string v)
    {
        if (!Directory.Exists(v)) return true;

        try
        {
            Directory.Delete(v, true);
            return true;
        }
        catch (Exception ex)
        {
            // Je to try takže nevím co tu dělá tohle a
            //ThrowEx.FolderCannotBeDeleted(v, ex);
            //var result = InvokePs(v);
            //if (result.Count > 0)
            //{
            //    return false;
            //}
        }

        var files = FSGetFiles.GetFiles(v, "*", SearchOption.AllDirectories);
        foreach (var item in files) File.SetAttributes(item, FileAttributes.Normal);

        try
        {
            Directory.Delete(v, true);
            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
    }


    public static string AllIncludeIfOnlyLetters(string item)
    {
        item = item.ToLower().TrimStart('*').TrimStart('.');
        //if ( SH.ContainsOnlyCase(item.ToLower(), false, false))
        //{
        item = "*." + item;
        //}


        return item;
    }


    /// <summary>
    ///     Get number higher by one from the number filenames with highest value (as 3.txt)
    /// </summary>
    /// <param name="slozka"></param>
    /// <param name="fn"></param>
    /// <param name="ext"></param>
    public static string GetFileSeries(string slozka, string fn, string ext)
    {
        var dalsi = 0;
        var soubory = FSGetFiles.GetFiles(slozka);
        foreach (var item in soubory)
        {
            int p;
            var withoutFn =
                new Regex(fn).Replace(Path.GetFileName(item), "",
                    1); /*SHReplace.ReplaceOnce(Path.GetFileName(item), fn, "")));*/
            var withoutFnAndExt = SHReplace.ReplaceOnce(withoutFn, ext, "");
            withoutFnAndExt = withoutFnAndExt.TrimStart('_');
            if (int.TryParse(withoutFnAndExt, out p))
                if (p > dalsi)
                    dalsi = p;
        }

        dalsi++;

        return Path.Combine(slozka, fn + "_" + dalsi + ext);
    }


    ///// <summary>
    ///// If path ends with backslash, Path.GetDirectoryName returns empty string
    ///// </summary>
    ///// <param name="rp"></param>
    //public static string GetFileName(string rp)
    //{
    //    rp = rp.TrimEnd('\\');
    //    int dex = rp.LastIndexOf('\\');
    //    return rp.Substring(dex + 1);
    //}


    //public static bool ExistsDirectory<StorageFolder, StorageFile>(string item, AbstractCatalog<StorageFolder, StorageFile> ac = null, bool _falseIfContainsNoFile = false)
    //{
    //    if (ac == null)
    //    {
    //        return ExistsDirectoryWorker(item, _falseIfContainsNoFile);
    //    }
    //    else
    //    {
    //        // Call from Apps
    //        return BTS.GetValueOfNullable(ac.Directory.Exists.Invoke(item));
    //    }
    //}


    //public static void MakeUncLongPath<StorageFolder, StorageFile>(ref StorageFile path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        path = (StorageFile)(dynamic)MakeUncLongPath(path.ToString());
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //    }
    //    //return path;
    //}


    //public static string MakeUncLongPath(string path)
    //{
    //    return se.FS.MakeUncLongPath(path);
    //}

    //public static string MakeUncLongPath(ref string path)
    //{
    //    return se.FS.MakeUncLongPath(ref path);
    //}

    /// <summary>
    ///     For empty or whitespace return false.
    /// </summary>
    /// <param name="selectedFile"></param>
    //public static bool ExistsFileAc<StorageFolder, StorageFile>(StorageFile selectedFile, AbstractCatalog<StorageFolder, StorageFile> ac = null)
    //{
    //    if (ac == null)
    //    {
    //        return File.Exists(selectedFile.ToString());
    //    }
    //    return ac.fs.existsFile.Invoke(selectedFile);
    //}

    ///// <summary>
    ///// Create all upfolders of A1, if they dont exist
    ///// </summary>
    ///// <param name="nad"></param>
    //public static void CreateUpfoldersPsysicallyUnlessThereAc<StorageFolder, StorageFile>(StorageFile nad, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        CreateUpfoldersPsysicallyUnlessThere(nad.ToString());
    //    }
    //    else
    //    {
    //        CreateFoldersPsysicallyUnlessThereFolder<StorageFolder, StorageFile>(Path.GetDirectoryName<StorageFolder, StorageFile>(nad, ac), ac);
    //    }
    //}

    ///// <summary>
    ///// Works with and without end backslash
    ///// Return with backslash
    ///// </summary>
    ///// <param name="rp"></param>
    //public static StorageFolder GetDirectoryName<StorageFolder, StorageFile>(StorageFile rp2, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.Path.GetDirectoryName.Invoke(rp2);
    //    }

    //    var rp = rp2.ToString();
    //    return (dynamic)GetDirectoryName(rp);
    //}

    //public static void CreateFoldersPsysicallyUnlessThereFolder<StorageFolder, StorageFile>(StorageFolder nad, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        CreateFoldersPsysicallyUnlessThere(nad.ToString());
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //    }
    //}
    public static bool? ExistsDirectoryNull(string item)
    {
        return ExistsDirectoryNull(item, false);
    }

    public static bool? ExistsDirectoryNull(string item, bool _falseIfContainsNoFile = false)
    {
        return ExistsDirectory(item, _falseIfContainsNoFile);
    }

    public static bool ExistsDirectory(string item, bool _falseIfContainsNoFile = false)
    {
        return Directory.Exists(item);
        //return ExistsDirectory<string, string>(item, null, _falseIfContainsNoFile);
    }

    #region For easy copy

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static string NormalizeExtension2(string item)
    //{
    //    return se.FS.NormalizeExtension2(item);
    //}

    public static string NonSpacesFilename(string nameOfPage)
    {
        ThrowEx.NotImplementedMethod();
        return null;

        //var v = ConvertCamelConventionWithNumbers.ToConvention(nameOfPage);
        //v = FS.ReplaceInvalidFileNameChars(v);
        //return v;
    }

    #endregion

    #region Making problem in translate

    /// <summary>
    ///     Delete whole folder A1. If fail, only "1" subdir
    ///     Use in RepairBlogContent but sample data is NA
    ///     Deleting old folder hiearchy and create new
    /// </summary>
    /// <param name="repairedBlogPostsFolder"></param>
    public static int DeleteSerieDirectoryOrCreateNew(string repairedBlogPostsFolder)
    {
        var resultSerie = 1;
        var folders = Directory.GetDirectories(repairedBlogPostsFolder);

        var deleted = true;
        // 0 or 1
        if (folders.Length < 2)
            try
            {
                Directory.Delete(repairedBlogPostsFolder, true);
            }
            catch (Exception ex)
            {
                ThrowEx.FolderCannotBeDeleted(repairedBlogPostsFolder, ex);
                deleted = false;
            }

        var withEndFlash = WithEndSlash(repairedBlogPostsFolder);

        if (!deleted)
        {
            // confuse me, dir can exists
            // Here seems to be OK on 8-7-19 (unit test)
            Directory.CreateDirectory(withEndFlash + @"1" + "\\");
        }
        else
        {
            // When deleting will be successful, create new dir
            var generator = new TextOutputGenerator();
            generator.sb.Append(withEndFlash);
            //generator.sb.CanUndo = true;
            for (; resultSerie < int.MaxValue; resultSerie++)
            {
                generator.sb.Append(resultSerie);
                var newDirectory = generator.ToString();
                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                    break;
                }

                generator.Undo();
            }
        }

        return resultSerie;
    }

    public static SearchOption ToSearchOption(bool? recursive)
    {
        return recursive.GetValueOrDefault() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    }


    public static void WriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public static bool IsAllInSameFolder(List<string> c)
    {
        if (c.Count > 0)
        {
            var f = Path.GetDirectoryName(c[0]);

            for (var i = 1; i < c.Count; i++)
                if (Path.GetDirectoryName(c[i]) != f)
                    return false;
        }

        return true;
    }

    public static void CreateFileWithTemplateContent(string folder, string files, string ext,
        string templateFromContent)
    {
        var lines = SHGetLines.GetLines(files);

        foreach (var item in lines)
        {
            var path = Path.Combine(folder, item + ext);
            if (!File.Exists(path)) File.WriteAllText(path, templateFromContent);
        }
    }

    public static bool ContainsInvalidFileNameChars(string arg)
    {
        foreach (var item in invalidFileNameStrings)
            if (arg.Contains(item))
                return true;

        return false;
    }

    public static void NumberByDateModified(string folder, string masc, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(folder, masc, so, new GetFilesArgsFS { byDateOfLastModifiedAsc = true });
        var i = 1;
        foreach (var item in files)
        {
            RenameFile(item, i + Path.GetExtension(item), FileMoveCollisionOption.DontManipulate);
            i++;
        }
    }

    #endregion

    #region GetDirectoryName

    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="rp"></param>
    /// <returns></returns>
    public static string GetDirectoryName(string rp)
    {
        // Zde zároveň vyhazuji výjimky
        var deli = DetectPathDelimiterChar(rp);


        if (string.IsNullOrEmpty(rp)) ThrowEx.IsNullOrEmpty("rp", rp);

        if (!IsWindowsPathFormat(rp)) ThrowEx.IsNotWindowsPathFormat("rp", rp, true, FS.IsWindowsPathFormat);

        rp = rp.TrimEnd(deli);
        var dex = rp.LastIndexOf(deli);
        if (dex != -1)
        {
            var result = rp.Substring(0, dex + 1);
            FirstCharUpper(ref result);
            return result;
        }


        return "";
    }

    public static Tuple<bool, bool> DetectPathDelimiter(string rp)
    {
        var containsFs = rp.Contains("/");
        var containsBs = rp.Contains("\\");

        if (containsBs && containsFs) throw new Exception("Path contains both fs & bs");

        return Tuple.Create(containsFs, containsBs);
    }

    public static char DetectPathDelimiterChar(string rp)
    {
        var t = DetectPathDelimiter(rp);

        var containsFs = t.Item1;
        var containsBs = t.Item2;

        var deli = 'a';

        if (containsBs)
            deli = '\\';
        else if (containsFs)
            deli = '/';
        else
            throw new Exception("Path contains no delimiter");

        return deli;
    }

    /// <summary>
    ///     Usage: Exceptions.IsNotWindowsPathFormat
    /// </summary>
    /// <param name="argValue"></param>
    /// <returns></returns>
    public static bool IsWindowsPathFormat(string argValue)
    {
        if (string.IsNullOrWhiteSpace(argValue)) return false;

        var badFormat = false;

        if (argValue.Length < 3) return badFormat;

        if (!char.IsLetter(argValue[0])) badFormat = true;

        if (char.IsLetter(argValue[1])) badFormat = true;

        if (argValue.Length > 2)
            if (argValue[1] != '\\' && argValue[2] != '\\')
                badFormat = true;

        return !badFormat;
    }

    #endregion

    #region MakeUncLongPath

    public static string MakeUncLongPath(string path)
    {
        return MakeUncLongPath(ref path);
    }

    public static string MakeUncLongPath(ref string path)
    {
        if (!path.StartsWith(@"\\?\"))
        {
            // V ASP.net mi vrátilo u každé directory.exists false. Byl jsem pod ApplicationPoolIdentity v IIS a bylo nastaveno Full Control pro IIS AppPool\DefaultAppPool
#if !ASPNET
            //  asp.net / vps nefunguje, ve windows store apps taktéž, NECHAT TO TRVALE ZAKOMENTOVANÉ
            // v asp.net toto způsobí akorát zacyklení, IIS začne vyhazovat 0xc00000fd, pak už nejde načíst jediná stránka
            //path = @"\\?\" + path;
#endif
        }

        return path;
    }

    #endregion


    //public static string GetFileNameWithoutExtension(string s)
    //{
    //    return Path.GetFileNameWithoutExtension(s);
    //    //return GetFileNameWithoutExtension<string, string>(s, null);
    //}


    //public static bool IsFileHasKnownExtension(string relativeTo)
    //{
    //    var ext = Path.GetExtension(relativeTo);
    //    ext = FS.NormalizeExtension2(ext);


    //    return AllExtensionsHelperWithoutDot.allExtensionsWithoutDot.ContainsKey(ext);
    //}

    //public static string PathWithoutExtension(string path)
    //{
    //    string path2, file, ext;
    //    GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
    //    return Combine(path2, file);
    //}

    ///// <summary>
    ///// Vrátí cestu a název souboru bez ext a ext
    ///// All returned is normal case
    ///// </summary>
    ///// <param name="fn"></param>
    ///// <param name="path"></param>
    ///// <param name="file"></param>
    ///// <param name="ext"></param>
    //public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    //{
    //    path = Path.GetDirectoryName(fn) + '\\';
    //    file = GetFileNameWithoutExtension(fn);
    //    ext = Path.GetExtension(fn);
    //}

    ///// <summary>
    ///// Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    ///// if have more extension, remove just one
    ///// </summary>
    ///// <param name="s"></param>
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile s, AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ss = s.ToString();
    //        var vr = Path.GetFileNameWithoutExtension(ss.TrimEnd('\\'));
    //        var ext = Path.GetExtension(ss).TrimStart('.');

    //        if (!SH.ContainsOnly(ext, RandomHelper.allCharsWithoutSpecial))
    //        {
    //            if (ext != string.Empty)
    //            {
    //                return (dynamic)vr + "." + ext;
    //            }
    //        }
    //        return (dynamic)vr;
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //        return s;
    //    }
    //}

    //public static string GetFileNameWithoutExtension(string s)
    //{
    //    return GetFileNameWithoutExtension<string, string>(s, null);
    //}

    //private static string Combine(params string[] path2)
    //{
    //    return Path.Combine(path2);
    //}

    #region MakeUncLongPath

    #endregion


    #region from FSShared64.cs

    //        /// <summary>
    //        /// Convert to UNC path
    //        /// </summary>
    //        /// <param name="item"></param>
    //        public static bool ExistsDirectoryWorker(string item, bool _falseIfContainsNoFile = false)
    //        {
    //            // Not working, flags from GeoCachingTool wasnt transfered to standard
    //#if NETFX_CORE
    //        ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
    //#endif
    //#if WINDOWS_UWP
    //        ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
    //#endif

    //            if (item == @"\\?\" || item == string.Empty)
    //            {
    //                return false;
    //            }

    //            var item2 = MakeUncLongPath(item);

    //            // Directory.Exists if pass SE or only start of Unc return false
    //            var result = Directory.Exists(item2);
    //            if (_falseIfContainsNoFile)
    //            {
    //                if (result)
    //                {
    //                    var f = GetFilesSimple(item, "*", SearchOption.AllDirectories).Count;
    //                    result = f > 0;
    //                }
    //            }
    //            return result;
    //        }

    //public static bool IsCountOfFilesMoreThan(string bpMb, int v)
    //{
    //    return false;
    //}

    #region FirstCharUpper

    public static string FirstCharUpper(ref string result)
    {
        if (IsWindowsPathFormat(result)) result = SH.FirstCharUpper(result);

        return result;
    }

    public static string FirstCharUpper(string nazevPP, bool only = false)
    {
        if (nazevPP != null)
        {
            var sb = nazevPP.Substring(1);
            if (only) sb = sb.ToLower();

            return nazevPP[0].ToString().ToUpper() + sb;
        }

        return null;
    }

    #endregion


    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    public static void GetPathAndFileName(string fn, out string path, out string file)
    {
        path = WithEndSlash(GetDirectoryName(fn));
        file = GetFileName(fn);
    }

    public static string GetFileName(string fn)
    {
        return PathMs.GetFileName(fn.TrimEnd(Path.DirectorySeparatorChar));
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension2(string item)
    {
        return item.ToLower().TrimStart('.');
    }


    //private static void ThrowNotImplementedUwp()
    //{
    //    throw new Exception("Not implemented in UWP");
    //}

    //        private static Type type = typeof(FS);

    //
    //        #region

    //        #endregion
    //
    //
    //

    //
    //        private static long ConvertToSmallerComputerUnitSize(long value, ComputerSizeUnits b, ComputerSizeUnits to)
    //        {
    //            return ConvertToSmallerComputerUnitSize(value, b, to);
    //        }
    //
    //

    //
    //        public static string GetSizeInAutoString(long value, ComputerSizeUnits b)
    //        {
    //            if (b != ComputerSizeUnits.B)
    //            {
    //                // Získám hodnotu v bytech
    //                value = ConvertToSmallerComputerUnitSize(value, b, ComputerSizeUnits.B);
    //            }
    //
    //
    //            if (value < 1024)
    //            {
    //                return value + " B";
    //            }
    //
    //            double previous = value;
    //            value /= 1024;
    //
    //            if (value < 1)
    //            {
    //                return previous + " B";
    //            }
    //
    //            previous = value;
    //            value /= 1024;
    //
    //            if (value < 1)
    //            {
    //                return previous + " KB";
    //            }
    //
    //            previous = value;
    //            value /= 1024;
    //            if (value < 1)
    //            {
    //                return previous + " MB";
    //            }
    //
    //            previous = value;
    //            value /= 1024;
    //
    //            if (value < 1)
    //            {
    //                return previous + " GB";
    //            }
    //
    //            return value + " TB";
    //        }
    //
    //
    //        public static List<string> GetFiles(string path, string v, SearchOption topDirectoryOnly)
    //        {
    //            return GetFilesMoreMasc(path, v, topDirectoryOnly).ToList();
    //        }
    //
    //        public static List<string> GetFilesMoreMasc(string path, string v, SearchOption topDirectoryOnly)
    //        {
    //            var c = ",";
    //            var sc = ";";
    //            List<string> result = new List<string>();
    //            List<string> masks = new List<string>();
    //
    //            if (v.Contains(c))
    //            {
    //                masks.AddRange(SHSplit.SplitMore(v, c));
    //            }
    //            else if (v.Contains(sc))
    //            {
    //                masks.AddRange(SHSplit.SplitMore(v, sc));
    //            }
    //            else
    //            {
    //                masks.Add(v);
    //            }
    //
    //            foreach (var item in masks)
    //            {
    //                result.AddRange(GetFiles(path, item, topDirectoryOnly));
    //            }
    //
    //            return result;
    //        }
    //

    //
    //        public static void CreateUpfoldersPsysicallyUnlessThere(string nad)
    //        {
    //            CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(nad));
    //        }
    //
    //        /// <summary>
    //        /// Create all upfolders of A1 with, if they dont exist
    //        /// </summary>
    //        /// <param name="nad"></param>
    //        public static void CreateFoldersPsysicallyUnlessThere(string nad)
    //        {
    //            ThrowEx.IsNullOrEmpty("nad", nad);
    //            //ThrowEx.IsNotWindowsPathFormat("nad", nad);
    //
    //            FS.MakeUncLongPath(ref nad);
    //            if (Directory.Exists(nad))
    //            {
    //                return;
    //            }
    //            else
    //            {
    //                List<string> slozkyKVytvoreni = new List<string>();
    //                slozkyKVytvoreni.Add(nad);
    //
    //                while (true)
    //                {
    //                    nad = Path.GetDirectoryName(nad);
    //
    //                    // TODO: Tady to nefunguje pro UWP/UAP apps protoze nemaji pristup k celemu disku. Zjistit co to je UWP/UAP/... a jak v nem ziskat/overit jakoukoliv slozku na disku
    //                    if (Directory.Exists(nad))
    //                    {
    //                        break;
    //                    }
    //
    //                    string kopia = nad;
    //                    slozkyKVytvoreni.Add(kopia);
    //                }
    //
    //                slozkyKVytvoreni.Reverse();
    //                foreach (string item in slozkyKVytvoreni)
    //                {
    //                    string folder = FS.MakeUncLongPath(item);
    //                    if (!Directory.Exists(folder))
    //                    {
    //                        Directory.CreateDirectory(folder);
    //                    }
    //                }
    //            }
    //        }
    //
    //        public static string ReadAllText(string filename)
    //        {
    //            return File.ReadAllTextAsync(filename);
    //        }
    //
    //        #region MyRegion
    //
    //
    //        private static void MakeUncLongPath(ref string nad)
    //        {
    //
    //        }
    //

    //        #endregion
    //
    //        #region Just in SunamoExceptions
    //        public static void CreateFileIfDoesntExists(string path)
    //        {
    //            if (!File.Exists(path))
    //            {
    //                File.WriteAllText(path, string.Empty);
    //            }
    //        }
    //        #endregion


    //        /// <summary>
    //        /// Dont check for size
    //        /// Into A2 is good put true - when storage was fulled, all new files will be written with zero size. But then failing because HtmlNode as null - empty string as input
    //        /// But when file is big, like backup of DB, its better false.. Then will be avoid reading whole file to determining their size and totally blocking HW resources on VPS
    //        ///
    //        /// A2 must be false otherwise read file twice
    //        ///
    //        /// Change falseIfSizeZeroOrEmpty = false. Its extremely resource intensive
    //        /// </summary>
    //        /// <param name="selectedFile"></param>
    //        public static bool ExistsFile(string selectedFile, bool falseIfSizeZeroOrEmpty = false)
    //        {
    //            SH.FirstCharUpper(ref selectedFile);
    //            //ThrowEx.FirstLetterIsNotUpper(selectedFile);

    //            if (filesWhichSurelyExists.Contains(selectedFile))
    //            {
    //                return true;
    //            }

    //#if DEBUG

    //#endif

    //            if (selectedFile == @"\\?\" || selectedFile == string.Empty)
    //            {
    //                return false;
    //            }

    //            FS.MakeUncLongPath(ref selectedFile);

    //            var exists = File.Exists(selectedFile);

    //            if (falseIfSizeZeroOrEmpty)
    //            {
    //                if (!exists)
    //                {

    //                    return false;
    //                }
    //                else
    //                {
    //                    var ext = Path.GetExtension(selectedFile).ToLower();
    //                    // Musím to kontrolovat jen když je to tmp, logicky
    //                    if (ext == AllExtensions.tmp)
    //                    {
    //                        return false;
    //                    }
    //                    else
    //                    {
    //                        var c = string.Empty;
    //                        try
    //                        {
    //                            c = File.ReadAllTextAsync(selectedFile);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            if (ex.Message.StartsWith("The process cannot access the file"))
    //                            {
    //                                return true;
    //                            }

    //                        }

    //                        if (c == string.Empty)
    //                        {
    //                            // Měl jsem tu chybu že ač exists bylo true, File.ReadAllText selhalo protože soubor neexistoval.
    //                            // Vyřešil jsem to kontrolou přípony, snad
    //                            return false;
    //                        }
    //                    }
    //                }
    //            }
    //            return exists;
    //        }


    ///// <summary>
    ///// Cant return with end slash becuase is working also with files
    ///// Use this than Path.Combine which if argument starts with backslash ignore all arguments before this
    ///// </summary>
    ///// <param name="upFolderName"></param>
    ///// <param name="dirNameDecoded"></param>
    //public static string Combine(params string[] s)
    //{
    //    return CombineWorker(true, s);
    //}

    ///// <summary>
    ///// Cant return with end slash becuase is working also with files
    ///// </summary>
    ///// <param name="FirstCharUpper"></param>
    ///// <param name="s"></param>
    //private static string CombineWorker(bool FirstCharUpper, params string[] s)
    //{
    //    s = CA.TrimStart('\\', s).ToArray();
    //    var result = Path.Combine(s);
    //    if (FirstCharUpper)
    //    {
    //        result = SH.FirstCharUpper(ref result);
    //    }
    //    else
    //    {
    //        result = SH.FirstCharUpper(ref result);
    //    }
    //    // Cant return with end slash becuase is working also with files
    //    //FS.WithEndSlash(ref result);
    //    return result;
    //}

    //public static string GetFileNameWithoutExtension(string s)
    //{
    //    return GetFileNameWithoutExtension<string, string>(s, null);
    //}

    /// <summary>
    /// Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    /// if have more extension, remove just one
    /// </summary>
    /// <param name = "s" ></ param >
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile s, AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ss = s.ToString();
    //        var vr = Path.GetFileNameWithoutExtension(ss.TrimEnd('\\'));
    //        var ext = Path.GetExtension(ss).TrimStart('.');

    //        if (!SH.ContainsOnly(ext, RandomHelper.allCharsWithoutSpecial))
    //        {
    //            if (ext != string.Empty)
    //            {
    //                return (dynamic)vr + "." + ext;
    //            }
    //        }
    //        return (dynamic)vr;
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //        return s;
    //    }
    //}


    //        #region  from FSShared.cs
    //        public static void DeleteFile(string item)
    //        {
    //            File.Delete(item);
    //        }
    //
    //
    /// <summary>
    ///     Usage: FileWasntFoundInDirectory
    ///     Vrátí cestu a název souboru s ext
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>

    //
    //        /// <summary>
    //        /// Vrátí cestu a název souboru bez ext a ext
    //        /// All returned is normal case
    //        /// </summary>
    //        /// <param name="fn"></param>
    //        /// <param name="path"></param>
    //        /// <param name="file"></param>
    //        /// <param name="ext"></param>
    //        public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    //        {
    //            path = Path.GetDirectoryName(fn) + '\\';
    //            file = Path.GetFileNameWithoutExtension(fn);
    //            ext = Path.GetExtension(fn);
    //        }
    //
    //        public static string PathWithoutExtension(string path)
    //        {
    //            string path2, file, ext;
    //            FS.GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
    //            return Path.Combine(path2, file);
    //        }
    //
    //        public static string GetFullPath(string vr)
    //        {
    //            var result = Path.GetFullPath(vr);
    //            SH.FirstCharUpper(ref result);
    //            return result;
    //        }
    //
    //        public static void FileToDirectory(ref string dir)
    //        {
    //            if (!dir.EndsWith("\""))
    //            {
    //                dir = Path.GetDirectoryName(dir);
    //            }
    //        }
    //
    //        #endregion

    #endregion

    #region GetFilesMoreMasc - in thread

    //public static List<string> GetFilesMoreMasc(string path, string masc, SearchOption searchOption, GetFilesMoreMascArgs e = null)
    //{
    //    if (e == null)
    //    {
    //        e = new GetFilesMoreMascArgs();
    //    }

    //    e.path = path;
    //    e.masc = masc;
    //    e.searchOption = searchOption;

    //    return GetFilesMoreMasc(e);
    //}

    //public static List<string> GetFilesMoreMasc(GetFilesMoreMascArgs e = null)
    //{
    //    Thread t = new Thread(new ParameterizedThreadStart(GetFilesMoreMascWorker));

    //    t.Start();

    //}

    //private static void GetFilesMoreMascWorker(object o)
    //{
    //var e = (GetFilesMoreMascArgs)o;

    #endregion

    public static string FilesWithSameName(string vs, string v, SearchOption allDirectories)
    {
        WithEndSlash(ref vs);

        var f = new Dictionary<string, List<string>>();
        var s = FSGetFiles.GetFiles(vs, v, allDirectories);
        foreach (var item in s) DictionaryHelper.AddOrCreate(f, Path.GetFileName(item), item);

        var sb = new StringBuilder();

        //TextOutputGenerator tog = new TextOutputGenerator();
        foreach (var item in f)
            if (item.Value.Count > 1)
            {
                foreach (var item2 in item.Value) sb.AppendLine(item2);
                sb.AppendLine();
                sb.AppendLine();
                //tog.List(item.Value);
            }

        return sb.ToString();
    }

    #region For easy copy - GetNameWithoutSeries

    /// <summary>
    ///     Do A1 se dává buď celá cesta ke souboru, nebo jen jeho název(může být i včetně neomezeně přípon)
    ///     A2 říká, zda se má vrátit plná cesta ke souboru A1, upraví se pouze samotný název souboru
    ///     Works for brackets, not dash
    /// </summary>
    public static string GetNameWithoutSeries(string p, bool path)
    {
        int serie;
        var hasSerie = false;
        return GetNameWithoutSeries(p, path, out hasSerie, SerieStyleFS.Brackets, out serie);
    }
    //public static string GetNameWithoutSeries(string p, bool path, out bool hasSerie, SerieStyle serieStyle)
    //{
    //    int serie;
    //    return GetNameWithoutSeries(p, path, out hasSerie, serieStyle, out serie);
    //}

    public static (string, bool) GetNameWithoutSeriesNoOut(string p, bool path, SerieStyleFS serieStyle)
    {
        int serie;
        var result = GetNameWithoutSeries(p, path, out var hasSerie, serieStyle, out serie);
        return (result, hasSerie);
    }

    public static string GetNameWithoutSeries(string p, bool path, out bool hasSerie, SerieStyleFS serieStyle)
    {
        int serie;
        return GetNameWithoutSeries(p, path, out hasSerie, serieStyle, out serie);
    }

    /// <summary>
    ///     Vrací vždy s příponou
    ///     Do A1 se dává buď celá cesta ke souboru, nebo jen jeho název(může být i včetně neomezeně přípon)
    ///     A2 říká, zda se má vrátit plná cesta ke souboru A1, upraví se pouze samotný název souboru
    ///     When file has unknown extension, return SE
    ///     Default for A4 was bracket
    /// </summary>
    /// <param name="p"></param>
    /// <param name="a1IsWithPath"></param>
    /// <param name="hasSerie"></param>
    public static string GetNameWithoutSeries(string p, bool a1IsWithPath, out bool hasSerie, SerieStyleFS serieStyle,
        out int serie)
    {
        serie = -1;
        hasSerie = false;
        var dd = string.Empty;

        if (a1IsWithPath) dd = WithEndSlash(Path.GetDirectoryName(p));

        var sbExt = new StringBuilder();
        var ext = Path.GetExtension(p);
        if (ext == string.Empty) return p;

        var pocetSerii = 0;

        p = SHParts.RemoveAfterLast(p, ".");
        sbExt.Append(ext);

        ext = sbExt.ToString();

        var g = p;

        if (dd.Length != 0) g = g.Substring(dd.Length);

        // Nejdříve ořežu všechny přípony a to i tehdy, má li soubor více přípon

        if (serieStyle == SerieStyleFS.Brackets || serieStyle == SerieStyleFS.All)
            while (true)
            {
                g = g.Trim();
                var lb = g.LastIndexOf('(');
                var rb = g.LastIndexOf(')');

                if (lb != -1 && rb != -1)
                {
                    var between = g.Substring(lb, rb - lb); //SH.GetTextBetweenTwoCharsInts(g, lb, rb);
                    if (double.TryParse(between, out var _) /*SH.IsNumber(between, [])*/)
                    {
                        serie = int.Parse(between);
                        pocetSerii++;
                        // s - 4, on end (1) -
                        g = g.Substring(0, lb);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

        if (serieStyle == SerieStyleFS.Dash || serieStyle == SerieStyleFS.All)
        {
            var dex = g.IndexOf('-');

            if (g[g.Length - 3] == '-')
            {
                serie = int.Parse(g.Substring(g.Length - 2));
                g = g.Substring(0, g.Length - 3);
            }
            else if (g[g.Length - 2] == '-')
            {
                serie = int.Parse(g.Substring(g.Length - 1));
                g = g.Substring(0, g.Length - 2);
            }

            if (serie != -1)
                // To true hasSerie
                pocetSerii++;
        }

        if (serieStyle == SerieStyleFS.Underscore || serieStyle == SerieStyleFS.All)
            RemoveSerieUnderscore(ref serie, ref g, ref pocetSerii);

        if (pocetSerii != 0) hasSerie = true;
        g = g.Trim();
        if (a1IsWithPath) return dd + g + ext;
        return g + ext;
    }

    public static string RemoveSerieUnderscore(string d)
    {
        var serie = 0;
        var pocetSerii = 0;
        RemoveSerieUnderscore(ref serie, ref d, ref pocetSerii);
        return d;
    }

    private static void RemoveSerieUnderscore(ref int serie, ref string g, ref int pocetSerii)
    {
        while (true)
        {
            var dex = g.LastIndexOf('_');
            if (dex != -1)
            {
                var serieS = g.Substring(dex + 1);
                g = g.Substring(0, dex);

                if (int.TryParse(serieS, out serie)) pocetSerii++;
            }
            else
            {
                break;
            }
        }
    }

    #endregion

    #region For easy copy from FSShared.cs

    public static void DeleteFile(string item)
    {
        File.Delete(item);
    }


    ///// <summary>
    ///// Vrátí cestu a název souboru s ext
    ///// </summary>
    ///// <param name="fn"></param>
    ///// <param name="path"></param>
    ///// <param name="file"></param>
    //public static void GetPathAndFileName(string fn, out string path, out string file)
    //{
    //    se.FS.GetPathAndFileName(fn, out path, out file);
    //}


    //public static string WithEndSlash(ref string v)
    //{
    //    return se.FS.WithEndSlash(ref v);
    //}

    //public static string GetDirectoryName(string rp)
    //{
    //    return se.Path.GetDirectoryName(rp);
    //}

    /// <summary>
    ///     Vrátí cestu a název souboru bez ext a ext
    ///     All returned is normal case
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    {
        path = Path.GetDirectoryName(fn) + '\\';
        file = GetFileNameWithoutExtension(fn);
        ext = Path.GetExtension(fn);
    }

    public static string PathWithoutExtension(string path)
    {
        string path2, file, ext;
        GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
        return Combine(path2, file);
    }

    public static string GetFullPath(string vr)
    {
        var result = Path.GetFullPath(vr);
        FirstCharUpper(ref result);
        return result;
    }

    public static void FileToDirectory(ref string dir)
    {
        if (!dir.EndsWith("\"")) dir = GetDirectoryName(dir);
    }

    ///// <summary>
    ///// Cant name GetAbsolutePath because The call is ambiguous between the following methods or properties: 'CAChangeContent.ChangeContent0(null,List<string>, Func<string, string, string>)' and 'CAChangeContent.ChangeContent0(null,List<string>, Func<string, string>)'
    ///// </summary>
    ///// <param name="a"></param>
    //public static string AbsoluteFromCombinePath(string a)
    //{
    //    return se.FS.AbsoluteFromCombinePath(a);
    //}

    #endregion

    #region For easy copy from FSShared64.cs

    /// <summary>
    ///     Convert to UNC path
    /// </summary>
    /// <param name="item"></param>
    public static bool ExistsDirectoryWorker(string item, bool _falseIfContainsNoFile = false)
    {
        // Not working, flags from GeoCachingTool wasnt transfered to standard
#if NETFX_CORE
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
#endif
#if WINDOWS_UWP
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
#endif

        if (item == @"\\?\" || item == string.Empty) return false;

        var item2 = MakeUncLongPath(item);

        // Directory.Exists if pass SE or only start of Unc return false
        var result = Directory.Exists(item2);
        if (_falseIfContainsNoFile)
            if (result)
            {
                var f = FSGetFiles.GetFiles(item, "*", SearchOption.AllDirectories).Count;
                result = f > 0;
            }

        return result;
    }

    public static List<string> filesWhichSurelyExists = new();


    /// <summary>
    ///     Dont check for size
    ///     Into A2 is good put true - when storage was fulled, all new files will be written with zero size. But then failing
    ///     because HtmlNode as null - empty string as input
    ///     But when file is big, like backup of DB, its better false.. Then will be avoid reading whole file to determining
    ///     their size and totally blocking HW resources on VPS
    ///     A2 must be false otherwise read file twice
    ///     Change falseIfSizeZeroOrEmpty = false. Its extremely resource intensive
    /// </summary>
    /// <param name="selectedFile"></param>
    public static
#if ASYNC
        async Task<bool>
#else
bool
#endif
        ExistsFile(string selectedFile, bool falseIfSizeZeroOrEmpty)
    {
        selectedFile = SH.FirstCharUpper(selectedFile);
        //ThrowEx.FirstLetterIsNotUpper(selectedFile);

        if (filesWhichSurelyExists.Contains(selectedFile)) return true;

        if (selectedFile == @"\\?\" || selectedFile == string.Empty) return false;

        MakeUncLongPath(ref selectedFile);

        var exists = File.Exists(selectedFile);

        if (falseIfSizeZeroOrEmpty)
        {
            if (!exists) return false;

            var ext = Path.GetExtension(selectedFile).ToLower();
            // Musím to kontrolovat jen když je to tmp, logicky
            if (ext == ".tmp") return false;

            var c = string.Empty;
            try
            {
                c =
#if ASYNC
                    await
#endif
                        File.ReadAllTextAsync(selectedFile);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The process cannot access the file")) return true;
            }

            if (c == string.Empty)
                // Měl jsem tu chybu že ač exists bylo true, File.ReadAllTextAsync selhalo protože soubor neexistoval.
                // Vyřešil jsem to kontrolou přípony, snad
                return false;
        }

        return exists;
    }


    /// <summary>
    ///     Cant return with end slash becuase is working also with files
    ///     Use this than Path.Combine which if argument starts with backslash ignore all arguments before this
    /// </summary>
    /// <param name="upFolderName"></param>
    /// <param name="dirNameDecoded"></param>
    public static string Combine(params string[] s)
    {
        //return Path.Combine(s);
        return CombineWorker(true, true, s);
    }

    public static string CombineFile(params string[] s)
    {
        return CombineWorker(true, true, s);
    }

    public static string CombineDir(params string[] s)
    {
        return CombineWorker(true, false, s);
    }

    /// <summary>
    ///     Cant return with end slash becuase is working also with files
    /// </summary>
    /// <param name="FirstCharUpper"></param>
    /// <param name="s"></param>
    private static string CombineWorker(bool FirstCharUpper, bool file, params string[] s)
    {
        for (var i = 0; i < s.Length; i++) s[i] = s[i].TrimStart('\\');
        //s = CA.TrimStartChar('\\', s.ToList()).ToArray();
        var result = Path.Combine(s);
        if (result[2] != '\\') result = result.Insert(2, "\"");
        if (FirstCharUpper)
            result = SH.FirstCharUpper(ref result);
        else
            result = SH.FirstCharUpper(ref result);
        if (!file)
            // Cant return with end slash becuase is working also with files
            WithEndSlash(ref result);
        return result;
    }


    public static long GetFolderSize(string path)
    {
        return GetFolderSize(new DirectoryInfo(path));
    }

    public static long GetFolderSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        //
        FileInfo[] fis = null;
        try
        {
            fis = d.GetFiles();
        }
        catch (DirectoryNotFoundException ex)
        {
            fis = new FileInfo[0];
            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api a zbylé složky v něm jsou junctiony které ale ztratily svůj cíl
        }

        foreach (var fi in fis) size += fi.Length;

        // Add subdirectory sizes.
        DirectoryInfo[] dis = null;
        try
        {
            dis = d.GetDirectories();
        }
        catch (DirectoryNotFoundException ex)
        {
            dis = new DirectoryInfo[0];
            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api a zbylé složky v něm jsou junctiony které ale ztratily svůj cíl
        }

        foreach (var di in dis) size += GetFolderSize(di);
        return size;
    }

    public static Dictionary<string, List<string>> GroupFilesByName(List<string> filesInSubfolders)
    {
        var result = new Dictionary<string, List<string>>();

        foreach (var item in filesInSubfolders) DictionaryHelper.AddOrCreate(result, Path.GetFileName(item), item);

        return result;
    }

    public static string BasePath(List<string> ms, string path)
    {
        foreach (var item in ms)
            if (path.Contains(item))
                return item;

        return null;
    }

    public static bool HasAnyFoldersOrFiles(string folderWhereToCreate)
    {
        return Directory.GetFiles(folderWhereToCreate).Length > 0 ||
               Directory.GetDirectories(folderWhereToCreate).Length > 0;
    }

    public static void MoveDirectoryNoRecursive(string v1, string v2, DirectoryMoveCollisionOption throwEx1, object throwEx2)
    {
        throw new NotImplementedException();
    }


    ///// <summary>
    ///// Use FirstCharUpper instead
    ///// </summary>
    ///// <param name="result"></param>
    //private static string FirstCharUpper(ref string result)
    //{
    //    return se.SH.FirstCharUpper(ref result);

    //}

    //public static bool IsWindowsPathFormat(string argValue)
    //{
    //    return se.FS.IsWindowsPathFormat(argValue);
    //}

    #endregion
}