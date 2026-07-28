namespace SunamoFileSystem;

using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

public partial class FS
{
    public const string DEndsWithReplaceInFile = "SubdomainHelperSimple.cs";
    protected static readonly List<char> invalidFileNameCharsReadonly = Path.GetInvalidFileNameChars().ToList();
    protected static readonly List<string> invalidFileNameStringsReadonly;
    public static bool IsAbsolutePath(string path)
    {
        return !String.IsNullOrWhiteSpace(path)
            && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1
            && Path.IsPathRooted(path)
            && !Path.GetPathRoot(path)!.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
    }
    public static void CopyFolder(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);
        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
        foreach (var directory in Directory.GetDirectories(sourceDir))
            CopyFolder(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
    }
    protected static List<char> invalidPathChars;
    public static string InvalidFileNameCharsString;
    public static List<char> InvalidFileNameChars;
    protected static List<char> invalidCharsForMapPath;
    protected static List<char> invalidFileNameCharsWithoutDelimiterOfFolders;
    public static string ReplaceIncorrectFor = string.Empty;
    public static Action<string>? DeleteFileMaybeLocked;
    public static Func<string, bool, List<Process>>? FileUtilWhoIsLocking = null;
    static FS()
    {
        invalidFileNameStringsReadonly = new List<string>(invalidFileNameCharsReadonly.Count);
        foreach (var item in invalidFileNameCharsReadonly) invalidFileNameStringsReadonly.Add(item.ToString());
        invalidPathChars = new List<char>(Path.GetInvalidPathChars());
        if (!invalidPathChars.Contains('/')) invalidPathChars.Add('/');
        if (!invalidPathChars.Contains('\\')) invalidPathChars.Add('\\');
        InvalidFileNameChars = new List<char>(invalidFileNameCharsReadonly);
        InvalidFileNameCharsString = string.Join("", invalidFileNameCharsReadonly);
        for (var i = (char)65529; i < 65534; i++) InvalidFileNameChars.Add(i);
        invalidCharsForMapPath = new List<char>();
        invalidCharsForMapPath.AddRange(InvalidFileNameChars.ToArray());
        foreach (var item in invalidFileNameCharsReadonly)
            if (!invalidCharsForMapPath.Contains(item))
                invalidCharsForMapPath.Add(item);
        invalidCharsForMapPath.Remove('/');
        invalidFileNameCharsWithoutDelimiterOfFolders = new List<char>(InvalidFileNameChars.ToArray());
        invalidFileNameCharsWithoutDelimiterOfFolders.Remove('\\');
        invalidFileNameCharsWithoutDelimiterOfFolders.Remove('/');
    }
    public static void CreateUpfoldersPsysicallyUnlessThere(string path)
    {
        CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(path)!);
    }
    public static bool ExistsDirectory(string path)
    {
        return Directory.Exists(path);
    }
    public static void CreateFoldersPsysicallyUnlessThere(string path)
    {
        ThrowEx.IsNullOrEmpty("path", path);
        if (Directory.Exists(path)) return;
        var foldersToCreate = new List<string>
        {
            path
        };
        while (true)
        {
            path = Path.GetDirectoryName(path)!;
            // TODO: This doesn't work for UWP/UAP apps because they don't have access to entire disk
            if (Directory.Exists(path)) break;
            foldersToCreate.Add(path!);
        }
        foldersToCreate.Reverse();
        foreach (var item in foldersToCreate)
        {
            if (!Directory.Exists(item)) Directory.CreateDirectory(item);
        }
    }

    public static void CreateDirectory(string value)
    {
        try
        {
            Directory.CreateDirectory(value);
        }
        catch (NotSupportedException)
        {
        }
    }
    public static void CreateDirectoryIfNotExists(string path)
    {
        MakeUncLongPath(ref path);
        if (!ExistsDirectory(path)) Directory.CreateDirectory(path);
    }
    public static string WithEndSlash(string value)
    {
        return WithEndSlash(ref value);
    }

    public static string WithEndSlash(ref string value)
    {
        if (value != string.Empty) value = value.TrimEnd('\\') + '\\';
        FirstCharUpper(ref value);
        return value;
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
    public static string FirstCharUpper(string text)
    {
        if (text.Length == 1) return text.ToUpper();
        var substring = text.Substring(1);
        return text[0].ToString().ToUpper() + substring;
    }
    public static bool TryDeleteFile(string filePath)
    {
        // TODO: To all code message logging as here
        try
        {
            // If file won't exists, wont throw any exception
            File.Delete(filePath);
            return true;
        }
        catch
        {
            //ThisApp.Error(Translate.FromKey(XlfKeys.FileCanTBeDeleted) + ": " + filePath);
            return false;
        }
    }
    public static async Task WriteAllTextWithExc(string file, string content)
    {
        try
        {
            await FileAsync.WriteAllTextAsync(file, content);
        }
        catch (Exception)
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
    //    await FileAsync.WriteAllBytesAsync(path.ToString(), new Byte[] { });
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
        var element = GetExtension(origS);
        if (origS.Contains('/') || origS.Contains('\\'))
        {
            var path = Path.GetDirectoryName(origS);
            return Path.Combine(path!, fn + whatInsert + element);
        }
        return fn + whatInsert + element;
    }
    public static string ReplaceInvalidFileNameChars(string filename, params char[] characters)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in filename)
            if (!InvalidFileNameChars.Contains(item) || characters.Contains(item))
                stringBuilder.Append(item);
        return stringBuilder.ToString();
    }





    //public static StorageFile InsertBetweenFileNameAndExtension<StorageFolder, StorageFile>(StorageFile orig, string whatInsert, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    // Cesta by se zde hodila kvůli FS.CiStorageFile
    //    // nicméně StorageFolder nevím zda se používá, takže to bude umět i bez toho
    //    var origS = orig.ToString();
    //    string fn = Path.GetFileNameWithoutExtension(origS);
    //    string element = GetExtension(origS);
    //    if (origS.Contains('/') || origS.Contains('\\'))
    //    {
    //        string path = Path.GetDirectoryName(origS);
    //        return CiStorageFile<StorageFolder, StorageFile>(Path.Combine(path, fn + whatInsert + element), ac);
    //    }
    //    return CiStorageFile<StorageFolder, StorageFile>(fn + whatInsert + element, ac);
    //}
    public static string GetExtension(string value, GetExtensionArgs? args = null)
    {
        if (args == null) args = new GetExtensionArgs();
        var result = "";
        var lastDot = value.LastIndexOf('.');
        if (lastDot == -1) return string.Empty;
        var lastSlash = value.LastIndexOf('/');
        var lastBs = value.LastIndexOf('\\');
        if (lastSlash > lastDot) return string.Empty;
        if (lastBs > lastDot) return string.Empty;
        result = value.Substring(lastDot);
        if (!IsExtension(result))
        {
            if (args.FilesWithoutExtensionReturnAsIs) return result;
            return string.Empty;
        }
        if (!args.ReturnOriginalCase) result = result.ToLower();
        return result;
    }
    public static bool IsExtension(string result)
    {
        if (string.IsNullOrWhiteSpace(result)) return false;
        if (!result.TrimStart('.').ToLower()
                .All(character => (char.IsLetter(character) && char.IsLower(character)) || char.IsDigit(character))) return false;
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
    public static bool ExistsFile(string path)
    {
        return File.Exists(path);
    }
    public static void MoveSubfoldersToFolder(ILogger logger, List<string> subfolderNames, string from, string to,
        FileMoveCollisionOption fo)
    {
        foreach (var item in subfolderNames)
        {
            var sourcePath = Path.Combine(from, item);
            var temp = Path.Combine(to, item);
            MoveAllRecursivelyAndThenDirectory(logger, sourcePath, temp, fo);
        }
    }
    public static void TrimBasePathAndTrailingBs(List<string> text, string basePath)
    {
        for (var i = 0; i < text.Count; i++)
        {
            text[i] = text[i].Substring(basePath.Length);
            text[i] = text[i].TrimEnd('\\');
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
        async Task<List<string>>
        KeepOnlyWhichIsNotInFiles(List<string> opts, List<string> paths)
    {
        var count = new CollectionWithoutDuplicates<string>();
        foreach (var item in paths)
            count.AddRange(SHGetLines.GetLines(
                await FileAsync.ReadAllTextAsync(item)
                ).ToList());
        CAG.CompareList(opts, count.c);
        return opts;
    }
    public static string RelativeToAbsolutePath(string fullPathToSecondFile, string relativePath)
    {
        var fullPathToFirstFile =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(fullPathToSecondFile)!, relativePath));
        return fullPathToFirstFile;
    }
    // Proč to volám zde? Má se to volat value aplikacích kde to potřebuji
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
    public static string AbsoluteFromCombinePath(string path)
    {
        var result = Path.GetFullPath(new Uri(path).LocalPath);
        return result;
    }
    public static string WrapWithQm(string text, bool? forceNotIncludeQm)
    {
        if (text.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return SH.WrapWithQm(text);
        return text;
    }
    public static List<string> FilterInRootAndInSubFolder(string rootFolder, List<string> files)
    {
        WithEndSlash(ref rootFolder);
        var count = rootFolder.Length;
        var subFolder = new List<string>(files.Count);
        for (var i = files.Count - 1; i >= 0; i--)
        {
            var item = files[i];
            if (item.Substring(count).Contains("\""))
            {
                subFolder.Add(item);
                files.RemoveAt(i);
            }
        }
        return subFolder;
    }
    public static void OnlyNames(List<string> subfolders)
    {
        for (var i = 0; i < subfolders.Count; i++) subfolders[i] = Path.GetFileName(subfolders[i]);
    }
    public static List<string> FilesWhichContainsAll(object sunamo, string searchPattern, params string[] requiredContents)
    {
        return FilesWhichContainsAll(sunamo, searchPattern, requiredContents);
    }
    public static string PathSpecialAndLevel(string basePath, string relativePath, int value)
    {
        basePath = basePath.Trim('\\');
        relativePath = relativePath.Trim('\\');
        relativePath = relativePath.Replace(basePath, string.Empty);
        var pBasePath = SHSplit.Split(basePath, "\"");
        var basePathC = pBasePath.Count;
        var path = SHSplit.Split(relativePath, "\"");
        var i = 0;
        for (; i < path.Count; i++)
            if (path[i].StartsWith("_"))
                pBasePath.Add(path[i]);
            else
                //i--;
                break;
        for (var yValue = 0; yValue < i; yValue++) path.RemoveAt(0);
        var remainingSegments = path.Count - i + basePathC;
        var to = Math.Min(value, remainingSegments);
        i = 0;
        for (; i < to; i++) pBasePath.Add(path[i]);
        return string.Join("\"", pBasePath);
    }
    public static string GetDirectoryNameIfIsFile(string path)
    {
        if (File.Exists(path)) return Path.GetDirectoryName(path)!;
        return path;
    }
    public static string MaskFromExtensions(List<string> allExtensions)
    {
        for (var i = 0; i < allExtensions.Count; i++) allExtensions[i] = "*" + allExtensions[i];
        //CA.Prepend("*", allExtensions);
        return string.Join(",", allExtensions);
    }
    //public static string GetRelativePath(string relativeTo, string path)
    //{
    //    return SunamoExceptions.PathPolyfill.GetRelativePath(relativeTo, path);
    //}
    //public static bool IsAbsolutePath(string path)
    //{
    //    return SunamoExceptions.FS.IsAbsolutePath(path);
    //}
    public static void RenameNumberedSerieFiles(ILogger logger, List<string> data, string path, int startFrom, string ext)
    {
        var searchPattern = MascFromExtension(ext);
        var files = FSGetFiles.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        RenameNumberedSerieFiles(logger, data, files, startFrom, ext);
    }
    public static void RenameNumberedSerieFiles(ILogger logger, List<string> data, List<string> files, int startFrom, string ext)
    {
        var path = Path.GetDirectoryName(files[0]);
        if (files.Count >= data.Count)
        {
            var filesCountMinusOne = files.Count - 1;
            //var result = files.First();
            for (var i = startFrom; ; i++)
            {
                if (filesCountMinusOne < i) break;
                var result = files[i];
                var numberedFilePath = path + i + ext;
                if (files.Contains(numberedFilePath))
                    //break;
                    continue;
                // AddSerie is useless coz file never will be exists
                //FS.RenameFile(numberedFilePath, data[i - startFrom] + ext, FileMoveCollisionOption.AddSerie);
                RenameFile(logger, result, numberedFilePath, FileMoveCollisionOption.AddSerie);
            }
        }
    }
    public static string PlaceInFolder(string sourcePath, string targetFolder)
    {
        //return Slozka.ci.PridejNadslozku(sourcePath, targetFolder);
        var parentPath = Path.GetDirectoryName(sourcePath);
        var parentFolderName = Path.GetFileName(parentPath);
        return Path.Combine(targetFolder, Path.Combine(parentFolderName!, Path.GetFileName(sourcePath)));
    }
    public static void CopyMoveFilesInList(ILogger logger, List<string> filesFrom, string folderFrom, string folderTo,
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
                    CopyFile(logger, oldPath, newPath, FileMoveCollisionOption.Overwrite);
                else
                    MoveFile(logger, oldPath, newPath, FileMoveCollisionOption.Overwrite);
            }
            filesFrom.RemoveAt(i);
        }
    }
    public static void CopyMoveFilesInListSimple(ILogger logger, List<string> files, string basePathCjHtml1, string basePathCjHtml2,
        bool copy, bool overwrite = true)
    {
        List<string>? wasntExistsInFrom = null;
        var mustExistsInTarget = false;
        CopyMoveFilesInList(logger, files, basePathCjHtml1, basePathCjHtml2, wasntExistsInFrom!, mustExistsInTarget, copy, null!,
            overwrite);
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
    public static void CopyMoveFromMultiLocationIntoOne(ILogger logger, List<string> files, string folderFrom, string folderTo)
    {
        var wasntExists = new List<string>();
        var files2 = new Dictionary<string, List<string>>();
        var getFiles = FSGetFiles.GetFiles(folderFrom, "*.cs", SearchOption.AllDirectories,
            new GetFilesArgsFS { ExcludeFromLocationsContains = new List<string>(["TestFiles"]) });
        foreach (var item in files) files2.Add(item, getFiles.Where(data => Path.GetFileName(data) == item).ToList());
        CopyMoveFilesInList(logger, files, folderFrom, folderTo, wasntExists, false, true, files2);
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
    private static Stream OpenStream(string value)
    {
        return new FileStream(value, FileMode.OpenOrCreate);
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

    //public static StorageFile GetStorageFile<StorageFolder, StorageFile>(StorageFolder folder, string value, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ((dynamic)ac.fs.getStorageFile(folder, value)).Path;
    //    }
    //    return (dynamic)Path.Combine(folder.ToString(), value);
    //}
    public static
        async Task
        DeleteEmptyFiles(string folder, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(folder, "*.*", so);
        foreach (var item in files)
        {
            var fileSize = new FileInfo(item).Length;
            if (fileSize == 0)
                TryDeleteFile(item);
            else if (fileSize < 4)
                if ((
                        await FileAsync.ReadAllTextAsync(item)
                        ).Trim() == string.Empty)
                    TryDeleteFile(item);
        }
    }
    private static async Task ReplaceInAllFilesWorker(object args, Func<string, bool> EncodingHelperIsBinary)
    {
        var path = (ReplaceInAllFilesArgs)args;
        #region ReplaceInAllFilesArgsBase - Zkopírovat i do ReplaceInAllFilesWorker. Viz comment níže
        // musím to rozdělit na jednotlivé proměnné abych viděl co se používá a co ne. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
        var fasterMethodForReplacing = path.FasterMethodForReplacing;
        var files = path.Files;
        var inDownloadedFolders = path.InDownloadedFolders;
        var inFoldersToDelete = path.InFoldersToDelete;
        var inGitFiles = path.InGitFiles;
        var isMultilineWithVariousIndent = path.IsMultilineWithVariousIndent;
        var writeEveryReadedFileAsStatus = path.WriteEveryReadedFileAsStatus;
        var writeEveryWrittenFileAsStatus = path.WriteEveryWrittenFileAsStatus;
        #endregion
        #region ReplaceInAllFilesArgs
        var from = path.From;
        var to = path.To;
        var pairLinesInFromAndTo = path.PairLinesInFromAndTo;
        var replaceWithEmpty = path.ReplaceWithEmpty;
        var isNotReplaceInTemporaryFiles = path.IsNotReplaceInTemporaryFiles;
        #endregion
        if (isMultilineWithVariousIndent)
        {
            from = SHReplace.ReplaceAllDoubleSpaceToSingle2(from);
            to = SHReplace.ReplaceAllDoubleSpaceToSingle2(to);
        }
        if (pairLinesInFromAndTo)
        {
            var from2 = SHSplit.Split(from, Environment.NewLine);
            var to2 = SHSplit.Split(to, Environment.NewLine);
            if (replaceWithEmpty)
            {
                to2.Clear();
                foreach (var item in from2) to2.Add(string.Empty);
            }
            ThrowEx.DifferentCountInLists("from2", from2, "to2", to2);
            await ReplaceInAllFiles(from2, to2, (args as ReplaceInAllFilesArgsBase)!, EncodingHelperIsBinary);
        }
        else
        {
            await ReplaceInAllFiles(new List<string>([from]), new List<string>([to])
                , (args as ReplaceInAllFilesArgsBase)!, EncodingHelperIsBinary);
        }
    }
    public static async Task ReplaceInAllFiles(string from, string to, ReplaceInAllFilesArgsBase args,
        Func<string, bool> EncodingHelperIsBinary)
    {
        var result = new ReplaceInAllFilesArgs(args);
        result.From = from;
        result.To = to;
        await ReplaceInAllFilesWorker(result, EncodingHelperIsBinary);
        //Thread temp = new Thread(new ParameterizedThreadStart(ReplaceInAllFilesWorker));
        //temp.Start(result);
    }
    public static async Task ReplaceInAllFiles(string folder, string extension, List<string> replaceFrom,
        List<string> replaceTo, bool isMultilineWithVariousIndent, Func<string, bool> EncodingHelperIsBinary)
    {
        var files = FSGetFiles.GetFiles(folder, MascFromExtension(extension), SearchOption.AllDirectories);
        ThrowEx.DifferentCountInLists("replaceFrom", replaceFrom, "replaceTo", replaceTo);
        Func<StringBuilder, IList<string>, IList<string>, StringBuilder>? fasterMethodForReplacing = null;
        await ReplaceInAllFiles(replaceFrom, replaceTo,
            new ReplaceInAllFilesArgsBase
            {
                Files = files,
                IsMultilineWithVariousIndent = isMultilineWithVariousIndent,
                FasterMethodForReplacing = fasterMethodForReplacing
            }, EncodingHelperIsBinary);
    }
    public static
        async Task
        ReplaceInAllFiles(IList<string> replaceFrom, IList<string> replaceTo, ReplaceInAllFilesArgsBase args,
            Func<string, bool> EncodingHelperIsBinary)
    {
        #region ReplaceInAllFilesArgsBase - Zkopírovat i do ReplaceInAllFilesWorker. Viz comment níže
        // musím to rozdělit na jednotlivé proměnné abych viděl co se používá a co ne. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
        var fasterMethodForReplacing = args.FasterMethodForReplacing;
        var files = args.Files;
        var inDownloadedFolders = args.InDownloadedFolders;
        var inFoldersToDelete = args.InFoldersToDelete;
        var inGitFiles = args.InGitFiles;
        var isMultilineWithVariousIndent = args.IsMultilineWithVariousIndent;
        var writeEveryReadedFileAsStatus = args.WriteEveryReadedFileAsStatus;
        var writeEveryWrittenFileAsStatus = args.WriteEveryWrittenFileAsStatus;
        var dRemoveGitFiles = args.DRemoveGitFiles;
        #endregion
        if (!inGitFiles || !inFoldersToDelete || !inDownloadedFolders)
            dRemoveGitFiles!(files, inGitFiles, inDownloadedFolders, inFoldersToDelete);
        foreach (var item in files)
        {
            if (!EncodingHelperIsBinary(item))
            {
                if (writeEveryReadedFileAsStatus)
                {
                    //SunamoTemplateLogger.Instance.LoadedFromStorage(item);
                }
                // File.ReadAllText is 20x faster than File.ReadAllText
                var content =
                    await FileAsync.ReadAllTextAsync(item);
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
                    await FileAsync.WriteAllTextAsync(item, content2);
                    if (writeEveryReadedFileAsStatus)
                    {
                        //SunamoTemplateLogger.Instance.SavedToDrive(item);
                    }
                }
            }
            //ThisApp.Warning(Translate.FromKey(XlfKeys.ContentOf) + " " + item + " couldn't be replaced - contains control chars.");
        }
    }

    public static string GetFileInStartupPath(string value)
    {
        return AppPaths.GetFileInStartupPath(value);
    }
    public static
        async Task
        RemoveDiacriticInFileContents(string folder, string mask)
    {
        var files = FSGetFiles.GetFiles(folder, mask, SearchOption.AllDirectories);
        foreach (var item in files)
        {
            var df2 =
                await FileAsync.ReadAllTextAsync(item, Encoding.Default);
            if (true) //SH.ContainsDiacritic(df2))
            {
                await FileAsync.WriteAllTextAsync(item, df2.RemoveDiacritics());
                df2 = SHReplace.ReplaceOnce(df2, "\u010F\u00BB\u017C", "");
                await FileAsync.WriteAllTextAsync(item, df2);
            }
        }
    }
    //public static List<string> PathsOfStorageFiles<StorageFolder, StorageFile>(IList<StorageFile> files1, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    List<string> data = new List<string>(files1.Count());
    //    foreach (var item in files1)
    //    {
    //        data.Add(FS.StorageFilePath(item, ac));
    //    }
    //    return data;
    //}
    public static string RemoveFile(string fullPathCsproj)
    {
        // Most effecient way to handle csproj and dir
        var ext = Path.GetExtension(fullPathCsproj);
        if (ext != string.Empty) fullPathCsproj = Path.GetDirectoryName(fullPathCsproj)!;
        var result = WithoutEndSlash(fullPathCsproj);
        SH.FirstCharUpper(ref result);
        return result;
    }
    public static string MakeFromLastPartFile(string fullPath, string ext)
    {
        WithoutEndSlash(ref fullPath);
        return fullPath + ext;
    }
    public static string GetFileNameWithoutExtensions(string path)
    {
        while (Path.HasExtension(path)) path = Path.GetFileNameWithoutExtension(path);
        return path;
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
        // Originally - 265 chars, 254 also too long: element:\Documents\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\Facebook.cs
        // 4+265 - OK: @"\\?\D:\_NewlyRecovered\Visual Studio 2020\Projects\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\Facebook.cs"
        // 216 - OK: data:\Recovered data 03-23 12_11_44012345678901234567890123456\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\
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
    public static void CopyFilesOfExtensions(string folderFrom, string folderTo, params string[] extensions)
    {
        folderFrom = WithEndSlash(folderFrom);
        folderTo = WithEndSlash(folderTo);
        var filesOfExtension = FSGetFiles.FilesOfExtensions(folderFrom, extensions);
        foreach (var item in filesOfExtension)
            foreach (var path in item.Value)
            {
                var newPath = path.Replace(folderFrom, folderTo);
                CreateUpfoldersPsysicallyUnlessThere(newPath);
                File.Copy(path, newPath);
            }
    }
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
                var newpath = Path.Combine(directory!, filename);
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
                string? newpath = null;
                try
                {
                    newpath = Path.Combine(directory!, filename);
                }
                catch (Exception ex)
                {
                    ThrowEx.Custom(ex);
                    File.Delete(item);
                    continue;
                }
                var realNewPath = newpath;
                var insertedCount = 0;
                while (File.Exists(realNewPath))
                {
                    realNewPath = InsertBetweenFileNameAndExtension(newpath, insertedCount.ToString());
                    insertedCount++;
                }
                File.Move(item, realNewPath);
            }
        }
    }
    public static string? GetUpFolderWhichContainsExtension(string path, string fileExt)
    {
        while (FSGetFiles.FilesOfExtension(path!, fileExt).Count == 0)
        {
            if (path.Length < 4) return null;
            path = Path.GetDirectoryName(path)!;
        }
        return path;
    }
    public static void TrimContentInFilesOfFolder(string folder, string searchPattern, SearchOption searchOption)
    {
        var files = FSGetFiles.GetFiles(folder, searchPattern, searchOption);
        foreach (var item in files)
        {
            var fileStream = new FileStream(item, FileMode.Open);
            var streamReader = new StreamReader(fileStream, true);
            var content = streamReader.ReadToEnd();
            var encoding = streamReader.CurrentEncoding;
            //streamReader.Close();
            streamReader.Dispose();
            streamReader = null;
            var contentTrim = content.Trim();
            File.WriteAllText(item, contentTrim, encoding);
            //}
        }
    }
    public static string ReplaceInFileName(string oldPath, string what, string forWhat)
    {
        string path, fileName;
        GetPathAndFileName(oldPath, out path, out fileName);
        var result = path + "\"" + fileName.Replace(what, forWhat);
        SH.FirstCharUpper(ref result);
        return result;
    }
    public static long GetSizeIn(long value, ComputerSizeUnits fromUnit, ComputerSizeUnits to)
    {
        if (fromUnit == to) return value;
        var toLarger = (byte)fromUnit < (byte)to;
        if (toLarger)
        {
            value = ConvertToSmallerComputerUnitSize(value, fromUnit, ComputerSizeUnits.B);
            if (to == ComputerSizeUnits.Auto)
                throw new Exception(
                    "Output ComputerSizeUnit was specified, cannot change this setting");
            if (to == ComputerSizeUnits.KB && fromUnit != ComputerSizeUnits.KB)
                value /= 1024;
            else if (to == ComputerSizeUnits.MB && fromUnit != ComputerSizeUnits.MB)
                value /= 1024 * 1024;
            else if (to == ComputerSizeUnits.GB && fromUnit != ComputerSizeUnits.GB)
                value /= 1024 * 1024 * 1024;
            else if (to == ComputerSizeUnits.TB && fromUnit != ComputerSizeUnits.TB) value /= 1024L * 1024L * 1024L * 1024L;
        }
        else
        {
            value = ConvertToSmallerComputerUnitSize(value, fromUnit, to);
        }
        return value;
    }
    public static void DeleteAllEmptyDirectories(string value/*, bool deleteAlsoA1*/, params string[] excludePatterns)
    {
        var dirs = DirectoriesWithToken(value, AscDesc.Desc);
        foreach (var item in dirs)
            if (IsDirectoryEmpty(item.Value, true, true))
            {
                if (excludePatterns.Length > 0)
                {
                    if (!excludePatterns.Any(data =>
                            item.Value.Contains(data))) //CANewSH.ContainsAnyFromArray(item.Value, excludePatterns))
                        TryDeleteDirectory(item.Value);
                }
                else
                {
                    TryDeleteDirectory(item.Value);
                }
            }
        if (IsDirectoryEmpty(value, true, true) && !excludePatterns.Any()) TryDeleteDirectory(value);
    }
    //private static List<TWithInt<string>> DirectoriesWithToken(string value, AscDesc desc)
    //{
    //    ThrowEx.NotImplementedMethod();
    //}
    public static int CompareTWithInt<T>(TWithInt<T> first, TWithInt<T> second)
    {
        if (first.Count > second.Count)
            return 1;
        if (first.Count < second.Count) return -1;
        return 0;
    }
    public static List<TWithInt<string>> DirectoriesWithToken(string value, AscDesc sortOrder)
    {
        var dirs = Directory.GetDirectories(value, "*", SearchOption.AllDirectories);
        var result = new List<TWithInt<string>>();
        foreach (var item in dirs)
            result.Add(new TWithInt<string>
            {
                Value = item,
                Count = SH.OccurencesOfStringIn(item, "\"")
            });
        result.Sort(CompareTWithInt);
        if (sortOrder == AscDesc.Desc) result.Reverse();
        //result.Sort(new SunamoComparerICompare.TWithIntComparer.Asc<string>(new SunamoComparer.TWithIntSunamoComparer<string>()));
        //else if (sortOrder == AscDesc.Desc)
        //{
        //    result.Sort(new SunamoComparerICompare.TWithIntComparer.Desc<string>(new SunamoComparer.TWithIntSunamoComparer<string>()));
        //}
        return result;
    }
    public static string MoveDirectoryNoRecursive(ILogger logger, string from, string to, DirectoryMoveCollisionOption directoryMoveCollisionOption,
        FileMoveCollisionOption fileMoveCollisionOption)
    {
        string? resultMessage = null;
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
                        resultMessage = Translate.FromKey(XlfKeys.FolderHasBeenRenamedTo) + " " + Path.GetFileName(newFn);
                        to = newFn;
                        break;
                    }
                    serie++;
                }
            }
            else if (directoryMoveCollisionOption == DirectoryMoveCollisionOption.DiscardFrom)
            {
                Directory.Delete(from, true);
                return resultMessage!;
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
            MoveFile(logger, item2, fileTo, fileMoveCollisionOption);
        }
        try
        {
            Directory.Move(from, to);
        }
        catch (Exception)
        {
            //ThrowEx.CannotMoveFolder(item, nova, ex);
        }
        DeleteAllEmptyDirectories(from);
        return resultMessage!;
    }
    private static bool IsDirectoryEmpty(string directoryPath, bool folders, bool files)
    {
        var itemCount = 0;
        if (folders) itemCount += Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly).Length;
        if (files) itemCount += FSGetFiles.GetFiles(directoryPath, "*", SearchOption.TopDirectoryOnly).Count;
        return itemCount == 0;
    }
    public static void MoveAllRecursivelyAndThenDirectory(ILogger logger, string sourcePath, string targetPath, FileMoveCollisionOption collisionOption)
    {
        CopyMoveAllFilesRecursively(logger, sourcePath, targetPath, collisionOption, true, null!, SearchOption.AllDirectories);
        var dirs = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
        for (var i = dirs.Length - 1; i >= 0; i--) TryDeleteDirectory(dirs[i]);
        TryDeleteDirectory(sourcePath);
    }
    [Obsolete("Use MoveDirectoryNoRecursive instead")]
    public static void MoveAllFilesRecursively(ILogger logger, string sourcePath, string targetPath, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, sourcePath, targetPath, collisionOption, true, contains!, SearchOption.AllDirectories);
    }
    public static void DeleteFilesWithSameContentBytes(List<string> files)
    {
        DeleteFilesWithSameContentWorking<List<byte>, byte>(files, TF.ReadAllBytesSync);
    }
    public static void DeleteDuplicatedImages(List<string> files)
    {
        throw new Exception(Translate.FromKey(XlfKeys.OnlyForTestFilesForAnotherApps) + ". ");
    }
    public static void DeleteFilesWithSameContentWorking<TContent, ColType>(List<string> files, Func<string, TContent> readFunc) where TContent : notnull
    {
        var dictionary = new Dictionary<string, TContent>(files.Count);
        foreach (var item in files) dictionary.Add(item, readFunc.Invoke(item));
        var sameContent = DictionaryHelper.GroupByValues<string, TContent, ColType>(dictionary);
        foreach (var item in sameContent)
            if (item.Value.Count > 1)
            {
                item.Value.RemoveAt(0);
                item.Value.ForEach(data => File.Delete(data));
            }
    }
    public static void DeleteFilesWithSameContent(List<string> files)
    {
        DeleteFilesWithSameContentWorking<string, object>(files, File.ReadAllText);
    }
    public static List<string> OrderByNaturalNumberSerie(List<string> list)
    {
        var filenames = new List<Tuple<string, int, string>>();
        var dontHaveNumbersOnBeginning = new List<string>();
        for (var i = list.Count - 1; i >= 0; i--)
        {
            var backup = list[i];
            var pathParts = SHSplit.SplitToPartsFromEnd(list[i], 2, '\\');
            string path;
            if (pathParts.Count == 1)
            {
                path = string.Empty;
            }
            else
            {
                path = pathParts[0];
                list[i] = pathParts[1];
            }
            var fn = list[i];
            //var (sh, fnNew) = NH.NumberIntUntilWontReachOtherChar(fn);
            var sh = int.Parse(Regex.Match(fn, @"\d+").Value);
            var fnNew = fn.Replace(sh.ToString(), string.Empty);
            fn = fnNew;
            if (sh == int.MaxValue)
                dontHaveNumbersOnBeginning.Add(backup);
            else
                filenames.Add(new Tuple<string, int, string>(path, sh, fn));
        }
        var sorted = filenames.OrderBy(data => data.Item2);
        var result = new List<string>(list.Count);
        foreach (var item in sorted) result.Add(Path.Combine(item.Item1, item.Item2 + item.Item3));
        result.AddRange(dontHaveNumbersOnBeginning);
        return result;
    }
    public static Dictionary<string, List<string>> SortPathsByFileName(List<string> allCsFilesInFolder,
        bool onlyOneExtension)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var item in allCsFilesInFolder)
        {
            string? fileName = null;
            if (onlyOneExtension)
                fileName = Path.GetFileNameWithoutExtension(item);
            else
                fileName = Path.GetFileName(item);
            DictionaryHelper.AddOrCreate(result, fileName, item);
        }
        return result;
    }
    public static void DeleteAllRecursively(string path, bool rootDirectoryToo = false)
    {
        if (Directory.Exists(path))
        {
            var files = FSGetFiles.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var item in files) TryDeleteFile(item);
            var dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            for (var i = dirs.Length - 1; i >= 0; i--) TryDeleteDirectory(dirs[i]);
            if (rootDirectoryToo) TryDeleteDirectory(path);
            // Commented due to NI
            FS.DeleteFoldersWhichNotContains(@"E:\", "bin", new List<string>(["node_modules"]));
        }
    }
    public static void DeleteFoldersWhichNotContains(string value, string folder, IList<string> excludedContainingTexts)
    {
        var folders = Directory.GetDirectories(value, folder, SearchOption.AllDirectories).ToList();
        for (int i = folders.Count - 1; i >= 0; i--)
        {
            if (CA.ReturnWhichContainsIndexes(folders[i], excludedContainingTexts).Count != 0)
            {
                folders.RemoveAt(i);
            }
        }
        foreach (var item in folders)
        {
            //FS.DeleteF
        }
    }
    public static void DeleteAllRecursivelyAndThenDirectory(string path)
    {
        DeleteAllRecursively(path, true);
    }
    public static List<string> OnlyExtensions(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        //CA.InitFillWith(result, paths.Count);
        for (var i = 0; i < result.Count; i++) result[i] = Path.GetExtension(paths[i]);
        return result;
    }
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
    public static List<string> OnlyExtensionsToLower(List<string> paths, GetExtensionArgs? args = null)
    {
        if (args == null) args = new GetExtensionArgs();
        args.ReturnOriginalCase = false;
        var result = new List<string>(paths.Count);
        CA.InitFillWith(result, paths.Count);
        for (var i = 0; i < result.Count; i++)
            result[i] = Path.GetExtension(paths[i]).ToLower();
        return result;
    }
    public static List<string> OnlyExtensionsToLowerWithPath(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        //CA.InitFillWith(result, paths.Count);
        for (var i = 0; i < result.Count; i++) result[i] = OnlyExtensionToLowerWithPath(paths[i]);
        return result;
    }
    public static string OnlyExtensionToLowerWithPath(string data)
    {
        string path, fn, ext;
        GetPathAndFileName(data, out path, out fn, out ext);
        var result = path + fn + ext.ToLower();
        return result;
    }
    public static List<string> AllExtensionsInFolders(SearchOption so, params string[] folders)
    {
        ThrowEx.NoPassedFolders(folders);
        List<string> filesFull = FSGetFiles.AllFilesInFolders(folders.ToList(), new List<string>(["*"]), so);
        return AllExtensionsInFolders(filesFull);
    }


    public static List<string> AllExtensionsInFolders(List<string> filesFull, GetExtensionArgs? args = null)
    {
        var result = new List<string>();
        var files = new List<string>(OnlyExtensionsToLower(filesFull, args));
        foreach (var item in files)
            if (!result.Contains(item))
                result.Add(item);
        return result;
    }
    public static string ExpandEnvironmentVariables(EnvironmentVariables environmentVariable)
    {
        return Environment.ExpandEnvironmentVariables(SH.WrapWith(environmentVariable.ToString(), "%"));
    }
    public static string GetFileNameWithoutExtensionLower(string text)
    {
        return GetFileNameWithoutExtension(text).ToLower();
    }
    public static string AddUpfoldersToRelativePath(int i2, string file, char delimiter)
    {
        var jumpUp = ".." + delimiter;
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < i2; i++) stringBuilder.Append(jumpUp);
        stringBuilder.Append(file);
        return stringBuilder.ToString();
        //return SHJoin.JoinTimes(i, jumpUp) + file;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension(string extension)
    {
        return "." + extension.TrimStart('.');
    }
    public static string GetNormalizedExtension(string filename)
    {
        return NormalizeExtension(filename);
    }
    public static long ModifiedinUnix(string filePath)
    {
        return (long)File.GetLastWriteTimeUtc(filePath).Subtract(DTConstants.UnixFsStart).TotalSeconds;
    }
    public static void ReplaceDiacriticRecursive(ILogger logger, string folder, bool dirs, bool files, DirectoryMoveCollisionOption directoryCollisionOption,
        FileMoveCollisionOption fileCollisionOption)
    {
        if (dirs)
        {
            var dires = DirectoriesWithToken(folder, AscDesc.Desc);
            foreach (var item in dires)
            {
                var dirPath = WithoutEndSlash(item.Value);
                var dirName = Path.GetFileName(dirPath);
                if (dirName.HasDiacritics())
                {
                    var dirNameWithoutDiac = dirName.RemoveDiacritics(); //SH.TextWithoutDiacritic(dirName);
                    RenameDirectory(logger, item.Value, dirNameWithoutDiac, directoryCollisionOption, fileCollisionOption);
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
                    RenameFile(logger, item, dirNameWithoutDiac, fileCollisionOption);
                }
            }
        }
    }
    public static void RenameFile(ILogger logger, string oldPath, string newFileNameWithoutPath, FileMoveCollisionOption collisionOption)
    {
        var to = ChangeFilename(oldPath, newFileNameWithoutPath, false);
        MoveFile(logger, oldPath, to, collisionOption);
    }
    public static string RenameDirectory(ILogger logger, string path, string newname, DirectoryMoveCollisionOption directoryCollisionOption,
        FileMoveCollisionOption fileCollisionOption)
    {
        string? resultMessage = null;
        path = WithoutEndSlash(path);
        var parentDirectory = Path.GetDirectoryName(path);
        var newPath = Path.Combine(parentDirectory!, newname);
        resultMessage = MoveDirectoryNoRecursive(logger, path, newPath, directoryCollisionOption, fileCollisionOption);
        return resultMessage;
    }
    public static void NormalizeExtensions(List<string> extension)
    {
        for (var i = 0; i < extension.Count; i++) extension[i] = NormalizeExtension(extension[i]);
    }


    public static void GetFileNameWithoutExtensionAndExtension(string filePath, out string file, out string ext)
    {
        file = Path.GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(file);
    }
    public static void SaveStream(string path, Stream text)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            CopyStream(text, fileStream);
            fileStream.Flush();
        }
    }
    public static List<string> OnlyNamesWithoutExtensionCopy(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        for (var i = 0; i < paths.Count; i++) result.Add(Path.GetFileNameWithoutExtension(paths[i]));
        return result;
    }
    public static bool DirectoryExistsAndIsNotEmpty(string value)
    {
        if (Directory.Exists(value) && Directory.GetFiles(value, "*", SearchOption.AllDirectories).Length != 0) return true;
        return false;
    }
    public static List<string> OnlyNamesWithoutExtension(string appendToStart, List<string> fullPaths)
    {
        var result = new List<string>(fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++)
            result.Add(appendToStart + Path.GetFileNameWithoutExtension(fullPaths[i]));
        return result;
    }
    public static string Postfix(string aPath, string text)
    {
        var result = aPath.TrimEnd('\\') + text;
        WithEndSlash(ref result);
        return result;
    }
    public static
        async Task<string>
        ReadAllText(string path)
    {
        return
            await FileAsync.ReadAllTextAsync(path);
    }
    public static string GetFileNameWithoutExtension(string text)
    {
        return PathMs.GetFileNameWithoutExtension(text.TrimEnd(PathMs.DirectorySeparatorChar));
    }

    public static StorageFile GetFileNameWithoutExtensionNoAc<StorageFile>(StorageFile text)
    {
        var ss = text!.ToString();
        var result = Path.GetFileNameWithoutExtension(ss!.TrimEnd('\\'));
        var ext = Path.GetExtension(ss).TrimStart('.');
        LetterAndDigitCharService letterAndDigitChar = new LetterAndDigitCharService();
        if (!ext.All(data =>
                letterAndDigitChar.AllCharsWithoutSpecial.Contains(data)) /*SH.ContainsOnly(ext, AllChars.allCharsWithoutSpecial)*/)
            if (ext != string.Empty)
                return (dynamic)result + "." + ext;
        return (dynamic)result;
    }
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile text,
    //AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        return GetFileNameWithoutExtension<StorageFolder, StorageFile>(text, null)
    //    }
    //    ThrowNotImplementedUwp();
    //    return text;
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
        var result = new List<string>(jpgcka.Count);
        for (var i = 0; i < jpgcka.Count; i++) result.Add(Path.GetFileNameWithoutExtension(jpgcka[i]));
        return result;
    }
    public static string GetTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
    }
    public static void CopyTo(string value, string targetDirectory, FileMoveCollisionOption collisionOption)
    {
        var fileTo = Path.Combine(targetDirectory, Path.GetFileName(value));
        CopyFile(value, fileTo, collisionOption);
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

    public static string ReplaceDirectoryThrowExceptionIfFromDoesntExists(string path, string folderWithProjectsFolders,
        string folderWithTemporaryMovedContentWithoutBackslash)
    {
        path = SH.FirstCharUpper(path);
        folderWithProjectsFolders = SH.FirstCharUpper(folderWithProjectsFolders);
        folderWithTemporaryMovedContentWithoutBackslash =
            SH.FirstCharUpper(folderWithTemporaryMovedContentWithoutBackslash);
        if (!ThrowEx.NotContains(path, folderWithProjectsFolders))
            // Here can never accomplish when exc was throwed
            return path;
        // Here can never accomplish when exc was throwed
        return path.Replace(folderWithProjectsFolders, folderWithTemporaryMovedContentWithoutBackslash);
    }

    public static List<string> OnlyNamesWithoutExtension(List<string> path)
    {
        for (var i = 0; i < path.Count; i++) path[i] = Path.GetFileNameWithoutExtension(path[i]);
        return path;
    }

    public static void GetPathAndFileName(string filePath, out string path, out string file, out string ext)
    {
        path = WithEndSlash(Path.GetDirectoryName(filePath)!);
        file = Path.GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(filePath);
    }
    public static string GetAbsolutePath2(string relativePath, string dir)
    {
        var ap = GetAbsolutePath(dir, relativePath);
        return Path.GetFullPath(ap);
    }
    public static string GetAbsolutePath(string dir, string relativePath)
    {
        FileToDirectory(ref dir);
        var currentDirectoryPrefix = "./";
        var parentDirectoryPrefix = "../";
        var parentDirectoryCount = 0;
        while (true)
            if (relativePath.StartsWith(currentDirectoryPrefix))
            {
                relativePath = relativePath.Substring(currentDirectoryPrefix.Length);
            }
            else if (relativePath.StartsWith(parentDirectoryPrefix))
            {
                parentDirectoryCount++;
                relativePath = relativePath.Substring(parentDirectoryPrefix.Length);
            }
            else
            {
                break;
            }
        var tokens = GetTokens(relativePath);
        tokens = tokens.Skip(parentDirectoryCount).ToList();
        tokens.Insert(0, dir);
        var result = Combine(tokens.ToArray());
        result = GetFullPath(result);
        return result;
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
        return SHSplit.Split(relativePath, deli);
    }
    public static void CopyStream(Stream input, Stream output)
    {
        var buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0) output.Write(buffer, 0, len);
    }

    public static string CombineWithoutFirstCharUpper(params string[] text)
    {
        return CombineWorker(false, true, text);
    }
    public static void SaveMemoryStream(MemoryStream mss, string path)
    {
        //SaveMemoryStream<string, string>(mss, path, null);
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            var matriz = mss.ToArray();
            fileStream.Write(matriz, 0, matriz.Length);
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
    public static string DeleteWrongCharsInDirectoryName(string path)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in path)
            if (!invalidPathChars.Contains(item))
                stringBuilder.Append(item);
        var result = stringBuilder.ToString();
        SH.FirstCharUpper(ref result);
        return result;
    }
    public static string DeleteWrongCharsInFileName(string path, bool isPath)
    {
        List<char>? invalidFileNameChars2 = null;
        if (isPath)
            invalidFileNameChars2 = invalidFileNameCharsWithoutDelimiterOfFolders;
        else
            invalidFileNameChars2 = InvalidFileNameChars;
        var stringBuilder = new StringBuilder();
        foreach (var item in path)
            if (!invalidFileNameChars2.Contains(item))
                stringBuilder.Append(item);
        var result = stringBuilder.ToString();
        SH.FirstCharUpper(ref result);
        return result;
    }
    public static bool ContainsInvalidPathCharForPartOfMapPath(string path)
    {
        foreach (var item in invalidCharsForMapPath)
            if (path.IndexOf(item) != -1)
                return true;
        return false;
    }
    public static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }
    public static List<string> OnlyNamesNoDirectEdit(string[] files2)
    {
        var tl = files2.ToList();
        return OnlyNamesNoDirectEdit(tl);
    }

    public static List<string> OnlyNamesNoDirectEdit(List<string> files2)
    {
        var files = new List<string>(files2.Count);
        for (var i = 0; i < files2.Count; i++) files.Add(Path.GetFileName(files2[i]));
        return files;
    }
    public static List<string> OnlyNamesNoDirectEdit(string appendToStart, List<string> fullPaths)
    {
        var result = new List<string>(fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++) result.Add(appendToStart + Path.GetFileName(fullPaths[i]));
        return result;
    }



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
    public static string ChangeExtension(string filePath, string newExt, bool physically)
    {
        //if (UH.HasHttpProtocol(filePath))
        //{
        //    return UH.ChangeExtension(filePath, Path.GetExtension(filePath, new GetExtensionArgs { ReturnOriginalCase = true }), newExt);
        //}
        var directory = Path.GetDirectoryName(filePath);
        var fnwoe = Path.GetFileNameWithoutExtension(filePath);
        var newPath = Path.Combine(directory!, fnwoe + newExt);
        if (physically)
            try
            {
                if (File.Exists(newPath)) File.Delete(newPath);
                File.Move(filePath, newPath);
            }
            catch
            {
            }
        FirstCharUpper(ref newPath);
        return newPath;
    }
    public static string CreateDirectory(string value, DirectoryCreateCollisionOption whenExists, SerieStyleFS serieStyle,
        bool reallyCreate)
    {
        if (Directory.Exists(value))
        {
            bool hasSerie;
            var nameWithoutSerie = GetNameWithoutSeries(value, false, out hasSerie, serieStyle);
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
                        value = newFn;
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
        if (reallyCreate) Directory.CreateDirectory(value);
        return value;
    }
    //public static List<string> GetFilesEveryFolder(string folder, string mask, SearchOption searchOption, bool _trimA1 = false)
    //{
    //    var data = Task.Run<List<string>>(async () => await GetFilesEveryFolderAsync(folder, mask, searchOption, new GetFilesEveryFolderArgs {_trimA1 =  _trimA1 })).Result;
    //    return data;
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
                        var expandedBuffer = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, expandedBuffer, 0, readBuffer.Length);
                        Buffer.SetByte(expandedBuffer, totalBytesRead, (byte)nextByte);
                        readBuffer = expandedBuffer;
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
    public static string InsertBetweenFileNameAndExtensionRemovePath(string orig, string whatInsert)
    {
        var fn = Path.GetFileNameWithoutExtension(orig);
        var element = Path.GetExtension(orig);
        return Path.Combine(fn + whatInsert + element);
    }


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
    public static string ChangeFilename(string filePath, string newFileNameWithoutPath, bool physically)
    {
        var directory = Path.GetDirectoryName(filePath);
        var newPath = Path.Combine(directory!, newFileNameWithoutPath);
        if (physically)
            try
            {
                if (File.Exists(newPath)) File.Delete(newPath);
                File.Move(filePath, newPath);
            }
            catch
            {
            }
        return newPath;
    }








    //public static string ChangeFilename<StorageFolder, StorageFile>(StorageFile item, string newFileName, bool physically, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        ChangeFilename(item.ToString(), newFileName, physically);
    //    }
    //    ThrowNotImplementedUwp();
    //    return null;
    //}
    public static string Slash(string path, bool slash)
    {
        string? result = null;
        if (slash)
            result = path.Replace("\"",
                "/"); //SHReplace.ReplaceAll2(path, "/", "\"");
        else
            result = path.Replace("/",
                "\""); //SHReplace.ReplaceAll2(path, "\"", " / ");
        SH.FirstCharUpper(ref result);
        return result;
    }
    public static bool TryDeleteWithRepetition(string filePath)
    {
        var attemptCount = 0;
        while (true)
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch
            {
                attemptCount++;
                if (attemptCount == 9) return false;
            }
    }
    public static bool TryDeleteFile(string filePath, out string? message)
    {
        message = null;
        try
        {
            File.Delete(filePath);
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
        if (size > NumConsts.KB)
        {
            unit = ComputerSizeUnits.KB;
            size /= NumConsts.KB;
        }
        if (size > NumConsts.KB)
        {
            unit = ComputerSizeUnits.MB;
            size /= NumConsts.KB;
        }
        if (size > NumConsts.KB)
        {
            unit = ComputerSizeUnits.GB;
            size /= NumConsts.KB;
        }
        if (size > NumConsts.KB)
        {
            unit = ComputerSizeUnits.TB;
            size /= NumConsts.KB;
        }
        return size + " " + unit;
    }
    public static string GetSizeInAutoString(long value, ComputerSizeUnits fromUnit)
    {
        return GetSizeInAutoString((double)value, fromUnit);
    }

    public static string GetSizeInAutoString(double value, ComputerSizeUnits fromUnit)
    {
        if (fromUnit != ComputerSizeUnits.B)
            // Získám hodnotu value bytech
            value = ConvertToSmallerComputerUnitSize(value, fromUnit, ComputerSizeUnits.B);
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
    private static long ConvertToSmallerComputerUnitSize(long value, ComputerSizeUnits fromUnit, ComputerSizeUnits to)
    {
        return ConvertToSmallerComputerUnitSize(value, fromUnit, to);
    }
    private static double ConvertToSmallerComputerUnitSize(double value, ComputerSizeUnits fromUnit, ComputerSizeUnits to)
    {
        if (to == ComputerSizeUnits.Auto)
            throw new Exception(
                "Output ComputerSizeUnit was specified, cannot change this setting");
        if (to == ComputerSizeUnits.KB && fromUnit != ComputerSizeUnits.KB)
            value *= 1024;
        else if (to == ComputerSizeUnits.MB && fromUnit != ComputerSizeUnits.MB)
            value *= 1024 * 1024;
        else if (to == ComputerSizeUnits.GB && fromUnit != ComputerSizeUnits.GB)
            value *= 1024 * 1024 * 1024;
        else if (to == ComputerSizeUnits.TB && fromUnit != ComputerSizeUnits.TB) value *= 1024L * 1024L * 1024L * 1024L;
        return value;
    }
    public static string RepairFilter(string filter)
    {
        if (!filter.Contains("|"))
        {
            filter = filter.TrimStart('*');
            return "*" + filter + "|" + "*" + filter;
        }
        return filter;
    }
    public static string ReplaceIncorrectCharactersFile(string path)
    {
        var result = path;
        foreach (var item in InvalidFileNameChars)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item2 in result)
                if (item != item2)
                    stringBuilder.Append(item2);
                else
                    stringBuilder.Append("");
            result = stringBuilder.ToString();
        }
        return result;
    }
    public static string ReplaceIncorrectCharactersFile(string path, string replaceAllOfThisByA3, string replaceForThis)
    {
        var result = path;
        foreach (var item in InvalidFileNameChars)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item2 in result)
                if (item != item2)
                    stringBuilder.Append(item2);
                else
                    stringBuilder.Append(replaceForThis);
            result = stringBuilder.ToString();
        }
        if (!string.IsNullOrEmpty(replaceAllOfThisByA3))
            foreach (var item in replaceAllOfThisByA3)
                result = /*SHReplace.ReplaceAll*/ result.Replace(item.ToString(), replaceForThis)
                    ; //(result, replaceForThis, item.ToString());
        return result;
    }
    public static string ReplaceIncorrectCharactersFile(string path, string replaceAllOfThisThen)
    {
        var replaceFor = "";
        var result = path;
        foreach (var item in InvalidFileNameChars)
        {
            var stringBuilder = new StringBuilder();
            foreach (var item2 in result)
                if (item != item2)
                    stringBuilder.Append(item2);
                else
                    stringBuilder.Append(replaceFor);
            result = stringBuilder.ToString();
        }
        if (!string.IsNullOrEmpty(replaceAllOfThisThen))
        {
            result = result.Replace(replaceAllOfThisThen,
                replaceFor); // SHReplace.ReplaceAll(result, replaceFor, replaceAllOfThisThen);
            result = result.Replace(" ",
                replaceFor); //SHReplace.ReplaceAll(result, replaceFor, "");
        }
        return result;
    }
    public static string InsertBetweenFileNameAndPath(string folder, string parentFolder, string insert)
    {
        ThrowEx.IsNotWindowsPathFormat(nameof(folder), folder, true, FS.IsWindowsPathFormat);
        if (parentFolder == null) parentFolder = Path.GetDirectoryName(folder)!;
        var outputFolder = Path.Combine(parentFolder, insert);
        CreateFoldersPsysicallyUnlessThere(outputFolder);
        return Path.Combine(outputFolder, Path.GetFileName(folder));
    }
    public static string ChangeDirectory(string fileName, string changeFolderTo)
    {
        var path = Path.GetDirectoryName(fileName);
        var fn = Path.GetFileName(fileName);
        return Path.Combine(changeFolderTo, fn);
    }
    public static List<string> DirectoryListing(string path, string mask, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(path, mask, so, new GetFilesArgsFS { TrimFirstPathAndLeadingBackslashes = true });
        return files;
    }
    public static string WithoutEndSlash(string value)
    {
        return WithoutEndSlash(ref value);
    }
    public static string WithoutEndSlash(ref string value)
    {
        value = value.TrimEnd('\\');
        return value;
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
        //var isContained = AllExtensionsHelperSH.IsContained(ext);
        //ComplexInfoString cis = new ComplexInfoString(fn);
        //var isNoMascEntered = !((ext2.Contains("*") || ext2.Contains("?")));// && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0));
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
    public static bool IsCountOfFilesMoreThan(string folderPath, string searchPattern, int getNullIfThereIsMoreThanXFiles)
    {
        var files = FSGetFiles.GetFilesEveryFolder(folderPath, searchPattern, SearchOption.AllDirectories,
            new GetFilesEveryFolderArgsFS { GetNullIfThereIsMoreThanXFiles = getNullIfThereIsMoreThanXFiles });
        return files == null;
    }
    public static List<string> GetFiles(string folderPath, bool recursive)
    {
        return FSGetFiles.GetFiles(folderPath, "*.*",
            recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
    }
    //    public static
    //    async Task<string>
    //#else
    //string
    //#endif
    //    ReadAllText(string filename)
    //    {
    //        return
    //        await
    //#endif
    //        FileAsync.ReadAllTextAsync(filename);
    //    }

    public static void MoveFile(ILogger logger, string sourceFilePath, string fileTo, FileMoveCollisionOption collisionOption)
    {
        if (CopyMoveFilePrepare(ref sourceFilePath, ref fileTo, collisionOption))
            try
            {
                sourceFilePath = MakeUncLongPath(sourceFilePath);
                fileTo = MakeUncLongPath(fileTo);
                if (collisionOption == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo)) return;
                FileCompat.Move(sourceFilePath, fileTo, collisionOption == FileMoveCollisionOption.Overwrite);
            }
            catch (Exception ex)
            {
                logger.LogError(sourceFilePath + " : " + ex.Message);
            }
    }
    public static bool CopyMoveFilePrepare(ref string sourceFilePath, ref string fileTo, FileMoveCollisionOption collisionOption)
    {
        //var fileTo = fileTo2.ToString();
        sourceFilePath = @"\\?\" + sourceFilePath;
        MakeUncLongPath(ref fileTo);
        CreateUpfoldersPsysicallyUnlessThere(fileTo);
        // Toto tu je důležité, nevím který kokot to zakomentoval
        if (File.Exists(fileTo))
        {
            if (collisionOption == FileMoveCollisionOption.AddFileSize)
            {
                var newFn = InsertBetweenFileNameAndExtension(fileTo, " " + new FileInfo(sourceFilePath).Length);
                if (File.Exists(newFn))
                {
                    File.Delete(sourceFilePath);
                    return true;
                }
                fileTo = newFn;
            }
            else if (collisionOption == FileMoveCollisionOption.AddSerie)
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
            else if (collisionOption == FileMoveCollisionOption.DiscardFrom)
            {
                // Cant delete from because then is file deleting
                if (DeleteFileMaybeLocked != null)
                    DeleteFileMaybeLocked(sourceFilePath);
                else
                    File.Delete(sourceFilePath);
            }
            else if (collisionOption == FileMoveCollisionOption.Overwrite)
            {
                if (DeleteFileMaybeLocked != null)
                    DeleteFileMaybeLocked(fileTo);
                else
                    File.Delete(fileTo);
            }
            else if (collisionOption == FileMoveCollisionOption.LeaveLarger)
            {
                var fsFrom = new FileInfo(sourceFilePath).Length;
                var fsTo = new FileInfo(fileTo).Length;
                if (fsFrom > fsTo)
                    File.Delete(fileTo);
                else //if (fsFrom < fsTo)
                    File.Delete(sourceFilePath);
            }
            else if (collisionOption == FileMoveCollisionOption.DontManipulate)
            {
                if (File.Exists(fileTo)) return false;
            }
            else if (collisionOption == FileMoveCollisionOption.ThrowEx)
            {
                ThrowEx.Custom($"Directory {fileTo} already exists");
            }
        }
        return true;
    }
    public static long GetFileSize(string filePath)
    {
        FileInfo? fi = null;
        try
        {
            fi = new FileInfo(filePath);
        }
        catch (Exception)
        {
            // Například příliš dlouhý název souboru
            return 0;
        }
        if (fi.Exists) return fi.Length;
        return 0;
    }
    public static void CopyAllFilesRecursively(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, path, to, collisionOption, false, contains!, SearchOption.AllDirectories);
    }

    public static void CopyAllFiles(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, path, to, collisionOption, false, contains!, SearchOption.TopDirectoryOnly);
    }
    private static void CopyMoveAllFilesRecursively(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, bool move,
        string mustContains, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(path, "*", so);
        if (!string.IsNullOrEmpty(mustContains))
        {
            foreach (var item in files)
                if (SH.IsContained(item, mustContains))
                {
                    MoveOrCopy(logger, path, to, collisionOption, move, item);
                }
        }
        else
        {
            foreach (var item in files) MoveOrCopy(logger, path, to, collisionOption, move, item);
        }
    }
    private static void MoveOrCopy(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, bool move, string filePath)
    {
        var fileTo = to + filePath.Substring(path.Length);
        if (move)
            MoveFile(logger, filePath, fileTo, collisionOption);
        else
            CopyFile(logger, filePath, fileTo, collisionOption);
    }
    public static
        void
        CopyFile(ILogger logger, string sourceFilePath, string fileTo2, FileMoveCollisionOption collisionOption, bool terminateProcessIfIsInUsed = false)
    {
        var fileTo = fileTo2;
        var source = sourceFilePath;
        var shouldCopy =
            CopyMoveFilePrepare(ref source, ref fileTo, collisionOption);
        if (shouldCopy)
        {
            if (collisionOption == FileMoveCollisionOption.DontManipulate &&
                File.Exists(fileTo))
                return;
            CopyFile(logger, source, fileTo, terminateProcessIfIsInUsed);
        }
    }

    public static void CopyFile(ILogger logger, string jsFiles, string value, bool terminateProcessIfIsInUsed = false)
    {
        try
        {
            File.Copy(jsFiles, value, true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("because it is being used by another process") && terminateProcessIfIsInUsed)
            {
                if (FileUtilWhoIsLocking != null)
                {
                    var pr = FileUtilWhoIsLocking(jsFiles, true);
                    foreach (var item in pr) item.Kill();
                }
                // Používá se i ve web, musel bych tam includovat spoustu metod
                //PH.ShutdownProcessWhichOccupyFileHandleExe(jsFiles);
                try
                {
                    File.Copy(jsFiles, value, true);
                }
                catch (Exception ex2)
                {
                    logger.LogError($"{jsFiles}: {Exceptions.TextOfExceptions(ex2)}");
                }
            }
            else
            {
                logger.LogError($"{jsFiles}: {Exceptions.TextOfExceptions(ex)}");
            }
        }
    }
    public static void CopyFile(string sourceFilePath, string fileTo2, FileMoveCollisionOption collisionOption)
    {
        var fileTo = fileTo2;
        if (CopyMoveFilePrepare(ref sourceFilePath, ref fileTo, collisionOption))
        {
            if (collisionOption == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo)) return;
            File.Copy(sourceFilePath, fileTo);
        }
    }
    public static DateTime LastModified(string rel)
    {
        if (File.Exists(rel)) return File.GetLastWriteTime(rel);
        // FileInfo mi držel soubor a vznikali chyby The process cannot access the file
        //var f = new FileInfo(rel);
        //var result = f.LastWriteTime;
        //return result;
        return DateTime.MinValue;
    }
    public static bool TryDeleteDirectoryOrFile(string value)
    {
        if (!TryDeleteDirectory(value)) return TryDeleteFile(value);
        return true;
    }
    //public static Func<string, List<string>> InvokePs;
    private static void KillProcessesHoldingDirectory(string directoryPath)
    {
        if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            return;
        try
        {
            var processes = System.Diagnostics.Process.GetProcesses();
            foreach (var process in processes)
            {
                try
                {
                    if (process.HasExited)
                        continue;
                    foreach (System.Diagnostics.ProcessModule module in process.Modules)
                    {
                        if (module.FileName.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase))
                        {
                            try
                            {
                                process.Kill();
                                process.WaitForExit(1000);
                            }
                            catch { }
                            break;
                        }
                    }
                }
                catch { }
            }
            try
            {
                var handleExePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "handle.exe");
                if (!File.Exists(handleExePath))
                {
                    handleExePath = "handle.exe";
                }
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = handleExePath,
                    Arguments = $"-accepteula -nobanner \"{directoryPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
                };
                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    var output = process!.StandardOutput.ReadToEnd();
                    process.WaitForExit(3000);
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.Contains(" pid: "))
                        {
                            var pidStart = line.IndexOf(" pid: ") + 6;
                            var pidEnd = line.IndexOf(' ', pidStart);
                            if (pidEnd == -1) pidEnd = line.Length;
                            if (int.TryParse(line.Substring(pidStart, pidEnd - pidStart), out int pid))
                            {
                                try
                                {
                                    var proc = System.Diagnostics.Process.GetProcessById(pid);
                                    proc.Kill();
                                    proc.WaitForExit(1000);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            catch { }
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }
        catch { }
    }

    public static bool TryDeleteDirectory(string value)
    {
        if (!Directory.Exists(value)) return true;
        try
        {
            Directory.Delete(value, true);
            return true;
        }
        catch (Exception)
        {
            // Je to try takže nevím co tu dělá tohle a
            //ThrowEx.FolderCannotBeDeleted(value, ex);
            //var result = InvokePs(value);
            //if (result.Count > 0)
            //{
            //    return false;
            //}
        }
        var files = FSGetFiles.GetFiles(value, "*", SearchOption.AllDirectories);
        foreach (var item in files) File.SetAttributes(item, FileAttributes.Normal);
        try
        {
            Directory.Delete(value, true);
            return true;
        }
        catch (Exception)
        {
            try
            {
                KillProcessesHoldingDirectory(value);
                System.Threading.Thread.Sleep(500);
                var dirs = Directory.GetDirectories(value, "*", SearchOption.AllDirectories);
                foreach (var dir in dirs)
                {
                    try
                    {
                        Directory.SetCurrentDirectory(Path.GetTempPath());
                        var di = new DirectoryInfo(dir);
                        di.Attributes = FileAttributes.Normal;
                        foreach (var file in di.GetFiles())
                        {
                            file.Attributes = FileAttributes.Normal;
                            file.Delete();
                        }
                        di.Delete(true);
                    }
                    catch { }
                }
                Directory.SetCurrentDirectory(Path.GetTempPath());
                var rootDi = new DirectoryInfo(value);
                rootDi.Attributes = FileAttributes.Normal;
                foreach (var file in rootDi.GetFiles())
                {
                    file.Attributes = FileAttributes.Normal;
                    file.Delete();
                }
                rootDi.Delete(true);
                return true;
            }
            catch
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    try
                    {
                        var psi = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c rmdir /s /q \"{value}\"",
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        var process = System.Diagnostics.Process.Start(psi);
                        process!.WaitForExit(5000);
                        if (!Directory.Exists(value))
                            return true;
                    }
                    catch { }
                }
            }
        }
        return false;
    }
    public static string AllIncludeIfOnlyLetters(string extension)
    {
        extension = extension.ToLower().TrimStart('*').TrimStart('.');

        if (extension == "")
        {
            extension = "*";
        }

        //if ( SH.ContainsOnlyCase(extension.ToLower(), false, false))
        //{
        extension = "*." + extension;
        //}
        return extension;
    }
    public static string? GetFileSerie(string fnwoe, SerieStyleFS ss)
    {
        if (ss == SerieStyleFS.Brackets)
        {
            return SHParts.GetTextBetweenTwoChars(fnwoe, '(', ')');
        }
        ThrowEx.NotImplementedMethod();
        return null;
    }

    public static string GetFileSeries(string folder, string fileName, string ext)
    {
        var nextNumber = 0;
        var files = FSGetFiles.GetFiles(folder);
        foreach (var item in files)
        {
            int path;
            var withoutFileName =
                new Regex(fileName).Replace(Path.GetFileName(item), "",
                    1); /*SHReplace.ReplaceOnce(Path.GetFileName(item), fileName, "")));*/
            var withoutFileNameAndExt = SHReplace.ReplaceOnce(withoutFileName, ext, "");
            withoutFileNameAndExt = withoutFileNameAndExt.TrimStart('_');
            if (int.TryParse(withoutFileNameAndExt, out path))
                if (path > nextNumber)
                    nextNumber = path;
        }
        nextNumber++;
        return Path.Combine(folder, fileName + "_" + nextNumber + ext);
    }
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

    //public static bool ExistsFileAc<StorageFolder, StorageFile>(StorageFile selectedFile, AbstractCatalog<StorageFolder, StorageFile> ac = null)
    //{
    //    if (ac == null)
    //    {
    //        return File.Exists(selectedFile.ToString());
    //    }
    //    return ac.fs.existsFile.Invoke(selectedFile);
    //}
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
    public static bool? ExistsDirectoryNull(string directoryPath)
    {
        return ExistsDirectoryNull(directoryPath, false);
    }
    public static bool? ExistsDirectoryNull(string directoryPath, bool isFalseIfContainsNoFile = false)
    {
        return ExistsDirectory(directoryPath, isFalseIfContainsNoFile);
    }
    public static bool ExistsDirectory(string directoryPath, bool isFalseIfContainsNoFile = false)
    {
        if (isFalseIfContainsNoFile)
        {
            if (Directory.Exists(directoryPath) && Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Length == 0)
            {
                return false;
            }
        }
        return Directory.Exists(directoryPath);
        //return ExistsDirectory<string, string>(directoryPath, null, isFalseIfContainsNoFile);
    }
    #region For easy copy
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static string NormalizeExtension2(string item)
    //{
    //    return se.FS.NormalizeExtension2(item);
    //}
    //public static string NonSpacesFilename(string nameOfPage)
    //{
    //    ThrowEx.NotImplementedMethod();
    //    return null;
    //    //var value = ConvertCamelConventionWithNumbers.ToConvention(nameOfPage);
    //    //v = FS.ReplaceInvalidFileNameChars(value);
    //    //return value;
    //}
    #endregion
    #region Making problem in translate
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
            generator.StringBuilder.Append(withEndFlash);
            //generator.StringBuilder.CanUndo = true;
            for (; resultSerie < int.MaxValue; resultSerie++)
            {
                generator.StringBuilder.Append(resultSerie);
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
    public static async Task WriteAllText(string path, string content)
    {
        await FileAsync.WriteAllTextAsync(path, content);
    }
    public static bool IsAllInSameFolder(List<string> paths)
    {
        if (paths.Count > 0)
        {
            var baseDirectory = Path.GetDirectoryName(paths[0]);
            for (var i = 1; i < paths.Count; i++)
                if (Path.GetDirectoryName(paths[i]) != baseDirectory)
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
        foreach (var item in invalidFileNameStringsReadonly)
            if (arg.Contains(item))
                return true;
        return false;
    }
    public static void NumberByDateModified(ILogger logger, string folder, string searchPattern, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(folder, searchPattern, so, new GetFilesArgsFS { ByDateOfLastModifiedAsc = true });
        var i = 1;
        foreach (var item in files)
        {
            RenameFile(logger, item, i + Path.GetExtension(item), FileMoveCollisionOption.DontManipulate);
            i++;
        }
    }
    #endregion
    #region GetDirectoryName

    public static string GetDirectoryName(string path)
    {
        // Zde zároveň vyhazuji výjimky
        var deli = DetectPathDelimiterChar(path);
        if (string.IsNullOrEmpty(path)) ThrowEx.IsNullOrEmpty("path", path);
        if (!IsWindowsPathFormat(path)) ThrowEx.IsNotWindowsPathFormat("path", path, true, FS.IsWindowsPathFormat);
        path = path.TrimEnd(deli);
        var delimiterIndex = path.LastIndexOf(deli);
        if (delimiterIndex != -1)
        {
            var result = path.Substring(0, delimiterIndex + 1);
            FirstCharUpper(ref result);
            return result;
        }
        return "";
    }
    public static Tuple<bool, bool> DetectPathDelimiter(string path)
    {
        var containsFs = path.Contains("/");
        var containsBs = path.Contains("\\");
        if (containsBs && containsFs) throw new Exception("Path contains both fs & bs");
        return Tuple.Create(containsFs, containsBs);
    }
    public static char DetectPathDelimiterChar(string path)
    {
        var delimiterInfo = DetectPathDelimiter(path);
        var containsFs = delimiterInfo.Item1;
        var containsBs = delimiterInfo.Item2;
        var deli = 'a';
        if (containsBs)
            deli = '\\';
        else if (containsFs)
            deli = '/';
        else
            throw new Exception("Path contains no delimiter");
        return deli;
    }
    public static bool IsWindowsPathFormat(string argValue)
    {
        PathFormatDetectorService pathFormatDetector = new(NullLogger.Instance);
        return pathFormatDetector.IsWindowsPathFormat(argValue);
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
            // value ASP.net mi vrátilo u každé directory.exists false. Byl jsem pod ApplicationPoolIdentity value IIS a bylo nastaveno Full Control pro IIS AppPool\DefaultAppPool
        }
        return path;
    }
    #endregion
    //public static string GetFileNameWithoutExtension(string text)
    //{
    //    return Path.GetFileNameWithoutExtension(text);
    //    //return GetFileNameWithoutExtension<string, string>(text, null);
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
    //public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    //{
    //    path = Path.GetDirectoryName(fn) + '\\';
    //    file = GetFileNameWithoutExtension(fn);
    //    ext = Path.GetExtension(fn);
    //}
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile text, AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ss = text.ToString();
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
    //        return text;
    //    }
    //}
    //public static string GetFileNameWithoutExtension(string text)
    //{
    //    return GetFileNameWithoutExtension<string, string>(text, null);
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
    //        ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-Translate.FromKey(XlfKeys.UseMethodsInFSApps));
    //#endif
    //#if WINDOWS_UWP
    //        ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-Translate.FromKey(XlfKeys.UseMethodsInFSApps));
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
    //public static bool IsCountOfFilesMoreThan(string bpMb, int value)
    //{
    //    return false;
    //}
    #region FirstCharUpper
    public static string FirstCharUpper(ref string result)
    {
        if (IsWindowsPathFormat(result)) result = SH.FirstCharUpper(result);
        return result;
    }
    public static string? FirstCharUpper(string text, bool only = false)
    {
        if (text != null)
        {
            var substring = text.Substring(1);
            if (only) substring = substring.ToLower();
            return text[0].ToString().ToUpper() + substring;
        }
        return null;
    }
    #endregion

    public static void GetPathAndFileName(string filePath, out string path, out string file)
    {
        path = WithEndSlash(GetDirectoryName(filePath));
        file = GetFileName(filePath);
    }
    public static string GetFileName(string filePath)
    {
        return PathMs.GetFileName(filePath.TrimEnd(Path.DirectorySeparatorChar));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension2(string extension)
    {
        return extension.ToLower().TrimStart('.');
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
    //                // Získám hodnotu value bytech
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
    //        public static List<string> GetFiles(string path, string value, SearchOption topDirectoryOnly)
    //        {
    //            return GetFilesMoreMasc(path, value, topDirectoryOnly).ToList();
    //        }
    //
    //        public static List<string> GetFilesMoreMasc(string path, string value, SearchOption topDirectoryOnly)
    //        {
    //            var count = ",";
    //            var sc = ";";
    //            List<string> result = new List<string>();
    //            List<string> masks = new List<string>();
    //
    //            if (value.Contains(count))
    //            {
    //                masks.AddRange(SHSplit.Split(value, count));
    //            }
    //            else if (value.Contains(sc))
    //            {
    //                masks.AddRange(SHSplit.Split(value, sc));
    //            }
    //            else
    //            {
    //                masks.Add(value);
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
    //                    // TODO: Tady to nefunguje pro UWP/UAP apps protoze nemaji pristup k celemu disku. Zjistit co to je UWP/UAP/... a jak value nem ziskat/overit jakoukoliv slozku na disku
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
    //            return FileAsync.ReadAllTextAsync(filename);
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
    //            if (FilesWhichSurelyExists.Contains(selectedFile))
    //            {
    //                return true;
    //            }
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
    //                        var count = string.Empty;
    //                        try
    //                        {
    //                            count = FileAsync.ReadAllTextAsync(selectedFile);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            if (ex.Message.StartsWith("The process cannot access the file"))
    //                            {
    //                                return true;
    //                            }
    //                        }
    //                        if (count == string.Empty)
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
    //public static string Combine(params string[] text)
    //{
    //    return CombineWorker(true, text);
    //}
    //private static string CombineWorker(bool FirstCharUpper, params string[] text)
    //{
    //    text = CA.TrimStart('\\', text).ToArray();
    //    var result = Path.Combine(text);
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
    //public static string GetFileNameWithoutExtension(string text)
    //{
    //    return GetFileNameWithoutExtension<string, string>(text, null);
    //}





    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile text, AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ss = text.ToString();
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
    //        return text;
    //    }
    //}
    //        #region  from FSShared.cs
    //        public static void DeleteFile(string item)
    //        {
    //            File.Delete(item);
    //        }
    //
    //



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
    //public static List<string> GetFilesMoreMasc(string path, string masc, SearchOption searchOption, GetFilesMoreMascArgs element = null)
    //{
    //    if (element == null)
    //    {
    //        element = new GetFilesMoreMascArgs();
    //    }
    //    element.path = path;
    //    element.masc = masc;
    //    element.searchOption = searchOption;
    //    return GetFilesMoreMasc(element);
    //}
    //public static List<string> GetFilesMoreMasc(GetFilesMoreMascArgs element = null)
    //{
    //    Thread temp = new Thread(new ParameterizedThreadStart(GetFilesMoreMascWorker));
    //    temp.Start();
    //}
    //private static void GetFilesMoreMascWorker(object o)
    //{
    //var element = (GetFilesMoreMascArgs)o;
    #endregion
    public static string FilesWithSameName(string folder, string searchPattern, SearchOption searchOption)
    {
        WithEndSlash(ref folder);
        var filesByName = new Dictionary<string, List<string>>();
        var text = FSGetFiles.GetFiles(folder, searchPattern, searchOption);
        foreach (var item in text) DictionaryHelper.AddOrCreate(filesByName, Path.GetFileName(item), item);
        var stringBuilder = new StringBuilder();
        //TextOutputGenerator tog = new TextOutputGenerator();
        foreach (var item in filesByName)
            if (item.Value.Count > 1)
            {
                foreach (var item2 in item.Value) stringBuilder.AppendLine(item2);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                //tog.List(item.Value);
            }
        return stringBuilder.ToString();
    }
    #region For easy copy - GetNameWithoutSeries
    public static string GetNameWithoutSeries(string path, bool a1IsWithPath)
    {
        int serie;
        var hasSerie = false;
        return GetNameWithoutSeries(path, a1IsWithPath, out hasSerie, SerieStyleFS.Brackets, out serie);
    }
    //public static string GetNameWithoutSeries(string path, bool path, out bool hasSerie, SerieStyle serieStyle)
    //{
    //    int serie;
    //    return GetNameWithoutSeries(path, path, out hasSerie, serieStyle, out serie);
    //}

    public static (string, bool) GetNameWithoutSeriesNoOut(string path, bool a1IsWithPath, SerieStyleFS serieStyle)
    {
        int serie;
        var result = GetNameWithoutSeries(path, a1IsWithPath, out var hasSerie, serieStyle, out serie);
        return (result, hasSerie);
    }
    public static string GetNameWithoutSeries(string path, bool a1IsWithPath, out bool hasSerie, SerieStyleFS serieStyle)
    {
        int serie;
        return GetNameWithoutSeries(path, a1IsWithPath, out hasSerie, serieStyle, out serie);
    }

    public static string GetNameWithoutSeries(string path, bool a1IsWithPath, out bool hasSerie, SerieStyleFS serieStyle,
        out int serie)
    {
        serie = -1;
        hasSerie = false;
        var directory = string.Empty;
        if (a1IsWithPath) directory = WithEndSlash(Path.GetDirectoryName(path)!);
        var sbExt = new StringBuilder();
        var ext = Path.GetExtension(path);
        //if (ext == string.Empty)
        //{
        //    return path;
        //}
        var seriesCount = 0;
        path = SHParts.RemoveAfterLast(path, ".");
        sbExt.Append(ext);
        ext = sbExt.ToString();
        var fullPath = path;
        if (directory.Length != 0)
        {
            fullPath = fullPath.Substring(directory.Length);
        }
        // Nejdříve ořežu všechny přípony a to i tehdy, má li soubor více přípon
        if (serieStyle == SerieStyleFS.Brackets || serieStyle == SerieStyleFS.All)
            while (true)
            {
                fullPath = fullPath.Trim();
                var lb = fullPath.LastIndexOf('(');
                var rb = fullPath.LastIndexOf(')');
                if (lb != -1 && rb != -1)
                {
                    var between = fullPath.Substring(lb + 1, rb - lb - 1); //SH.GetTextBetweenTwoCharsInts(fullPath, lb, rb);
                    if (double.TryParse(between, out var _) /*SH.IsNumber(between, [])*/)
                    {
                        serie = int.Parse(between);
                        seriesCount++;
                        // text - 4, on end (1) -
                        fullPath = fullPath.Substring(0, lb);
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
            if (fullPath[fullPath.Length - 3] == '-')
            {
                serie = int.Parse(fullPath.Substring(fullPath.Length - 2));
                fullPath = fullPath.Substring(0, fullPath.Length - 3);
            }
            else if (fullPath[fullPath.Length - 2] == '-')
            {
                serie = int.Parse(fullPath.Substring(fullPath.Length - 1));
                fullPath = fullPath.Substring(0, fullPath.Length - 2);
            }
            if (serie != -1)
                // To true hasSerie
                seriesCount++;
        }
        if (serieStyle == SerieStyleFS.Underscore || serieStyle == SerieStyleFS.All)
            RemoveSerieUnderscore(ref serie, ref fullPath, ref seriesCount);
        if (seriesCount != 0) hasSerie = true;
        fullPath = fullPath.Trim();
        if (a1IsWithPath) return directory + fullPath + ext;
        return fullPath + ext;
    }
    public static string RemoveSerieUnderscore(string data)
    {
        var serie = 0;
        var seriesCount = 0;
        RemoveSerieUnderscore(ref serie, ref data, ref seriesCount);
        return data;
    }
    private static void RemoveSerieUnderscore(ref int serie, ref string text, ref int seriesCount)
    {
        while (true)
        {
            var underscoreIndex = text.LastIndexOf('_');
            if (underscoreIndex != -1)
            {
                var serieS = text.Substring(underscoreIndex + 1);
                text = text.Substring(0, underscoreIndex);
                if (int.TryParse(serieS, out serie)) seriesCount++;
            }
            else
            {
                break;
            }
        }
    }
    #endregion
    #region For easy copy from FSShared.cs
    public static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }
    //public static void GetPathAndFileName(string fn, out string path, out string file)
    //{
    //    se.FS.GetPathAndFileName(fn, out path, out file);
    //}
    //public static string WithEndSlash(ref string value)
    //{
    //    return se.FS.WithEndSlash(ref value);
    //}
    //public static string GetDirectoryName(string rp)
    //{
    //    return se.Path.GetDirectoryName(rp);
    //}

    public static void GetPathAndFileNameWithoutExtension(string filePath, out string path, out string file, out string ext)
    {
        path = Path.GetDirectoryName(filePath) + '\\';
        file = GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(filePath);
    }
    public static string PathWithoutExtension(string path)
    {
        string path2, file, ext;
        GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
        return Combine(path2, file);
    }
    public static string GetFullPath(string path)
    {
        var result = Path.GetFullPath(path);
        FirstCharUpper(ref result);
        return result;
    }
    public static void FileToDirectory(ref string dir)
    {
        if (!dir.EndsWith("\"")) dir = GetDirectoryName(dir);
    }
    //public static string AbsoluteFromCombinePath(string a)
    //{
    //    return se.FS.AbsoluteFromCombinePath(a);
    //}
    #endregion
    #region For easy copy from FSShared64.cs
    public static bool ExistsDirectoryWorker(string directoryPath, bool isFalseIfContainsNoFile = false)
    {
        // Not working, flags from GeoCachingTool wasnt transfered to standard
#if NETFX_CORE
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-Translate.FromKey(XlfKeys.UseMethodsInFSApps));
#endif
#if WINDOWS_UWP
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-Translate.FromKey(XlfKeys.UseMethodsInFSApps));
#endif
        if (directoryPath == @"\\?\" || directoryPath == string.Empty) return false;
        var normalizedPath = MakeUncLongPath(directoryPath);
        // Directory.Exists if pass SE or only start of Unc return false
        var result = Directory.Exists(normalizedPath);
        if (isFalseIfContainsNoFile)
            if (result)
            {
                var fileCount = FSGetFiles.GetFiles(directoryPath, "*", SearchOption.AllDirectories).Count;
                result = fileCount > 0;
            }
        return result;
    }
    public static List<string> FilesWhichSurelyExists = new();
    public static
        async Task<bool>
        ExistsFile(string selectedFile, bool falseIfSizeZeroOrEmpty)
    {
        selectedFile = SH.FirstCharUpper(selectedFile);
        //ThrowEx.FirstLetterIsNotUpper(selectedFile);
        if (FilesWhichSurelyExists.Contains(selectedFile)) return true;
        if (selectedFile == @"\\?\" || selectedFile == string.Empty) return false;
        MakeUncLongPath(ref selectedFile);
        var exists = File.Exists(selectedFile);
        if (falseIfSizeZeroOrEmpty)
        {
            if (!exists) return false;
            var ext = Path.GetExtension(selectedFile).ToLower();
            // Musím to kontrolovat jen když je to tmp, logicky
            if (ext == ".tmp") return false;
            var content = string.Empty;
            try
            {
                content =
                    await FileAsync.ReadAllTextAsync(selectedFile);
            }
            catch (Exception ex)
            {
                if (ex.Message.StartsWith("The process cannot access the file")) return true;
            }
            if (content == string.Empty)
                // Měl jsem tu chybu že ač exists bylo true, File.ReadAllTextAsync selhalo protože soubor neexistoval.
                // Vyřešil jsem to kontrolou přípony, snad
                return false;
        }
        return exists;
    }


    public static string Combine(params string[] parts)
    {
        //return Path.Combine(paths);
        return CombineWorker(true, false, parts);
    }
    public static string CombineFile(params string[] parts)
    {
        return CombineWorker(true, true, parts);
    }
    public static string CombineDir(params string[] parts)
    {
        return CombineWorker(true, false, parts);
    }
    private static string CombineWorker(bool isFirstCharUpper, bool file, params string[] paths)
    {
        for (var i = 0; i < paths.Length; i++) paths[i] = paths[i].TrimStart('\\');
        //s = CA.TrimStartChar('\\', paths.ToList()).ToArray();
        var result = Path.Combine(paths);
        if (result[2] != '\\') result = result.Insert(2, "\"");
        if (isFirstCharUpper)
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
    public static long GetFolderSize(DirectoryInfo directoryInfo)
    {
        long size = 0;
        // Add file sizes.
        //
        FileInfo[]? files = null;
        try
        {
            files = directoryInfo.GetFiles();
        }
        catch (DirectoryNotFoundException)
        {
            files = new FileInfo[0];
            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api a zbylé složky value něm jsou junctiony které ale ztratily svůj cíl
        }
        foreach (var fileInfo in files) size += fileInfo.Length;
        // Add subdirectory sizes.
        DirectoryInfo[]? subdirectories = null;
        try
        {
            subdirectories = directoryInfo.GetDirectories();
        }
        catch (DirectoryNotFoundException)
        {
            subdirectories = new DirectoryInfo[0];
            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api a zbylé složky value něm jsou junctiony které ale ztratily svůj cíl
        }
        foreach (var subdirectory in subdirectories) size += GetFolderSize(subdirectory);
        return size;
    }
    public static Dictionary<string, List<string>> GroupFilesByName(List<string> filesInSubfolders)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var item in filesInSubfolders) DictionaryHelper.AddOrCreate(result, Path.GetFileName(item), item);
        return result;
    }
    public static string? BasePath(List<string> basePaths, string path)
    {
        foreach (var item in basePaths)
            if (path.Contains(item))
                return item;
        return null;
    }
    public static bool HasAnyFoldersOrFiles(string folder)
    {
        return Directory.GetFiles(folder).Length > 0 ||
               Directory.GetDirectories(folder).Length > 0;
    }
    public static void MoveDirectoryNoRecursive(string sourcePath, string targetPath, DirectoryMoveCollisionOption directoryMoveCollisionOption, object fileMoveCollisionOption)
    {
        throw new NotImplementedException();
    }
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
