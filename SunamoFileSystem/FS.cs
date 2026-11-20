namespace SunamoFileSystem;

using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

/// <summary>
/// Represents the FS type.
/// </summary>
public partial class FS
{
    /// <summary>
    /// The d ends with replace in file constant.
    /// </summary>
    public const string DEndsWithReplaceInFile = "SubdomainHelperSimple.cs";
    /// <summary>
    /// The invalid file name chars readonly.
    /// </summary>
    protected static readonly List<char> invalidFileNameCharsReadonly = Path.GetInvalidFileNameChars().ToList();
    /// <summary>
    /// The invalid file name strings readonly.
    /// </summary>
    protected static readonly List<string> invalidFileNameStringsReadonly;
    /// <summary>
    /// Is absolute path.
    /// </summary>
    /// <returns>The is absolute path result.</returns>
    public static bool IsAbsolutePath(string path)
    {
        return !String.IsNullOrWhiteSpace(path)
            && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1
            && Path.IsPathRooted(path)
            && !Path.GetPathRoot(path)!.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
    }
    /// <summary>
    /// Copy folder.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The copy folder result.</returns>
    public static void CopyFolder(string sourceDir, string targetDir)
    {
        Directory.CreateDirectory(targetDir);
        foreach (var file in Directory.GetFiles(sourceDir))
            File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
        foreach (var directory in Directory.GetDirectories(sourceDir))
            CopyFolder(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
    }
    /// <summary>
    /// The invalid path chars.
    /// </summary>
    protected static List<char> invalidPathChars;
    /// <summary>
    /// The invalid file name chars string.
    /// </summary>
    public static string InvalidFileNameCharsString;
    /// <summary>
    /// The invalid file name chars.
    /// </summary>
    public static List<char> InvalidFileNameChars;
    /// <summary>
    /// The invalid chars for map path.
    /// </summary>
    protected static List<char> invalidCharsForMapPath;
    /// <summary>
    /// The invalid file name chars without delimiter of folders.
    /// </summary>
    protected static List<char> invalidFileNameCharsWithoutDelimiterOfFolders;
    /// <summary>
    /// The replace incorrect for.
    /// </summary>
    public static string ReplaceIncorrectFor = string.Empty;
    /// <summary>
    /// The delete file maybe locked.
    /// </summary>
    public static Action<string>? DeleteFileMaybeLocked;
    /// <summary>
    /// The file util who is locking.
    /// </summary>
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
    /// <summary>
    /// Create upfolders psysically unless there.
    /// </summary>
    /// <param name="path">The path.</param>
    public static void CreateUpfoldersPsysicallyUnlessThere(string path)
    {
        CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(path)!);
    }
    /// <summary>
    /// Exists directory.
    /// </summary>
    /// <returns>The exists directory result.</returns>
    public static bool ExistsDirectory(string path)
    {
        return Directory.Exists(path);
    }
    /// <summary>
    /// Create folders psysically unless there.
    /// </summary>
    /// <returns>The create folders psysically unless there result.</returns>
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

    /// <summary>
    /// Initializes a new instance of the create directory class.
    /// </summary>
    /// <param name="'\\'">The '\\'.</param>
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
    /// <summary>
    /// Create directory if not exists.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The create directory if not exists result.</returns>
    public static void CreateDirectoryIfNotExists(string path)
    {
        MakeUncLongPath(ref path);
        if (!ExistsDirectory(path)) Directory.CreateDirectory(path);
    }
    /// <summary>
    /// With end slash.
    /// </summary>
    /// <param name="true">The true.</param>
    /// <returns>The with end slash result.</returns>
    public static string WithEndSlash(string value)
    {
        return WithEndSlash(ref value);
    }

