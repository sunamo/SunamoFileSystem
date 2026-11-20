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
}