    /// <summary>
    /// With end slash.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The with end slash result.</returns>
    public static string WithEndSlash(ref string value)
    {
        if (value != string.Empty) value = value.TrimEnd('\\') + '\\';
        FirstCharUpper(ref value);
        return value;
    }
    /// <summary>
    /// Initializes a new instance of the folders with subfolder class.
    /// </summary>
    public static List<string> FoldersWithSubfolder(string solutionFolder, string folderName)
    {
        var subFolders = Directory.GetDirectories(solutionFolder, "*", SearchOption.AllDirectories);
        var result = new List<string>();
        foreach (var item in subFolders)
        {
            /*
 I have an error here:
System.IO.DirectoryNotFoundException: 'Could not find a part of the path
            'E:\vs\Projects\PlatformIndependentNuGetPackages.net\Clients\node_modules\napi-wasm'.'
            it must be some <|>, because here it should get only for sunamo or swod
            not for sunamo.net
            */
            var subf = Directory.GetDirectories(item, folderName, SearchOption.TopDirectoryOnly).ToList();
            if (subf.Count == 1) result.Add(item);
        }
        return result;
    }
    /// <summary>
    /// First char upper.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The first char upper result.</returns>
    public static string FirstCharUpper(string text)
    {
        if (text.Length == 1) return text.ToUpper();
        var substring = text.Substring(1);
        return text[0].ToString().ToUpper() + substring;
    }
    /// <summary>
    /// Try delete file.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The try delete file result.</returns>
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
            return false;
        }
    }
    /// <summary>
    /// Write all text with exc.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="folderName">The folder name.</param>
    /// <param name="SearchOption.TopDirectoryOnly">The search option top directory only.</param>
    /// <returns>The write all text with exc result.</returns>
    public static async Task WriteAllTextWithExc(string file, string content)
    {
        try
        {
            await FileAsync.WriteAllTextAsync(file, content);
        }
        catch (Exception)
        {
        }
    }
    /// <summary>
    /// Create file if doesnt exists.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The create file if doesnt exists result.</returns>
    public static async void CreateFileIfDoesntExists(string path)
    {
        if (!File.Exists(path))
            File.WriteAllText(path, "");
    }
    /// <summary>
    /// Initializes a new instance of the insert between file name and extension class.
    /// </summary>
    /// <param name="text.Length">The text length.</param>
    public static string InsertBetweenFileNameAndExtension(string orig, string whatInsert)
    {
        // The path would be useful here because of FS.CiStorageFile
        // however I do not know whether StorageFolder is used, so it will work even without it
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
    /// <summary>
    /// Replace invalid file name chars.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <param name="content">The content.</param>
    /// <returns>The replace invalid file name chars result.</returns>
    public static string ReplaceInvalidFileNameChars(string filename, params char[] characters)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in filename)
            if (!InvalidFileNameChars.Contains(item) || characters.Contains(item))
                stringBuilder.Append(item);
        return stringBuilder.ToString();
    }





    //    // The path would be useful here because of FS.CiStorageFile
    //    // however I do not know whether StorageFolder is used, so it will work without it too
    /// <summary>
    /// Get extension.
    /// </summary>
    /// <param name="!File.Exists(path)">The file exists path.</param>
    /// <returns>The get extension result.</returns>
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
    /// <summary>
    /// Is extension.
    /// </summary>
    /// <returns>The is extension result.</returns>
    public static bool IsExtension(string result)
    {
        if (string.IsNullOrWhiteSpace(result)) return false;
        if (!result.TrimStart('.').ToLower()
                .All(character => (char.IsLetter(character) && char.IsLower(character)) || char.IsDigit(character))) return false;
        return true;
    }
    /// <summary>
    /// Exists file.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="args">The args.</param>
    /// <returns>The exists file result.</returns>
    public static bool ExistsFile(string path)
    {
        return File.Exists(path);
    }
    /// <summary>
    /// Move subfolders to folder.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="args">The args.</param>
    /// <returns>The move subfolders to folder result.</returns>
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
    /// <summary>
    /// Trim base path and trailing bs.
    /// </summary>
    /// <param name="'\\'">The '\\'.</param>
    /// <returns>The trim base path and trailing bs result.</returns>
    public static void TrimBasePathAndTrailingBs(List<string> text, string basePath)
    {
        for (var i = 0; i < text.Count; i++)
        {
            text[i] = text[i].Substring(basePath.Length);
            text[i] = text[i].TrimEnd('\\');
        }
    }
    /// <summary>
    /// Get file name without one extension.
    /// </summary>
    /// <param name="!args.ReturnOriginalCase">The args return original case.</param>
    /// <returns>The get file name without one extension result.</returns>
    public static string GetFileNameWithoutOneExtension(string path)
    {
        return SHParts.RemoveAfterLast(path, "\\");
    }
    /// <summary>
    /// Get actual date time.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The get actual date time result.</returns>
    public static string GetActualDateTime()
    {
        var dt = DateTime.Now;
        return ReplaceIncorrectCharactersFile(dt.ToString());
    }
    /// <summary>
    /// Keep only which is not in files.
    /// </summary>
    /// <param name="opts">The opts.</param>
    /// <param name="paths">The paths.</param>
    /// <returns>The keep only which is not in files result.</returns>
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
    /// <summary>
    /// Relative to absolute path.
    /// </summary>
    /// <param name="to">The to.</param>
    /// <param name="item">The item.</param>
    /// <returns>The relative to absolute path result.</returns>
    public static string RelativeToAbsolutePath(string fullPathToSecondFile, string relativePath)
    {
        var fullPathToFirstFile =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(fullPathToSecondFile)!, relativePath));
        return fullPathToFirstFile;
    }
    // Why do I call it here? It should be called in the applications where I need it
    //    // Must call Initialize here, not in Loaded of Window. when I run auto code in debug, it wont be initialized as is needed.
    /// <summary>
    /// Absolute from combine path.
    /// </summary>
    /// <param name="basePath.Length">The base path length.</param>
    /// <returns>The absolute from combine path result.</returns>
    public static string AbsoluteFromCombinePath(string path)
    {
        var result = Path.GetFullPath(new Uri(path).LocalPath);
        return result;
    }
    /// <summary>
    /// Wrap with qm.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name=""\\"">The "\\".</param>
    /// <returns>The wrap with qm result.</returns>
    public static string WrapWithQm(string text, bool? forceNotIncludeQm)
    {
        if (text.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return SH.WrapWithQm(text);
        return text;
    }
    /// <summary>
    /// Filter in root and in sub folder.
    /// </summary>
    /// <param name="dt.ToString()">The dt to string.</param>
    /// <returns>The filter in root and in sub folder result.</returns>
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
    /// <summary>
    /// Only names.
    /// </summary>
    /// <param name="relativePath)">The relative path.</param>
    /// <returns>The only names result.</returns>
    public static void OnlyNames(List<string> subfolders)
    {
        for (var i = 0; i < subfolders.Count; i++) subfolders[i] = Path.GetFileName(subfolders[i]);
    }
    /// <summary>
    /// Files which contains all.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The files which contains all result.</returns>
    public static List<string> FilesWhichContainsAll(object sunamo, string searchPattern, params string[] requiredContents)
    {
        return FilesWhichContainsAll(sunamo, searchPattern, requiredContents);
    }
    /// <summary>
    /// Path special and level.
    /// </summary>
    /// <param name="Uri(path).LocalPath">The uri path local path.</param>
    /// <returns>The path special and level result.</returns>
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
                break;
        for (var yValue = 0; yValue < i; yValue++) path.RemoveAt(0);
        var remainingSegments = path.Count - i + basePathC;
        var to = Math.Min(value, remainingSegments);
        i = 0;
        for (; i < to; i++) pBasePath.Add(path[i]);
        return string.Join("\"", pBasePath);
    }
    /// <summary>
    /// Get directory name if is file.
    /// </summary>
    /// <param name="subfolders">The subfolders.</param>
    /// <returns>The get directory name if is file result.</returns>
    public static string GetDirectoryNameIfIsFile(string path)
    {
        if (File.Exists(path)) return Path.GetDirectoryName(path)!;
        return path;
    }
    /// <summary>
    /// Mask from extensions.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <returns>The mask from extensions result.</returns>
    public static string MaskFromExtensions(List<string> allExtensions)
    {
        for (var i = 0; i < allExtensions.Count; i++) allExtensions[i] = "*" + allExtensions[i];
        return string.Join(",", allExtensions);
    }
    /// <summary>
    /// Rename numbered serie files.
    /// </summary>
    /// <param name="sunamo">The sunamo.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="requiredContents">The required contents.</param>
    /// <returns>The rename numbered serie files result.</returns>
    public static void RenameNumberedSerieFiles(ILogger logger, List<string> data, string path, int startFrom, string ext)
    {
        var searchPattern = MascFromExtension(ext);
        var files = FSGetFiles.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        RenameNumberedSerieFiles(logger, data, files, startFrom, ext);
    }
    /// <summary>
    /// Rename numbered serie files.
    /// </summary>
    /// <param name="basePath">The base path.</param>
    /// <param name="string.Empty">The string empty.</param>
    /// <returns>The rename numbered serie files result.</returns>
    public static void RenameNumberedSerieFiles(ILogger logger, List<string> data, List<string> files, int startFrom, string ext)
    {
        var path = Path.GetDirectoryName(files[0]);
        if (files.Count >= data.Count)
        {
            var filesCountMinusOne = files.Count - 1;
            for (var i = startFrom; ; i++)
            {
                if (filesCountMinusOne < i) break;
                var result = files[i];
                var numberedFilePath = path + i + ext;
                if (files.Contains(numberedFilePath))
                    continue;
                // AddSerie is useless coz file never will be exists
                RenameFile(logger, result, numberedFilePath, FileMoveCollisionOption.AddSerie);
            }
        }
    }
    /// <summary>
    /// Place in folder.
    /// </summary>
    /// <param name="File.Exists(path)">The file exists path.</param>
    /// <returns>The place in folder result.</returns>
    public static string PlaceInFolder(string sourcePath, string targetFolder)
    {
        var parentPath = Path.GetDirectoryName(sourcePath);
        var parentFolderName = Path.GetFileName(parentPath);
        return Path.Combine(targetFolder, Path.Combine(parentFolderName!, Path.GetFileName(sourcePath)));
    }
    /// <summary>
    /// Initializes a new instance of the copy move files in list class.
    /// </summary>
    /// <param name="i">The i.</param>
    public static void CopyMoveFilesInList(ILogger logger, List<string> filesFrom, string folderFrom, string folderTo,
        List<string> wasntExistsInFrom, bool mustExistsInTarget, bool copy, Dictionary<string, List<string>> files,
        bool overwrite = true)
    {
        WithoutEndSlash(folderFrom);
        WithoutEndSlash(folderTo);
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
    /// <summary>
    /// Copy move files in list simple.
    /// </summary>
    /// <param name="folderFrom">The folder from.</param>
    /// <returns>The copy move files in list simple result.</returns>
    public static void CopyMoveFilesInListSimple(ILogger logger, List<string> files, string basePathCjHtml1, string basePathCjHtml2,
        bool copy, bool overwrite = true)
    {
        List<string>? wasntExistsInFrom = null;
        var mustExistsInTarget = false;
        CopyMoveFilesInList(logger, files, basePathCjHtml1, basePathCjHtml2, wasntExistsInFrom!, mustExistsInTarget, copy, null!,
            overwrite);
    }
    /// <summary>
    /// Create in other location same folder structure.
    /// </summary>
    /// <param name="!">The !.</param>
    /// <returns>The create in other location same folder structure result.</returns>
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
    /// Copy move from multi location into one.
    /// </summary>
    /// <param name="mustExistsInTarget">The must exists in target.</param>
    /// <returns>The copy move from multi location into one result.</returns>
    public static void CopyMoveFromMultiLocationIntoOne(ILogger logger, List<string> files, string folderFrom, string folderTo)
    {
        var wasntExists = new List<string>();
        var files2 = new Dictionary<string, List<string>>();
        var getFiles = FSGetFiles.GetFiles(folderFrom, "*.cs", SearchOption.AllDirectories,
            new GetFilesArgsFS { ExcludeFromLocationsContains = new List<string>(["TestFiles"]) });
        foreach (var item in files) files2.Add(item, getFiles.Where(data => Path.GetFileName(data) == item).ToList());
        CopyMoveFilesInList(logger, files, folderFrom, folderTo, wasntExists, false, true, files2);
    }
    private static Stream OpenStream(string value)
    {
        return new FileStream(value, FileMode.OpenOrCreate);
    }

    /// <summary>
    /// Delete empty files.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="so">The so.</param>
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
        #region ReplaceInAllFilesArgsBase - Copy also to ReplaceInAllFilesWorker. See comment below
        // I have to split it into individual variables to see what is used and what is not. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
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
    /// <summary>
    /// Replace in all files.
    /// </summary>
    /// <param name="isMultilineWithVariousIndent">The is multiline with various indent.</param>
    /// <returns>The replace in all files result.</returns>
    public static async Task ReplaceInAllFiles(string from, string to, ReplaceInAllFilesArgsBase args,
        Func<string, bool> EncodingHelperIsBinary)
    {
        var result = new ReplaceInAllFilesArgs(args);
        result.From = from;
        result.To = to;
        await ReplaceInAllFilesWorker(result, EncodingHelperIsBinary);
    }
    /// <summary>
    /// Replace in all files.
    /// </summary>
    /// <param name="isMultilineWithVariousIndent">The is multiline with various indent.</param>
    /// <returns>The replace in all files result.</returns>
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
    /// <summary>
    /// Replace in all files.
    /// </summary>
    /// <param name="replaceFrom">The replace from.</param>
    /// <param name="replaceTo">The replace to.</param>
    /// <param name="args">The args.</param>
    /// <param name="EncodingHelperIsBinary">The encoding helper is binary.</param>
    public static
        async Task
        ReplaceInAllFiles(IList<string> replaceFrom, IList<string> replaceTo, ReplaceInAllFilesArgsBase args,
            Func<string, bool> EncodingHelperIsBinary)
    {
        #region ReplaceInAllFilesArgsBase - Copy also to ReplaceInAllFilesWorker. See comment below
        // I have to split it into individual variables to see what is used and what is not. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
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
                }
                // File.ReadAllText is 20x faster than File.ReadAllText
                var content =
await FileAsync.ReadAllTextAsync(item);
                var content2 = string.Empty;
                if (fasterMethodForReplacing == null)
                    for (var i = 0; i < replaceFrom.Count; i++)
                        content2 = content.Replace(replaceFrom[i], replaceTo[i]);
                else
                    content2 = fasterMethodForReplacing.Invoke(new StringBuilder(content), replaceFrom, replaceTo)
                        .ToString();
                if (content != content2)
                {
                    await FileAsync.WriteAllTextAsync(item, content2);
                    if (writeEveryReadedFileAsStatus)
                    {
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get file in startup path.
    /// </summary>
    /// <param name="!inDownloadedFolders">The in downloaded folders.</param>
    /// <returns>The get file in startup path result.</returns>
    public static string GetFileInStartupPath(string value)
    {
        return AppPaths.GetFileInStartupPath(value);
    }
    /// <summary>
    /// Remove diacritic in file contents.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="mask">The mask.</param>
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
    /// <summary>
    /// Initializes a new instance of the remove file class.
    /// </summary>
    public static string RemoveFile(string fullPathCsproj)
    {
        // Most effecient way to handle csproj and dir
        var ext = Path.GetExtension(fullPathCsproj);
        if (ext != string.Empty) fullPathCsproj = Path.GetDirectoryName(fullPathCsproj)!;
        var result = WithoutEndSlash(fullPathCsproj);
        SH.FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Make from last part file.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The make from last part file result.</returns>
    public static string MakeFromLastPartFile(string fullPath, string ext)
    {
        WithoutEndSlash(ref fullPath);
        return fullPath + ext;
    }
    /// <summary>
    /// Get file name without extensions.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The get file name without extensions result.</returns>
    public static string GetFileNameWithoutExtensions(string path)
    {
        while (Path.HasExtension(path)) path = Path.GetFileNameWithoutExtension(path);
        return path;
    }
    /// <summary>
    /// Copy as 0 kb files subfolders.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="mask">The mask.</param>
    /// <param name="SearchOption.AllDirectories">The search option all directories.</param>
    /// <returns>The copy as 0 kb files subfolders result.</returns>
    public static void CopyAs0KbFilesSubfolders
        (string pathDownload, string pathVideos0Kb)
    {
        WithEndSlash(ref pathDownload);
        WithEndSlash(ref pathVideos0Kb);
        var folders = Directory.GetDirectories(pathDownload);
        foreach (var item in folders) CopyAs0KbFiles(item, item.Replace(pathDownload, pathVideos0Kb));
    }
    /// <summary>
    /// Copy as 0 kb files.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="df2.RemoveDiacritics()">The df 2 remove diacritics.</param>
    /// <returns>The copy as 0 kb files result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the shrink long path class.
    /// </summary>
    /// <param name="result">The result.</param>
    public static string ShrinkLongPath(string actualFilePath)
    {
        // .NET 4.7.1
        // Originally - 265 chars, 254 also too long: element:\Documents\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - before removing identical files from duplicate projects\vs\Projects\merge-huge temp\temp1\temp\Facebook.cs
        // 4+265 - OK: @"\\?\D:\_NewlyRecovered\Visual Studio 2020\Projects\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - before removing identical files from duplicate projects\vs\Projects\merge-giant temp\temp1\temp\Facebook.cs"
        // 216 - OK: data:\Recovered data 03-23 12_11_44012345678901234567890123456\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - before removing identical files from duplicate projects\vs\Projects\merge-giant temp\temp1\temp\
        // for many API is different limits: https://stackoverflow.com/questions/265769/maximum-filename-length-in-ntfs-windows-xp-and-windows-vista
        // 237+11 - bad
        return @"\\?\" + actualFilePath;
    }
    /// <summary>
    /// Initializes a new instance of the create new folder path with ending next to class.
    /// </summary>
    /// <param name="Path.HasExtension(path)">The path has extension path.</param>
    public static string CreateNewFolderPathWithEndingNextTo(string folder, string ending)
    {
        var pathToFolder = Path.GetDirectoryName(folder.TrimEnd('\\')) + "\"";
        var folderWithCaretFiles = pathToFolder + Path.GetFileName(folder.TrimEnd('\\')) + ending;
        var result = folderWithCaretFiles;
        SH.FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Copy files of extensions.
    /// </summary>
    /// <param name="pathDownload">The path download.</param>
    /// <returns>The copy files of extensions result.</returns>
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
    /// <summary>
    /// Remove diacritic in file system entry names.
    /// </summary>
    /// <param name="actualFilePath">The actual file path.</param>
    /// <returns>The remove diacritic in file system entry names result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the get up folder which contains extension class.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="realnewpath">The realnewpath.</param>
    public static string? GetUpFolderWhichContainsExtension(string path, string fileExt)
    {
        while (FSGetFiles.FilesOfExtension(path!, fileExt).Count == 0)
        {
            if (path.Length < 4) return null;
            path = Path.GetDirectoryName(path)!;
        }
        return path;
    }
    /// <summary>
    /// Trim content in files of folder.
    /// </summary>
    /// <returns>The trim content in files of folder result.</returns>
    public static void TrimContentInFilesOfFolder(string folder, string searchPattern, SearchOption searchOption)
    {
        var files = FSGetFiles.GetFiles(folder, searchPattern, searchOption);
        foreach (var item in files)
        {
            var fileStream = new FileStream(item, FileMode.Open);
            var streamReader = new StreamReader(fileStream, true);
            var content = streamReader.ReadToEnd();
            var encoding = streamReader.CurrentEncoding;
            streamReader.Dispose();
            streamReader = null;
            var contentTrim = content.Trim();
            File.WriteAllText(item, contentTrim, encoding);
        }
    }
    /// <summary>
    /// Initializes a new instance of the replace in file name class.
    /// </summary>
    /// <param name="File.Exists(realNewPath)">The file exists real new path.</param>
    public static string ReplaceInFileName(string oldPath, string what, string forWhat)
    {
        string path, fileName;
        GetPathAndFileName(oldPath, out path, out fileName);
        var result = path + "\"" + fileName.Replace(what, forWhat);
        SH.FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Get size in.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="fileExt">The file ext.</param>
    /// <returns>The get size in result.</returns>
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
    /// <summary>
    /// Delete all empty directories.
    /// </summary>
    /// <param name="oldPath">The old path.</param>
    /// <param name="what">The what.</param>
    /// <param name="forWhat">The for what.</param>
    /// <returns>The delete all empty directories result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the compare t with int t class.
    /// </summary>
    /// <param name="to">The to.</param>
    public static int CompareTWithInt<T>(TWithInt<T> first, TWithInt<T> second)
    {
        if (first.Count > second.Count)
            return 1;
        if (first.Count < second.Count) return -1;
        return 0;
    }
    /// <summary>
    /// Directories with token.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="fromUnit">The from unit.</param>
    /// <param name="to">The to.</param>
    /// <returns>The directories with token result.</returns>
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
        return result;
    }
    /// <summary>
    /// Move directory no recursive.
    /// </summary>
    /// <param name="!excludePatterns.Any(data">The exclude patterns any data.</param>
    /// <returns>The move directory no recursive result.</returns>
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
                        resultMessage = "Folder has been renamed to" + " " + Path.GetFileName(newFn);
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
    /// <summary>
    /// Move all recursively and then directory.
    /// </summary>
    /// <param name="directoryMoveCollisionOption">The directory move collision option.</param>
    /// <returns>The move all recursively and then directory result.</returns>
    public static void MoveAllRecursivelyAndThenDirectory(ILogger logger, string sourcePath, string targetPath, FileMoveCollisionOption collisionOption)
    {
        CopyMoveAllFilesRecursively(logger, sourcePath, targetPath, collisionOption, true, null!, SearchOption.AllDirectories);
        var dirs = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
        for (var i = dirs.Length - 1; i >= 0; i--) TryDeleteDirectory(dirs[i]);
        TryDeleteDirectory(sourcePath);
    }
    /// <summary>
    /// Move all files recursively.
    /// </summary>
    /// <param name="from">The from.</param>
    /// <param name=""*"">The "*".</param>
    /// <param name="SearchOption.AllDirectories">The search option all directories.</param>
    /// <returns>The move all files recursively result.</returns>
    [Obsolete("Use MoveDirectoryNoRecursive instead")]
    public static void MoveAllFilesRecursively(ILogger logger, string sourcePath, string targetPath, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, sourcePath, targetPath, collisionOption, true, contains!, SearchOption.AllDirectories);
    }
    /// <summary>
    /// Delete files with same content bytes.
    /// </summary>
    /// <param name="from.Length">The from length.</param>
    /// <returns>The delete files with same content bytes result.</returns>
    public static void DeleteFilesWithSameContentBytes(List<string> files)
    {
        DeleteFilesWithSameContentWorking<List<byte>, byte>(files, TF.ReadAllBytesSync);
    }
    /// <summary>
    /// Delete duplicated images.
    /// </summary>
    /// <param name="from">The from.</param>
    /// <param name="to">The to.</param>
    /// <returns>The delete duplicated images result.</returns>
    public static void DeleteDuplicatedImages(List<string> files)
    {
        throw new Exception("Only for test files for another apps" + ". ");
    }
    /// <summary>
    /// Initializes a new instance of the delete files with same content working t content col type class.
    /// </summary>
    /// <param name="Exception">The exception.</param>
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
    /// <summary>
    /// Delete files with same content.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="sourcePath">The source path.</param>
    /// <param name="targetPath">The target path.</param>
    /// <param name="collisionOption">The collision option.</param>
    /// <returns>The delete files with same content result.</returns>
    public static void DeleteFilesWithSameContent(List<string> files)
    {
        DeleteFilesWithSameContentWorking<string, object>(files, File.ReadAllText);
    }
    /// <summary>
    /// Order by natural number serie.
    /// </summary>
    /// <param name="sourcePath">The source path.</param>
    /// <param name=""*"">The "*".</param>
    /// <param name="SearchOption.AllDirectories">The search option all directories.</param>
    /// <returns>The order by natural number serie result.</returns>
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
    /// <summary>
    /// Sort paths by file name.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <returns>The sort paths by file name result.</returns>
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
    /// <summary>
    /// Delete all recursively.
    /// </summary>
    /// <param name="@"\d+").Value">The d value.</param>
    /// <returns>The delete all recursively result.</returns>
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
    /// <summary>
    /// Delete folders which not contains.
    /// </summary>
    /// <typeparam name="string">The type string.</typeparam>
    /// <param name="list.Count">The list count.</param>
    /// <returns>The delete folders which not contains result.</returns>
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
    /// <summary>
    /// Delete all recursively and then directory.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The delete all recursively and then directory result.</returns>
    public static void DeleteAllRecursivelyAndThenDirectory(string path)
    {
        DeleteAllRecursively(path, true);
    }
    /// <summary>
    /// Only extensions.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="rootDirectoryToo">The root directory too.</param>
    /// <returns>The only extensions result.</returns>
    public static List<string> OnlyExtensions(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        for (var i = 0; i < result.Count; i++) result[i] = Path.GetExtension(paths[i]);
        return result;
    }
    /// <summary>
    /// Initializes a new instance of the get dictionary by extension class.
    /// </summary>
    /// <param name="files">The files.</param>
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
    /// <summary>
    /// Only extensions to lower.
    /// </summary>
    /// <param name="folders">The folders.</param>
    /// <returns>The only extensions to lower result.</returns>
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
    /// <summary>
    /// Only extensions to lower with path.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <returns>The only extensions to lower with path result.</returns>
    public static List<string> OnlyExtensionsToLowerWithPath(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        for (var i = 0; i < result.Count; i++) result[i] = OnlyExtensionToLowerWithPath(paths[i]);
        return result;
    }
    /// <summary>
    /// Only extension to lower with path.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="mask">The mask.</param>
    /// <param name="searchOption">The search option.</param>
    /// <returns>The only extension to lower with path result.</returns>
    public static string OnlyExtensionToLowerWithPath(string data)
    {
        string path, fn, ext;
        GetPathAndFileName(data, out path, out fn, out ext);
        var result = path + fn + ext.ToLower();
        return result;
    }
    /// <summary>
    /// All extensions in folders.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The all extensions in folders result.</returns>
    public static List<string> AllExtensionsInFolders(SearchOption so, params string[] folders)
    {
        ThrowEx.NoPassedFolders(folders);
        List<string> filesFull = FSGetFiles.AllFilesInFolders(folders.ToList(), new List<string>(["*"]), so);
        return AllExtensionsInFolders(filesFull);
    }


    /// <summary>
    /// All extensions in folders.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <param name="args">The args.</param>
    /// <returns>The all extensions in folders result.</returns>
    public static List<string> AllExtensionsInFolders(List<string> filesFull, GetExtensionArgs? args = null)
    {
        var result = new List<string>();
        var files = new List<string>(OnlyExtensionsToLower(filesFull, args));
        foreach (var item in files)
            if (!result.Contains(item))
                result.Add(item);
        return result;
    }
    /// <summary>
    /// Expand environment variables.
    /// </summary>
    /// <param name="paths[i]">The paths i.</param>
    /// <returns>The expand environment variables result.</returns>
    public static string ExpandEnvironmentVariables(EnvironmentVariables environmentVariable)
    {
        return Environment.ExpandEnvironmentVariables(SH.WrapWith(environmentVariable.ToString(), "%"));
    }
    /// <summary>
    /// Get file name without extension lower.
    /// </summary>
    /// <typeparam name="string">The type string.</typeparam>
    /// <param name="paths.Count">The paths count.</param>
    /// <returns>The get file name without extension lower result.</returns>
    public static string GetFileNameWithoutExtensionLower(string text)
    {
        return GetFileNameWithoutExtension(text).ToLower();
    }
    /// <summary>
    /// Add upfolders to relative path.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The add upfolders to relative path result.</returns>
    public static string AddUpfoldersToRelativePath(int i2, string file, char delimiter)
    {
        var jumpUp = ".." + delimiter;
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < i2; i++) stringBuilder.Append(jumpUp);
        stringBuilder.Append(file);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// Initializes a new instance of the normalize extension class.
    /// </summary>
    /// <param name="folders">The folders.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension(string extension)
    {
        return "." + extension.TrimStart('.');
    }
    /// <summary>
    /// Get normalized extension.
    /// </summary>
    /// <param name="filesFull">The files full.</param>
    /// <param name="args">The args.</param>
    /// <returns>The get normalized extension result.</returns>
    public static string GetNormalizedExtension(string filename)
    {
        return NormalizeExtension(filename);
    }
    /// <summary>
    /// Modifiedin unix.
    /// </summary>
    /// <typeparam name="string">The type string.</typeparam>
    /// <returns>The modifiedin unix result.</returns>
    public static long ModifiedinUnix(string filePath)
    {
        return (long)File.GetLastWriteTimeUtc(filePath).Subtract(DTConstants.UnixFsStart).TotalSeconds;
    }
    /// <summary>
    /// Initializes a new instance of the replace diacritic recursive class.
    /// </summary>
    /// <param name="item">The item.</param>
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
    /// <summary>
    /// Rename file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="folder">The folder.</param>
    /// <param name="dirs">The dirs.</param>
    /// <param name="files">The files.</param>
    /// <param name="directoryCollisionOption">The directory collision option.</param>
    /// <param name="fileCollisionOption">The file collision option.</param>
    /// <returns>The rename file result.</returns>
    public static void RenameFile(ILogger logger, string oldPath, string newFileNameWithoutPath, FileMoveCollisionOption collisionOption)
    {
        var to = ChangeFilename(oldPath, newFileNameWithoutPath, false);
        MoveFile(logger, oldPath, to, collisionOption);
    }
    /// <summary>
    /// Rename directory.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="AscDesc.Desc">The asc desc desc.</param>
    /// <returns>The rename directory result.</returns>
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
    /// <summary>
    /// Normalize extensions.
    /// </summary>
    /// <param name="files">The files.</param>
    /// <returns>The normalize extensions result.</returns>
    public static void NormalizeExtensions(List<string> extension)
    {
        for (var i = 0; i < extension.Count; i++) extension[i] = NormalizeExtension(extension[i]);
    }


    /// <summary>
    /// Initializes a new instance of the get file name without extension and extension class.
    /// </summary>
    /// <param name="files2">The files 2.</param>
    public static void GetFileNameWithoutExtensionAndExtension(string filePath, out string file, out string ext)
    {
        file = Path.GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(file);
    }
    /// <summary>
    /// Save stream.
    /// </summary>
    /// <returns>The save stream result.</returns>
    public static void SaveStream(string path, Stream text)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            CopyStream(text, fileStream);
            fileStream.Flush();
        }
    }
    /// <summary>
    /// Only names without extension copy.
    /// </summary>
    /// <param name="oldPath">The old path.</param>
    /// <param name="newFileNameWithoutPath">The new file name without path.</param>
    /// <param name="false">The false.</param>
    /// <returns>The only names without extension copy result.</returns>
    public static List<string> OnlyNamesWithoutExtensionCopy(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        for (var i = 0; i < paths.Count; i++) result.Add(Path.GetFileNameWithoutExtension(paths[i]));
        return result;
    }
    /// <summary>
    /// Directory exists and is not empty.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The directory exists and is not empty result.</returns>
    public static bool DirectoryExistsAndIsNotEmpty(string value)
    {
        if (Directory.Exists(value) && Directory.GetFiles(value, "*", SearchOption.AllDirectories).Length != 0) return true;
        return false;
    }
    /// <summary>
    /// Only names without extension.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="path">The path.</param>
    /// <param name="newPath">The new path.</param>
    /// <param name="directoryCollisionOption">The directory collision option.</param>
    /// <param name="fileCollisionOption">The file collision option.</param>
    /// <returns>The only names without extension result.</returns>
    public static List<string> OnlyNamesWithoutExtension(string appendToStart, List<string> fullPaths)
    {
        var result = new List<string>(fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++)
            result.Add(appendToStart + Path.GetFileNameWithoutExtension(fullPaths[i]));
        return result;
    }
    /// <summary>
    /// Postfix.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="file">The file.</param>
    /// <param name="ext">The ext.</param>
    public static string Postfix(string aPath, string text)
    {
        var result = aPath.TrimEnd('\\') + text;
        WithEndSlash(ref result);
        return result;
    }
    /// <summary>
    /// Read all text.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The read all text result.</returns>
    public static
        async Task<string>
        ReadAllText(string path)
    {
        return
await FileAsync.ReadAllTextAsync(path);
    }
    /// <summary>
    /// Get file name without extension.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <returns>The get file name without extension result.</returns>
    public static string GetFileNameWithoutExtension(string text)
    {
        return PathMs.GetFileNameWithoutExtension(text.TrimEnd(PathMs.DirectorySeparatorChar));
    }

    /// <summary>
    /// Initializes a new instance of the get file name without extension no ac storage file class.
    /// </summary>
    /// <param name="i">The i.</param>
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
    //AbstractCatalogBase<StorageFolder, StorageFile> ac)
    /// <summary>
    /// Throw not implemented uwp.
    /// </summary>
    /// <param name="aPath">The a path.</param>
    /// <param name="text">The text.</param>
    /// <returns>The throw not implemented uwp result.</returns>
    public static void ThrowNotImplementedUwp()
    {
        throw new Exception("Not implemented in UWP");
    }
    /// <summary>
    /// Is file older than x hours.
    /// </summary>
    /// <param name="'\\'">The '\\'.</param>
    /// <returns>The is file older than x hours result.</returns>
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
    /// <summary>
    /// Get file names wo extension.
    /// </summary>
    /// <typeparam name="StorageFile">The type StorageFile.</typeparam>
    /// <param name="text">The text.</param>
    /// <returns>The get file names wo extension result.</returns>
    public static List<string> GetFileNamesWoExtension(List<string> jpgcka)
    {
        var result = new List<string>(jpgcka.Count);
        for (var i = 0; i < jpgcka.Count; i++) result.Add(Path.GetFileNameWithoutExtension(jpgcka[i]));
        return result;
    }
    /// <summary>
    /// Get temp file path.
    /// </summary>
    /// <returns>The get temp file path result.</returns>
    public static string GetTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
    }
    /// <summary>
    /// Initializes a new instance of the copy to class.
    /// </summary>
    /// <param name="dynamic">The dynamic.</param>
    public static void CopyTo(string value, string targetDirectory, FileMoveCollisionOption collisionOption)
    {
        var fileTo = Path.Combine(targetDirectory, Path.GetFileName(value));
        CopyFile(value, fileTo, collisionOption);
    }

    /// <summary>
    /// Replace directory throw exception if from doesnt exists.
    /// </summary>
    /// <param name="UWP"">The UWP.</param>
    /// <returns>The replace directory throw exception if from doesnt exists result.</returns>
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

    /// <summary>
    /// Initializes a new instance of the only names without extension class.
    /// </summary>
    /// <param name="-1)">The 1.</param>
    public static List<string> OnlyNamesWithoutExtension(List<string> path)
    {
        for (var i = 0; i < path.Count; i++) path[i] = Path.GetFileNameWithoutExtension(path[i]);
        return path;
    }

    /// <summary>
    /// Initializes a new instance of the get path and file name class.
    /// </summary>
    /// <param name="i">The i.</param>
    public static void GetPathAndFileName(string filePath, out string path, out string file, out string ext)
    {
        path = WithEndSlash(Path.GetDirectoryName(filePath)!);
        file = Path.GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(filePath);
    }
    /// <summary>
    /// Get absolute path 2.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetDirectory">The target directory.</param>
    /// <param name="collisionOption">The collision option.</param>
    /// <returns>The get absolute path 2 result.</returns>
    public static string GetAbsolutePath2(string relativePath, string dir)
    {
        var ap = GetAbsolutePath(dir, relativePath);
        return Path.GetFullPath(ap);
    }
    /// <summary>
    /// Get absolute path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="folderWithProjectsFolders">The folder with projects folders.</param>
    /// <param name="folderWithTemporaryMovedContentWithoutBackslash">The folder with temporary moved content without backslash.</param>
    /// <returns>The get absolute path result.</returns>
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
    /// <summary>
    /// Get tokens.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <param name="dir">The dir.</param>
    /// <returns>The get tokens result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the copy stream class.
    /// </summary>
    /// <param name="true">The true.</param>
    public static void CopyStream(Stream input, Stream output)
    {
        var buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0) output.Write(buffer, 0, len);
    }

    /// <summary>
    /// Combine without first char upper.
    /// </summary>
    /// <param name="parentDirectoryPrefix.Length">The parent directory prefix length.</param>
    /// <returns>The combine without first char upper result.</returns>
    public static string CombineWithoutFirstCharUpper(params string[] text)
    {
        return CombineWorker(false, true, text);
    }
    /// <summary>
    /// Save memory stream.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>The save memory stream result.</returns>
    public static void SaveMemoryStream(MemoryStream mss, string path)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            var matriz = mss.ToArray();
            fileStream.Write(matriz, 0, matriz.Length);
        }
    }
    /// <summary>
    /// Delete wrong chars in directory name.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
    /// <returns>The delete wrong chars in directory name result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the delete wrong chars in file name class.
    /// </summary>
    /// <param name="relativePath">The relative path.</param>
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
    /// <summary>
    /// Contains invalid path char for part of map path.
    /// </summary>
    /// <param name="mss">The mss.</param>
    /// <param name="path">The path.</param>
    public static bool ContainsInvalidPathCharForPartOfMapPath(string path)
    {
        foreach (var item in invalidCharsForMapPath)
            if (path.IndexOf(item) != -1)
                return true;
        return false;
    }
    /// <summary>
    /// Delete file if exists.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The delete file if exists result.</returns>
    public static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path)) File.Delete(path);
    }
    /// <summary>
    /// Initializes a new instance of the only names no direct edit class.
    /// </summary>
    /// <param name="path">The path.</param>
    public static List<string> OnlyNamesNoDirectEdit(string[] files2)
    {
        var tl = files2.ToList();
        return OnlyNamesNoDirectEdit(tl);
    }

    /// <summary>
    /// Only names no direct edit.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="isPath">The is path.</param>
    /// <returns>The only names no direct edit result.</returns>
    public static List<string> OnlyNamesNoDirectEdit(List<string> files2)
    {
        var files = new List<string>(files2.Count);
        for (var i = 0; i < files2.Count; i++) files.Add(Path.GetFileName(files2[i]));
        return files;
    }
    /// <summary>
    /// Only names no direct edit.
    /// </summary>
    /// <returns>The only names no direct edit result.</returns>
    public static List<string> OnlyNamesNoDirectEdit(string appendToStart, List<string> fullPaths)
    {
        var result = new List<string>(fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++) result.Add(appendToStart + Path.GetFileName(fullPaths[i]));
        return result;
    }



    /// <summary>
    /// Change extension.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The change extension result.</returns>
    public static string ChangeExtension(string filePath, string newExt, bool physically)
    {
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
    /// <summary>
    /// Create directory.
    /// </summary>
    /// <param name="files2">The files 2.</param>
    /// <returns>The create directory result.</returns>
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
    /// <summary>
    /// Stream to array bytes.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="false">The false.</param>
    /// <param name="hasSerie">The has serie.</param>
    /// <param name="serieStyle">The serie style.</param>
    /// <returns>The stream to array bytes result.</returns>
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
    /// <summary>
    /// Add extension if dont have.
    /// </summary>
    /// <param name="(bytesRead">The bytes read.</param>
    /// <returns>The add extension if dont have result.</returns>
    public static string AddExtensionIfDontHave(string file, string ext)
    {
        // For *.* and git paths {dir}/*
        if (file[file.Length - 1] == '*') return file;
        if (Path.GetExtension(file) == string.Empty) return file + ext;
        return file;
    }
    /// <summary>
    /// Initializes a new instance of the insert between file name and extension remove path class.
    /// </summary>
    /// <param name="totalBytesRead">The total bytes read.</param>
    public static string InsertBetweenFileNameAndExtensionRemovePath(string orig, string whatInsert)
    {
        var fn = Path.GetFileNameWithoutExtension(orig);
        var element = Path.GetExtension(orig);
        return Path.Combine(fn + whatInsert + element);
    }


    /// <summary>
    /// Get dictionary by file name with extension.
    /// </summary>
    /// <param name="!">The !.</param>
    /// <returns>The get dictionary by file name with extension result.</returns>
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
    /// <summary>
    /// Change filename.
    /// </summary>
    /// <param name="stream.CanSeek">The stream can seek.</param>
    /// <returns>The change filename result.</returns>
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








    /// <summary>
    /// Slash.
    /// </summary>
    /// <param name="files">The files.</param>
    /// <returns>The slash result.</returns>
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
    /// <summary>
    /// Try delete with repetition.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>The try delete with repetition result.</returns>
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
    /// <summary>
    /// Try delete file.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="slash">The slash.</param>
    /// <returns>The try delete file result.</returns>
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
    /// <summary>
    /// Get size in auto string.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name=""\""">The "\"".</param>
    /// <param name=""">The ".</param>
    /// <returns>The get size in auto string result.</returns>
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
    /// <summary>
    /// Get size in auto string.
    /// </summary>
    /// <param name="ex">The ex.</param>
    /// <returns>The get size in auto string result.</returns>
    public static string GetSizeInAutoString(long value, ComputerSizeUnits fromUnit)
    {
        return GetSizeInAutoString((double)value, fromUnit);
    }

    /// <summary>
    /// Get size in auto string.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>The get size in auto string result.</returns>
    public static string GetSizeInAutoString(double value, ComputerSizeUnits fromUnit)
    {
        if (fromUnit != ComputerSizeUnits.B)
            // I get the value in bytes
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
    /// <summary>
    /// Initializes a new instance of the repair filter class.
    /// </summary>
    /// <param name="1024">The 1024.</param>
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
    /// Initializes a new instance of the replace incorrect characters file class.
    /// </summary>
    /// <param name="1">The 1.</param>
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
    /// <summary>
    /// Initializes a new instance of the replace incorrect characters file class.
    /// </summary>
    /// <param name="to">The to.</param>
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
    /// <summary>
    /// Replace incorrect characters file.
    /// </summary>
    /// <param name="InvalidFileNameChars">The invalid file name chars.</param>
    /// <returns>The replace incorrect characters file result.</returns>
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
    /// <summary>
    /// Insert between file name and path.
    /// </summary>
    /// <param name="replaceForThis">The replace for this.</param>
    /// <returns>The insert between file name and path result.</returns>
    public static string InsertBetweenFileNameAndPath(string folder, string parentFolder, string insert)
    {
        ThrowEx.IsNotWindowsPathFormat(nameof(folder), folder, true, FS.IsWindowsPathFormat);
        if (parentFolder == null) parentFolder = Path.GetDirectoryName(folder)!;
        var outputFolder = Path.Combine(parentFolder, insert);
        CreateFoldersPsysicallyUnlessThere(outputFolder);
        return Path.Combine(outputFolder, Path.GetFileName(folder));
    }
    /// <summary>
    /// Change directory.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="replaceAllOfThisThen">The replace all of this then.</param>
    /// <returns>The change directory result.</returns>
    public static string ChangeDirectory(string fileName, string changeFolderTo)
    {
        var path = Path.GetDirectoryName(fileName);
        var fn = Path.GetFileName(fileName);
        return Path.Combine(changeFolderTo, fn);
    }
    /// <summary>
    /// Initializes a new instance of the directory listing class.
    /// </summary>
    /// <param name="InvalidFileNameChars">The invalid file name chars.</param>
    public static List<string> DirectoryListing(string path, string mask, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(path, mask, so, new GetFilesArgsFS { TrimFirstPathAndLeadingBackslashes = true });
        return files;
    }
    /// <summary>
    /// Initializes a new instance of the without end slash class.
    /// </summary>
    /// <param name="item2">The item 2.</param>
    public static string WithoutEndSlash(string value)
    {
        return WithoutEndSlash(ref value);
    }
    /// <summary>
    /// Without end slash.
    /// </summary>
    /// <param name="!string.IsNullOrEmpty(replaceAllOfThisThen)">The string is null or empty replace all of this then.</param>
    /// <returns>The without end slash result.</returns>
    public static string WithoutEndSlash(ref string value)
    {
        value = value.TrimEnd('\\');
        return value;
    }
    /// <summary>
    /// Masc from extension.
    /// </summary>
    /// <param name=""">The ".</param>
    /// <param name="replaceFor">The replace for.</param>
    /// <returns>The masc from extension result.</returns>
    public static string MascFromExtension(string ext2 = "*")
    {
        if (char.IsLetterOrDigit(ext2[0]))
            // For wildcard, dot (simply non letters) include .
            ext2 = "." + ext2;
        if (!ext2.StartsWith("*")) ext2 = "*" + ext2;
        if (!ext2.StartsWith("*.") && ext2.StartsWith(".")) ext2 = "*." + ext2;
        return ext2;
        //    // Dont understand why, when I insert .aspx.cs, then return just .aspx without *
    }
    /// <summary>
    /// Initializes a new instance of the is count of files more than class.
    /// </summary>
    /// <param name="outputFolder">The output folder.</param>
    public static bool IsCountOfFilesMoreThan(string folderPath, string searchPattern, int getNullIfThereIsMoreThanXFiles)
    {
        var files = FSGetFiles.GetFilesEveryFolder(folderPath, searchPattern, SearchOption.AllDirectories,
            new GetFilesEveryFolderArgsFS { GetNullIfThereIsMoreThanXFiles = getNullIfThereIsMoreThanXFiles });
        return files == null;
    }
    /// <summary>
    /// Get files.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <returns>The get files result.</returns>
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
    //        await
    //#endif

    /// <summary>
    /// Move file.
    /// </summary>
    /// <param name="'\\'">The '\\'.</param>
    /// <returns>The move file result.</returns>
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
    /// <summary>
    /// Copy move file prepare.
    /// </summary>
    /// <param name="folderPath">The folder path.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="SearchOption.AllDirectories">The search option all directories.</param>
    /// <param name="GetNullIfThereIsMoreThanXFiles">The get null if there is more than x files.</param>
    /// <returns>The copy move file prepare result.</returns>
    public static bool CopyMoveFilePrepare(ref string sourceFilePath, ref string fileTo, FileMoveCollisionOption collisionOption)
    {
        sourceFilePath = @"\\?\" + sourceFilePath;
        MakeUncLongPath(ref fileTo);
        CreateUpfoldersPsysicallyUnlessThere(fileTo);
        // This is important here, I do not know who commented it out
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
    /// <summary>
    /// Get file size.
    /// </summary>
    /// <param name="!">The !.</param>
    /// <returns>The get file size result.</returns>
    public static long GetFileSize(string filePath)
    {
        FileInfo? fi = null;
        try
        {
            fi = new FileInfo(filePath);
        }
        catch (Exception)
        {
            // For example too long a file name
            return 0;
        }
        if (fi.Exists) return fi.Length;
        return 0;
    }
    /// <summary>
    /// Copy all files recursively.
    /// </summary>
    /// <param name="sourceFilePath">The source file path.</param>
    /// <returns>The copy all files recursively result.</returns>
    public static void CopyAllFilesRecursively(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, path, to, collisionOption, false, contains!, SearchOption.AllDirectories);
    }

    /// <summary>
    /// Initializes a new instance of the copy all files class.
    /// </summary>
    /// <param name="sourceFilePath">The source file path.</param>
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
    /// <summary>
    /// Copy file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="sourceFilePath">The source file path.</param>
    /// <param name="fileTo2">The file to 2.</param>
    /// <param name="collisionOption">The collision option.</param>
    /// <param name="terminateProcessIfIsInUsed">The terminate process if is in used.</param>
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

    /// <summary>
    /// Initializes a new instance of the copy file class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="path">The path.</param>
    /// <param name="to">The to.</param>
    /// <param name="collisionOption">The collision option.</param>
    /// <param name="move">The move.</param>
    /// <param name="item">The item.</param>
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
                // It is also used in the web, I would have to include a lot of methods there
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
    /// <summary>
    /// Copy file.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="jsFiles">The js files.</param>
    /// <param name="value">The value.</param>
    /// <param name="terminateProcessIfIsInUsed">The terminate process if is in used.</param>
    /// <returns>The copy file result.</returns>
    public static void CopyFile(string sourceFilePath, string fileTo2, FileMoveCollisionOption collisionOption)
    {
        var fileTo = fileTo2;
        if (CopyMoveFilePrepare(ref sourceFilePath, ref fileTo, collisionOption))
        {
            if (collisionOption == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo)) return;
            File.Copy(sourceFilePath, fileTo);
        }
    }
    /// <summary>
    /// Last modified.
    /// </summary>
    /// <param name="terminateProcessIfIsInUsed">The terminate process if is in used.</param>
    /// <returns>The last modified result.</returns>
    public static DateTime LastModified(string rel)
    {
        if (File.Exists(rel)) return File.GetLastWriteTime(rel);
        // FileInfo held the file and errors occurred: The process cannot access the file
        return DateTime.MinValue;
    }
    /// <summary>
    /// Initializes a new instance of the try delete directory or file class.
    /// </summary>
    /// <param name="pr">The pr.</param>
    public static bool TryDeleteDirectoryOrFile(string value)
    {
        if (!TryDeleteDirectory(value)) return TryDeleteFile(value);
        return true;
    }
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

    /// <summary>
    /// Try delete directory.
    /// </summary>
    /// <returns>The try delete directory result.</returns>
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
            // It is a try so I do not know what this and a do here
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
    /// <summary>
    /// Initializes a new instance of the all include if only letters class.
    /// </summary>
    /// <param name="rootDi.GetFiles()">The root di get files.</param>
    public static string AllIncludeIfOnlyLetters(string extension)
    {
        extension = extension.ToLower().TrimStart('*').TrimStart('.');

        if (extension == "")
        {
            extension = "*";
        }

        extension = "*." + extension;
        return extension;
    }
    /// <summary>
    /// Get file serie.
    /// </summary>
    /// <param name="psi">The psi.</param>
    /// <returns>The get file serie result.</returns>
    public static string? GetFileSerie(string fnwoe, SerieStyleFS ss)
    {
        if (ss == SerieStyleFS.Brackets)
        {
            return SHParts.GetTextBetweenTwoChars(fnwoe, '(', ')');
        }
        ThrowEx.NotImplementedMethod();
        return null;
    }

    /// <summary>
    /// Get file series.
    /// </summary>
    /// <param name="psi">The psi.</param>
    /// <returns>The get file series result.</returns>
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

    /// <summary>
    /// Exists directory null.
    /// </summary>
    /// <param name="fnwoe">The fnwoe.</param>
    /// <param name="ss">The ss.</param>
    /// <returns>The exists directory null result.</returns>
    public static bool? ExistsDirectoryNull(string directoryPath)
    {
        return ExistsDirectoryNull(directoryPath, false);
    }
    /// <summary>
    /// Exists directory null.
    /// </summary>
    /// <param name="ss">The ss.</param>
    /// <returns>The exists directory null result.</returns>
    public static bool? ExistsDirectoryNull(string directoryPath, bool isFalseIfContainsNoFile = false)
    {
        return ExistsDirectory(directoryPath, isFalseIfContainsNoFile);
    }
    /// <summary>
    /// Exists directory.
    /// </summary>
    /// <returns>The exists directory result.</returns>
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
    }
    #region For easy copy
    #endregion
    #region Making problem in translate
    /// <summary>
    /// Delete serie directory or create new.
    /// </summary>
    /// <param name="Path.GetFileName(item)">The path get file name item.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="""">The "".</param>
    /// <returns>The delete serie directory or create new result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the to search option class.
    /// </summary>
    /// <param name="repairedBlogPostsFolder">The repaired blog posts folder.</param>
    /// <param name="true">The true.</param>
    public static SearchOption ToSearchOption(bool? recursive)
    {
        return recursive.GetValueOrDefault() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    }
    /// <summary>
    /// Initializes a new instance of the write all text class.
    /// </summary>
    /// <param name="repairedBlogPostsFolder">The repaired blog posts folder.</param>
    /// <param name="ex">The ex.</param>
    public static async Task WriteAllText(string path, string content)
    {
        await FileAsync.WriteAllTextAsync(path, content);
    }
    /// <summary>
    /// Initializes a new instance of the is all in same folder class.
    /// </summary>
    /// <param name="!deleted">The deleted.</param>
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
    /// <summary>
    /// Initializes a new instance of the create file with template content class.
    /// </summary>
    /// <param name="resultSerie++">The result serie.</param>
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
    /// <summary>
    /// Contains invalid file name chars.
    /// </summary>
    /// <param name="recursive">The recursive.</param>
    /// <returns>The contains invalid file name chars result.</returns>
    public static bool ContainsInvalidFileNameChars(string arg)
    {
        foreach (var item in invalidFileNameStringsReadonly)
            if (arg.Contains(item))
                return true;
        return false;
    }
    /// <summary>
    /// Number by date modified.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="content">The content.</param>
    /// <returns>The number by date modified result.</returns>
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

    /// <summary>
    /// Get directory name.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="files">The files.</param>
    /// <param name="ext">The ext.</param>
    /// <param name="templateFromContent">The template from content.</param>
    /// <returns>The get directory name result.</returns>
    public static string GetDirectoryName(string path)
    {
        // I also throw exceptions here
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
    /// <summary>
    /// Initializes a new instance of the detect path delimiter class.
    /// </summary>
    /// <param name="arg.Contains(item)">The arg contains item.</param>
    public static Tuple<bool, bool> DetectPathDelimiter(string path)
    {
        var containsFs = path.Contains("/");
        var containsBs = path.Contains("\\");
        if (containsBs && containsFs) throw new Exception("Path contains both fs & bs");
        return Tuple.Create(containsFs, containsBs);
    }
    /// <summary>
    /// Detect path delimiter char.
    /// </summary>
    /// <param name="files">The files.</param>
    /// <returns>The detect path delimiter char result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the is windows path format class.
    /// </summary>
    /// <param name="string.IsNullOrEmpty(path)">The string is null or empty path.</param>
    public static bool IsWindowsPathFormat(string argValue)
    {
        PathFormatDetectorService pathFormatDetector = new(NullLogger.Instance);
        return pathFormatDetector.IsWindowsPathFormat(argValue);
    }
    #endregion
    #region MakeUncLongPath
    /// <summary>
    /// Initializes a new instance of the make unc long path class.
    /// </summary>
    /// <param name="result">The result.</param>
    public static string MakeUncLongPath(string path)
    {
        return MakeUncLongPath(ref path);
    }
    /// <summary>
    /// Make unc long path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The make unc long path result.</returns>
    public static string MakeUncLongPath(ref string path)
    {
        if (!path.StartsWith(@"\\?\"))
        {
            // value ASP.net returned directory.exists false for every directory. I was under ApplicationPoolIdentity value IIS and Full Control was set for IIS AppPool\DefaultAppPool
        }
        return path;
    }
    #endregion
    #region MakeUncLongPath
    #endregion
    #region from FSShared64.cs
    //        /// <summary>
    //        /// Convert to UNC path
    //        /// </summary>
    //            // Not working, flags from GeoCachingTool wasnt transfered to standard
    //#if NETFX_CORE
    //#endif
    //#if WINDOWS_UWP
    //#endif
    //            // Directory.Exists if pass SE or only start of Unc return false
    #region FirstCharUpper
    /// <summary>
    /// First char upper.
    /// </summary>
    /// <param name="argValue">The arg value.</param>
    /// <returns>The first char upper result.</returns>
    public static string FirstCharUpper(ref string result)
    {
        if (IsWindowsPathFormat(result)) result = SH.FirstCharUpper(result);
        return result;
    }
    /// <summary>
    /// First char upper.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The first char upper result.</returns>
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

    /// <summary>
    /// Get path and file name.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="path">The path.</param>
    /// <param name="file">The file.</param>
    public static void GetPathAndFileName(string filePath, out string path, out string file)
    {
        path = WithEndSlash(GetDirectoryName(filePath));
        file = GetFileName(filePath);
    }
    /// <summary>
    /// Get file name.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The get file name result.</returns>
    public static string GetFileName(string filePath)
    {
        return PathMs.GetFileName(filePath.TrimEnd(Path.DirectorySeparatorChar));
    }
    /// <summary>
    /// Normalize extension 2.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The normalize extension 2 result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension2(string extension)
    {
        return extension.ToLower().TrimStart('.');
    }
    //        #region
    //        #endregion
    //                // I get the value in bytes
    //        /// <summary>
    //        /// Create all upfolders of A1 with, if they dont exist
    //        /// </summary>
    //                    // TODO: This does not work here for UWP/UAP apps because they have no access to the whole disk. Find out what UWP/UAP/... is and how to get/verify any folder on disk
    //        #region MyRegion
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
    //#endif
    //                    // I only need to check it when it is tmp, logically
    //                            // I had a bug where, although exists was true, File.ReadAllText failed because the file did not exist.
    //                            // I solved it by checking the extension, hopefully
    //    // Cant return with end slash becuase is working also with files





    //
    //



    //
    //        /// <summary>
    //        /// Returns the path and the file name without ext and the ext
    //        /// All returned is normal case
    //        /// </summary>
    //        /// <param name="fn"></param>
    //        /// <param name="path"></param>
    //        /// <param name="file"></param>
    //
    //        #endregion
    #endregion
    #region GetFilesMoreMasc - in thread
    #endregion
    /// <summary>
    /// Files with same name.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="searchOption">The search option.</param>
    /// <returns>The files with same name result.</returns>
    public static string FilesWithSameName(string folder, string searchPattern, SearchOption searchOption)
    {
        WithEndSlash(ref folder);
        var filesByName = new Dictionary<string, List<string>>();
        var text = FSGetFiles.GetFiles(folder, searchPattern, searchOption);
        foreach (var item in text) DictionaryHelper.AddOrCreate(filesByName, Path.GetFileName(item), item);
        var stringBuilder = new StringBuilder();
        foreach (var item in filesByName)
            if (item.Value.Count > 1)
            {
                foreach (var item2 in item.Value) stringBuilder.AppendLine(item2);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
            }
        return stringBuilder.ToString();
    }
    #region For easy copy - GetNameWithoutSeries
    /// <summary>
    /// Get name without series.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="a1IsWithPath">The a 1 is with path.</param>
    /// <returns>The get name without series result.</returns>
    public static string GetNameWithoutSeries(string path, bool a1IsWithPath)
    {
        int serie;
        var hasSerie = false;
        return GetNameWithoutSeries(path, a1IsWithPath, out hasSerie, SerieStyleFS.Brackets, out serie);
    }

    /// <summary>
    /// Get name without series no out.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="searchOption">The search option.</param>
    /// <returns>The get name without series no out result.</returns>
    public static (string, bool) GetNameWithoutSeriesNoOut(string path, bool a1IsWithPath, SerieStyleFS serieStyle)
    {
        int serie;
        var result = GetNameWithoutSeries(path, a1IsWithPath, out var hasSerie, serieStyle, out serie);
        return (result, hasSerie);
    }
    /// <summary>
    /// Get name without series.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <param name="searchPattern">The search pattern.</param>
    /// <param name="searchOption">The search option.</param>
    /// <returns>The get name without series result.</returns>
    public static string GetNameWithoutSeries(string path, bool a1IsWithPath, out bool hasSerie, SerieStyleFS serieStyle)
    {
        int serie;
        return GetNameWithoutSeries(path, a1IsWithPath, out hasSerie, serieStyle, out serie);
    }

    /// <summary>
    /// Get name without series.
    /// </summary>
    /// <returns>The get name without series result.</returns>
    public static string GetNameWithoutSeries(string path, bool a1IsWithPath, out bool hasSerie, SerieStyleFS serieStyle,
        out int serie)
    {
        serie = -1;
        hasSerie = false;
        var directory = string.Empty;
        if (a1IsWithPath) directory = WithEndSlash(Path.GetDirectoryName(path)!);
        var sbExt = new StringBuilder();
        var ext = Path.GetExtension(path);
        var seriesCount = 0;
        path = SHParts.RemoveAfterLast(path, ".");
        sbExt.Append(ext);
        ext = sbExt.ToString();
        var fullPath = path;
        if (directory.Length != 0)
        {
            fullPath = fullPath.Substring(directory.Length);
        }
        // First I trim all extensions, even if the file has multiple extensions
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
    /// <summary>
    /// Remove serie underscore.
    /// </summary>
    /// <param name="serieStyle">The serie style.</param>
    /// <returns>The remove serie underscore result.</returns>
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
    /// <summary>
    /// Initializes a new instance of the delete file class.
    /// </summary>
    /// <param name="serieStyle">The serie style.</param>
    public static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }

    /// <summary>
    /// Get path and file name without extension.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>The get path and file name without extension result.</returns>
    public static void GetPathAndFileNameWithoutExtension(string filePath, out string path, out string file, out string ext)
    {
        path = Path.GetDirectoryName(filePath) + '\\';
        file = GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(filePath);
    }
    /// <summary>
    /// Initializes a new instance of the path without extension class.
    /// </summary>
    /// <param name="serie">The serie.</param>
    /// <param name="data">The data.</param>
    /// <param name="seriesCount">The series count.</param>
    public static string PathWithoutExtension(string path)
    {
        string path2, file, ext;
        GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
        return Combine(path2, file);
    }
    /// <summary>
    /// Get full path.
    /// </summary>
    /// <param name="'_'">The '_'.</param>
    /// <returns>The get full path result.</returns>
    public static string GetFullPath(string path)
    {
        var result = Path.GetFullPath(path);
        FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Initializes a new instance of the file to directory class.
    /// </summary>
    /// <param name="serie)">The serie.</param>
    public static void FileToDirectory(ref string dir)
    {
        if (!dir.EndsWith("\"")) dir = GetDirectoryName(dir);
    }
    #endregion
    #region For easy copy from FSShared64.cs
    /// <summary>
    /// Exists directory worker.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns>The exists directory worker result.</returns>
    public static bool ExistsDirectoryWorker(string directoryPath, bool isFalseIfContainsNoFile = false)
    {
        // Not working, flags from GeoCachingTool wasnt transfered to standard
#if NETFX_CORE
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-"Use methods in FS apps");
#endif
#if WINDOWS_UWP
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exceptions.CallingMethod(), "  "+-"Use methods in FS apps");
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
    /// <summary>
    /// The files which surely exists.
    /// </summary>
    public static List<string> FilesWhichSurelyExists = new();
    /// <summary>
    /// Exists file.
    /// </summary>
    /// <param name="selectedFile">The selected file.</param>
    /// <param name="falseIfSizeZeroOrEmpty">The false if size zero or empty.</param>
    /// <returns>The exists file result.</returns>
    public static
        async Task<bool>
        ExistsFile(string selectedFile, bool falseIfSizeZeroOrEmpty)
    {
        selectedFile = SH.FirstCharUpper(selectedFile);
        if (FilesWhichSurelyExists.Contains(selectedFile)) return true;
        if (selectedFile == @"\\?\" || selectedFile == string.Empty) return false;
        MakeUncLongPath(ref selectedFile);
        var exists = File.Exists(selectedFile);
        if (falseIfSizeZeroOrEmpty)
        {
            if (!exists) return false;
            var ext = Path.GetExtension(selectedFile).ToLower();
            // I have to check it only when it is tmp, logically
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
                // I had a bug where even though exists was true, File.ReadAllTextAsync failed because the file did not exist.
                // I solved it by checking the extension, hopefully
                return false;
        }
        return exists;
    }


    /// <summary>
    /// Combine.
    /// </summary>
    /// <returns>The combine result.</returns>
    public static string Combine(params string[] parts)
    {
        return CombineWorker(true, false, parts);
    }
    /// <summary>
    /// Initializes a new instance of the combine file class.
    /// </summary>
    /// <param name="selectedFile">The selected file.</param>
    /// <param name="falseIfSizeZeroOrEmpty">The false if size zero or empty.</param>
    public static string CombineFile(params string[] parts)
    {
        return CombineWorker(true, true, parts);
    }
    /// <summary>
    /// Initializes a new instance of the combine dir class.
    /// </summary>
    /// <param name="selectedFile">The selected file.</param>
    public static string CombineDir(params string[] parts)
    {
        return CombineWorker(true, false, parts);
    }
    private static string CombineWorker(bool isFirstCharUpper, bool file, params string[] paths)
    {
        for (var i = 0; i < paths.Length; i++) paths[i] = paths[i].TrimStart('\\');
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
    /// <summary>
    /// Get folder size.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <returns>The get folder size result.</returns>
    public static long GetFolderSize(string path)
    {
        return GetFolderSize(new DirectoryInfo(path));
    }
    /// <summary>
    /// Get folder size.
    /// </summary>
    /// <param name="parts">The parts.</param>
    /// <returns>The get folder size result.</returns>
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
            // System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api and the remaining folders in it are junctions that lost their target
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
            // System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api and the remaining folders in it are junctions that lost their target
        }
        foreach (var subdirectory in subdirectories) size += GetFolderSize(subdirectory);
        return size;
    }
    /// <summary>
    /// Group files by name.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The group files by name result.</returns>
    public static Dictionary<string, List<string>> GroupFilesByName(List<string> filesInSubfolders)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var item in filesInSubfolders) DictionaryHelper.AddOrCreate(result, Path.GetFileName(item), item);
        return result;
    }
    /// <summary>
    /// Base path.
    /// </summary>
    /// <param name="directoryInfo">The directory info.</param>
    /// <returns>The base path result.</returns>
    public static string? BasePath(List<string> basePaths, string path)
    {
        foreach (var item in basePaths)
            if (path.Contains(item))
                return item;
        return null;
    }
    /// <summary>
    /// Has any folders or files.
    /// </summary>
    /// <returns>The has any folders or files result.</returns>
    public static bool HasAnyFoldersOrFiles(string folder)
    {
        return Directory.GetFiles(folder).Length > 0 ||
               Directory.GetDirectories(folder).Length > 0;
    }
    /// <summary>
    /// Move directory no recursive.
    /// </summary>
    /// <param name="files">The files.</param>
    /// <returns>The move directory no recursive result.</returns>
    public static void MoveDirectoryNoRecursive(string sourcePath, string targetPath, DirectoryMoveCollisionOption directoryMoveCollisionOption, object fileMoveCollisionOption)
    {
        throw new NotImplementedException();
    }
    #endregion
}
