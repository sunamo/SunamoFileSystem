namespace SunamoFileSystem;

using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

/// <summary>
/// File System utility class providing file and directory operations
/// </summary>
public class FS
{
    /// <summary>
    /// Default file name pattern for ends-with replacement operations.
    /// </summary>
    public const string DEndsWithReplaceInFile = "SubdomainHelperSimple.cs";
    /// <summary>
    /// Read-only list of invalid file name characters from the OS.
    /// </summary>
    protected static readonly List<char> invalidFileNameCharsReadonly = Path.GetInvalidFileNameChars().ToList();
    /// <summary>
    /// Read-only list of invalid file name characters as strings.
    /// </summary>
    protected static readonly List<string> invalidFileNameStringsReadonly;
    /// <summary>
    /// Determines whether the specified path is an absolute path.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static bool IsAbsolutePath(string path)
    {
        return !String.IsNullOrWhiteSpace(path)
            && path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1
            && Path.IsPathRooted(path)
            && !Path.GetPathRoot(path)!.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
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
    /// <summary>
    /// List of invalid path characters including directory separators.
    /// </summary>
    protected static List<char> invalidPathChars;
    /// <summary>
    /// Field as string because array requires ToArray() every time to construct string
    /// </summary>
    public static string InvalidFileNameCharsString;
    /// <summary>
    /// List of all invalid file name characters including extended Unicode.
    /// </summary>
    public static List<char> InvalidFileNameChars;
    /// <summary>
    /// List of invalid characters for path mapping operations.
    /// </summary>
    protected static List<char> invalidCharsForMapPath;
    /// <summary>
    /// Invalid file name characters excluding directory separators.
    /// </summary>
    protected static List<char> invalidFileNameCharsWithoutDelimiterOfFolders;
    /// <summary>
    /// Replacement string for incorrect file name characters.
    /// </summary>
    public static string ReplaceIncorrectFor = string.Empty;
    /// <summary>
    /// Action delegate to delete a file that may be locked by another process.
    /// </summary>
    public static Action<string>? DeleteFileMaybeLocked;
    /// <summary>
    /// Function to determine which processes are locking a file.
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
    /// Creates all parent folders of the specified path if they don't exist
    /// </summary>
    /// <param name="path">The file or folder path</param>
    public static void CreateUpfoldersPsysicallyUnlessThere(string path)
    {
        CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(path)!);
    }
    /// <summary>
    /// Determines whether the specified directory exists on disk.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static bool ExistsDirectory(string path)
    {
        return Directory.Exists(path);
    }
    /// <summary>
    /// Creates all parent folders of the specified path if they don't exist
    /// </summary>
    /// <param name="path">The folder path to create</param>
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
    ///     All occurences Path's method in sunamo replaced
    /// </summary>
    /// <param name="value">The value to process.</param>

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
    /// Creates a directory if it does not already exist.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static void CreateDirectoryIfNotExists(string path)
    {
        MakeUncLongPath(ref path);
        if (!ExistsDirectory(path)) Directory.CreateDirectory(path);
    }
    /// <summary>
    /// Ensures the path ends with a trailing backslash.
    /// </summary>
    /// <param name="value">The value to process.</param>
    public static string WithEndSlash(string value)
    {
        return WithEndSlash(ref value);
    }
    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>

    /// <returns></returns>
    /// <param name="value">The value to process.</param>
    public static string WithEndSlash(ref string value)
    {
        if (value != string.Empty) value = value.TrimEnd('\\') + '\\';
        FirstCharUpper(ref value);
        return value;
    }
    /// <summary>
    /// Finds all folders that contain a specific subfolder.
    /// </summary>
    /// <param name="solutionFolder">The solutionFolder parameter.</param>
    /// <param name="folderName">The folderName parameter.</param>
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
    /// <summary>
    /// Converts the first character of a string to uppercase
    /// </summary>
    /// <param name="text">The text to process</param>
    /// <returns>String with first character in uppercase</returns>
    public static string FirstCharUpper(string text)
    {
        if (text.Length == 1) return text.ToUpper();
        var substring = text.Substring(1);
        return text[0].ToString().ToUpper() + substring;
    }
    /// <summary>
    /// Attempts to delete a file, returning success status.
    /// </summary>
    /// <param name="filePath">The file path.</param>
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
    /// <summary>
    /// Writes all text to file with exception handling
    /// </summary>
    /// <param name="file">The file path to write to</param>
    /// <param name="content">The content to write</param>
    public static async Task WriteAllTextWithExc(string file, string content)
    {
        try
        {
            await File.WriteAllTextAsync(file, content);
        }
        catch (Exception)
        {
            //TypedSunamoLogger.Instance.Error//(Exceptions.TextOfExceptions(ex));
        }
    }
    /// <summary>
    /// Creates an empty file if it does not already exist.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
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
    /// <summary>
    /// Inserts text between the file name and its extension.
    /// </summary>
    /// <param name="orig">The orig parameter.</param>
    /// <param name="whatInsert">The whatInsert parameter.</param>
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
    /// <summary>
    ///     ReplaceIncorrectCharactersFile - can specify char for replace with
    ///     ReplaceInvalidFileNameChars - all wrong chars skip
    /// </summary>
    /// <param name="filename">The filename to process</param>
    /// <param name="characters">Characters to keep even if they're invalid</param>
    /// <returns>Filename with invalid characters removed</returns>
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
    /// <summary>
    ///     .babelrc etc. return as is. but files which not contains only alphanumeric will be returned when A3 (and A2 is then
    ///     ignored)
    ///     ALL EXT. HAVE TO BE ALWAYS LOWER
    ///     Return in lowercase
    /// </summary>
    /// <param name="value">The file path to extract extension from</param>
    /// <param name="args">Optional arguments for extension extraction</param>
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
    /// Determines whether the specified string is a valid file extension.
    /// </summary>
    /// <param name="result">The result parameter.</param>
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
    /// <summary>
    /// Determines whether the specified file exists on disk.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static bool ExistsFile(string path)
    {
        return File.Exists(path);
    }
    /// <summary>
    /// Moves specified subfolders from one location to another.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="subfolderNames">The subfolderNames parameter.</param>
    /// <param name="from">The from parameter.</param>
    /// <param name="to">The to parameter.</param>
    /// <param name="fo">The fo parameter.</param>
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
    /// Removes the base path and trailing backslashes from a list of paths.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <param name="basePath">The basePath parameter.</param>
    public static void TrimBasePathAndTrailingBs(List<string> text, string basePath)
    {
        for (var i = 0; i < text.Count; i++)
        {
            text[i] = text[i].Substring(basePath.Length);
            text[i] = text[i].TrimEnd('\\');
        }
    }
    /// <summary>
    /// Gets the file name by removing the last path segment.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static string GetFileNameWithoutOneExtension(string path)
    {
        return SHParts.RemoveAfterLast(path, "\\");
    }
    /// <summary>
    /// Returns the current date and time as a file-safe string.
    /// </summary>
    public static string GetActualDateTime()
    {
        var dt = DateTime.Now;
        return ReplaceIncorrectCharactersFile(dt.ToString());
    }
    /// <summary>
    /// Filters a list to keep only items not found in the specified files.
    /// </summary>
    /// <param name="opts">The list of options to filter.</param>
    /// <param name="paths">The list of file paths to compare against.</param>
    public static
#if ASYNC
        async Task<List<string>>
#else
List<string>
#endif
        KeepOnlyWhichIsNotInFiles(List<string> opts, List<string> paths)
    {
        var count = new CollectionWithoutDuplicates<string>();
        foreach (var item in paths)
            count.AddRange(SHGetLines.GetLines(
#if ASYNC
                await
#endif
                    File.ReadAllTextAsync(item)).ToList());
        CAG.CompareList(opts, count.c);
        return opts;
    }
    /// <summary>
    ///     count:\repos\EOM-7\Marvin\Module.VBtO\Clients\src\apps\vbto\src\pages\Administration\Administration.test.tsx
    ///     ../../../../../../../node_modules/@mui/material/Switch/Switch
    ///     => count:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@mui\material\Switch\Switch
    ///     => OK
    /// </summary>
    /// <param name="fullPathToSecondFile"></param>
    /// <param name="relativePath"></param>
    /// <returns></returns>
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
    /// <summary>
    ///     Usage: SunamoFubuCsprojFileHelper.GetProjectsInSlnFile
    ///     Cant name GetAbsolutePath because The call is ambiguous between the following methods or properties:
    ///     'CAChangeContent.ChangeContent0(null,List&lt;string&gt;, Func&lt;string, string, string&gt;)'
    ///     and 'CAChangeContent.ChangeContent0(null,List&lt;string&gt;, Func&lt;string, string&gt;)'
    /// </summary>
    /// <param name="path">Path to convert to absolute path</param>
    public static string AbsoluteFromCombinePath(string path)
    {
        var result = Path.GetFullPath(new Uri(path).LocalPath);
        return result;
    }
    /// <summary>
    /// Wraps text with quote marks if needed
    /// </summary>
    /// <param name="text">Text to wrap</param>
    /// <param name="forceNotIncludeQm">Force not to include quote marks</param>
    /// <returns>Text wrapped with quote marks if needed</returns>
    public static string WrapWithQm(string text, bool? forceNotIncludeQm)
    {
        if (text.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault()) return SH.WrapWithQm(text);
        return text;
    }
    /// <summary>
    /// Separates files into root-level and subfolder groups.
    /// </summary>
    /// <param name="rootFolder">The rootFolder parameter.</param>
    /// <param name="files">The files parameter.</param>
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
    /// Replaces full paths with just file names in the list.
    /// </summary>
    /// <param name="subfolders">The subfolders parameter.</param>
    public static void OnlyNames(List<string> subfolders)
    {
        for (var i = 0; i < subfolders.Count; i++) subfolders[i] = Path.GetFileName(subfolders[i]);
    }
    /// <summary>
    /// Finds files matching a pattern that contain all required contents.
    /// </summary>
    /// <param name="sunamo">The sunamo parameter.</param>
    /// <param name="searchPattern">The searchPattern parameter.</param>
    /// <param name="requiredContents">The requiredContents parameter.</param>
    public static List<string> FilesWhichContainsAll(object sunamo, string searchPattern, params string[] requiredContents)
    {
        return FilesWhichContainsAll(sunamo, searchPattern, requiredContents);
    }
    /// <summary>
    /// Constructs a path from base and relative segments up to a specified depth level.
    /// </summary>
    /// <param name="basePath">The basePath parameter.</param>
    /// <param name="relativePath">The relativePath parameter.</param>
    /// <param name="value">The value to process.</param>
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
    /// <summary>
    /// Returns the directory name if the path points to a file, otherwise returns the path.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static string GetDirectoryNameIfIsFile(string path)
    {
        if (File.Exists(path)) return Path.GetDirectoryName(path)!;
        return path;
    }
    /// <summary>
    /// Creates a wildcard search mask from a list of file extensions.
    /// </summary>
    /// <param name="allExtensions">The allExtensions parameter.</param>
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
    /// <param name="logger">Logger instance</param>
    /// <param name="data">New names of files without extension</param>
    /// <param name="path">Directory path containing files</param>
    /// <param name="startFrom">Starting index for renaming</param>
    /// <param name="ext">File extension</param>
    public static void RenameNumberedSerieFiles(ILogger logger, List<string> data, string path, int startFrom, string ext)
    {
        var searchPattern = MascFromExtension(ext);
        var files = FSGetFiles.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        RenameNumberedSerieFiles(logger, data, files, startFrom, ext);
    }
    /// <summary>
    ///     A1 is new names of files without extension. Can use LinearHelper
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="data">New names of files without extension</param>
    /// <param name="files">List of file paths to rename</param>
    /// <param name="startFrom">Starting index for renaming</param>
    /// <param name="ext">File extension</param>
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
    /// <summary>
    /// Places a file in a folder by combining the target folder path with the file's parent folder name and file name
    /// </summary>
    /// <param name="sourcePath">The source file path</param>
    /// <param name="targetFolder">The target folder path</param>
    /// <returns>Combined path: targetFolder/parentFolderName/fileName</returns>
    public static string PlaceInFolder(string sourcePath, string targetFolder)
    {
        //return Slozka.ci.PridejNadslozku(sourcePath, targetFolder);
        var parentPath = Path.GetDirectoryName(sourcePath);
        var parentFolderName = Path.GetFileName(parentPath);
        return Path.Combine(targetFolder, Path.Combine(parentFolderName!, Path.GetFileName(sourcePath)));
    }
    /// <summary>
    ///     Všechny soubory které se podaří přesunout vymažu z A1
    ///     A1 MUST BE WITH EXTENSION
    ///     A4 can be null if !A5
    ///     In A1 will keep files which doesnt exists in A3
    ///     A4 is files from A1 which wasnt founded in A2
    ///     A7 is files
    /// </summary>
    /// <param name="filesFrom">List of source file paths relative to folderFrom.</param>
    /// <param name="folderFrom">Source folder path.</param>
    /// <param name="folderTo">Target folder path.</param>
    /// <param name="wasntExistsInFrom">List to collect files that did not exist in source.</param>
    /// <param name="mustExistsInTarget">Whether files must exist in target.</param>
    /// <param name="copy">Whether to copy (true) or move (false).</param>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="files">Dictionary mapping file categories to their paths.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
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
#if DEBUG

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
                    CopyFile(logger, oldPath, newPath, FileMoveCollisionOption.Overwrite);
                else
                    MoveFile(logger, oldPath, newPath, FileMoveCollisionOption.Overwrite);
            }
            filesFrom.RemoveAt(i);
        }
    }
    /// <summary>
    /// Simplified version of CopyMoveFilesInList for basic copy/move operations.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="files">The files parameter.</param>
    /// <param name="basePathCjHtml1">The basePathCjHtml1 parameter.</param>
    /// <param name="basePathCjHtml2">The basePathCjHtml2 parameter.</param>
    /// <param name="copy">The copy parameter.</param>
    /// <param name="overwrite">Whether to overwrite existing files.</param>
    public static void CopyMoveFilesInListSimple(ILogger logger, List<string> files, string basePathCjHtml1, string basePathCjHtml2,
        bool copy, bool overwrite = true)
    {
        List<string>? wasntExistsInFrom = null;
        var mustExistsInTarget = false;
        CopyMoveFilesInList(logger, files, basePathCjHtml1, basePathCjHtml2, wasntExistsInFrom!, mustExistsInTarget, copy, null!,
            overwrite);
    }
    /// <summary>
    /// Recreates the folder structure from one location in another.
    /// </summary>
    /// <param name="from">The from parameter.</param>
    /// <param name="to">The to parameter.</param>
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
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    public static void CopyMoveFromMultiLocationIntoOne(ILogger logger, List<string> files, string folderFrom, string folderTo)
    {
        var wasntExists = new List<string>();
        var files2 = new Dictionary<string, List<string>>();
        var getFiles = FSGetFiles.GetFiles(folderFrom, "*.cs", SearchOption.AllDirectories,
            new GetFilesArgsFS { ExcludeFromLocationsContains = new List<string>(["TestFiles"]) });
        foreach (var item in files) files2.Add(item, getFiles.Where(data => Path.GetFileName(data) == item).ToList());
        CopyMoveFilesInList(logger, files, folderFrom, folderTo, wasntExists, false, true, files2);
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
    ///// <summary>
    /////     A1 must be sunamo.Data.StorageFolder or uwp StorageFolder
    /////     Return fixed string is here right
    ///// </summary>
    ///// <param name="folder"></param>

    //public static StorageFile GetStorageFile<StorageFolder, StorageFile>(StorageFolder folder, string value, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ((dynamic)ac.fs.getStorageFile(folder, value)).Path;
    //    }
    //    return (dynamic)Path.Combine(folder.ToString(), value);
    //}
    /// <summary>
    /// Deletes all empty (0-byte) files in the specified folder.
    /// </summary>
    /// <param name="folder">The directory path to search in.</param>
    /// <param name="so">Specifies whether to search the current directory or all subdirectories.</param>
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
            var fileSize = new FileInfo(item).Length;
            if (fileSize == 0)
                TryDeleteFile(item);
            else if (fileSize < 4)
                if ((
#if ASYNC
                        await
#endif
                            File.ReadAllTextAsync(item)).Trim() == string.Empty)
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
    /// <summary>
    /// Replaces text in all specified files.
    /// </summary>
    /// <param name="from">The from parameter.</param>
    /// <param name="to">The to parameter.</param>
    /// <param name="args">The args parameter.</param>
    /// <param name="EncodingHelperIsBinary">The EncodingHelperIsBinary parameter.</param>
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
    /// <summary>
    /// Replaces text in all specified files.
    /// </summary>
    /// <param name="folder">The directory path.</param>
    /// <param name="extension">The extension parameter.</param>
    /// <param name="replaceFrom">The replaceFrom parameter.</param>
    /// <param name="replaceTo">The replaceTo parameter.</param>
    /// <param name="isMultilineWithVariousIndent">The isMultilineWithVariousIndent parameter.</param>
    /// <param name="EncodingHelperIsBinary">The EncodingHelperIsBinary parameter.</param>
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
    ///     A4 - whether use text.Contains. A4 - SHReplace.ReplaceAll2
    /// </summary>
    /// <param name="replaceFrom">List of strings to find and replace.</param>
    /// <param name="replaceTo">List of replacement strings.</param>
    /// <param name="args">Arguments controlling the replacement behavior.</param>
    /// <param name="EncodingHelperIsBinary">Function to determine if a file is binary.</param>
    public static
#if ASYNC
        async Task
#else
void
#endif
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
#if DEBUG
            if (item.EndsWith(DEndsWithReplaceInFile))
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
            //ThisApp.Warning(Translate.FromKey(XlfKeys.ContentOf) + " " + item + " couldn't be replaced - contains control chars.");
        }
    }
    /// <summary>
    ///     Jen kvuli starým aplikacím, at furt nenahrazuji.
    /// </summary>
    /// <param name="value">The value to process.</param>

    public static string GetFileInStartupPath(string value)
    {
        return AppPaths.GetFileInStartupPath(value);
    }
    /// <summary>
    /// Removes diacritic marks from file contents in the specified folder.
    /// </summary>
    /// <param name="folder">The directory path.</param>
    /// <param name="mask">The file search mask pattern.</param>
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
    //    List<string> data = new List<string>(files1.Count());
    //    foreach (var item in files1)
    //    {
    //        data.Add(FS.StorageFilePath(item, ac));
    //    }
    //    return data;
    //}
    /// <summary>
    /// Removes a file from disk.
    /// </summary>
    /// <param name="fullPathCsproj">The fullPathCsproj parameter.</param>
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
    /// Creates a file from the last part of a path with the specified extension.
    /// </summary>
    /// <param name="fullPath">The fullPath parameter.</param>
    /// <param name="ext">The ext parameter.</param>
    public static string MakeFromLastPartFile(string fullPath, string ext)
    {
        WithoutEndSlash(ref fullPath);
        return fullPath + ext;
    }
    /// <summary>
    ///     Remove all extensions, not only one
    /// </summary>
    /// <param name="path">File path to process</param>
    public static string GetFileNameWithoutExtensions(string path)
    {
        while (Path.HasExtension(path)) path = Path.GetFileNameWithoutExtension(path);
        return path;
    }
    /// <summary>
    /// Copies the folder structure as empty 0KB files including subfolders.
    /// </summary>
    public static void CopyAs0KbFilesSubfolders
        (string pathDownload, string pathVideos0Kb)
    {
        WithEndSlash(ref pathDownload);
        WithEndSlash(ref pathVideos0Kb);
        var folders = Directory.GetDirectories(pathDownload);
        foreach (var item in folders) CopyAs0KbFiles(item, item.Replace(pathDownload, pathVideos0Kb));
    }
    /// <summary>
    /// Copies files as empty 0KB files to the target directory.
    /// </summary>
    /// <param name="pathDownload">The pathDownload parameter.</param>
    /// <param name="pathVideos0Kb">The pathVideos0Kb parameter.</param>
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
    /// Shortens a long file path to a displayable length.
    /// </summary>
    /// <param name="actualFilePath">The actualFilePath parameter.</param>
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
    /// <summary>
    /// Creates a new folder path adjacent to the specified path.
    /// </summary>
    /// <param name="folder">The directory path.</param>
    /// <param name="ending">The ending parameter.</param>
    public static string CreateNewFolderPathWithEndingNextTo(string folder, string ending)
    {
        var pathToFolder = Path.GetDirectoryName(folder.TrimEnd('\\')) + "\"";
        var folderWithCaretFiles = pathToFolder + Path.GetFileName(folder.TrimEnd('\\')) + ending;
        var result = folderWithCaretFiles;
        SH.FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Copies files with specified extensions from source to target directory.
    /// </summary>
    /// <param name="folderFrom">The folderFrom parameter.</param>
    /// <param name="folderTo">The folderTo parameter.</param>
    /// <param name="extensions">The extensions parameter.</param>
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
                var realNewPath = new string(newpath);
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
    /// Traverses up the directory tree to find a folder containing files with the specified extension.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <param name="fileExt">The fileExt parameter.</param>
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
    /// Trims whitespace from the content of all matching files in a folder.
    /// </summary>
    /// <param name="folder">The directory path.</param>
    /// <param name="searchPattern">The searchPattern parameter.</param>
    /// <param name="searchOption">The searchOption parameter.</param>
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
    /// <summary>
    ///     Náhrada za metodu ReplaceFileName se stejnými parametry
    /// </summary>
    /// <param name="oldPath"></param>
    /// <param name="what"></param>
    /// <param name="forWhat"></param>
    public static string ReplaceInFileName(string oldPath, string what, string forWhat)
    {
        string path, fileName;
        GetPathAndFileName(oldPath, out path, out fileName);
        var result = path + "\"" + fileName.Replace(what, forWhat);
        SH.FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Converts a file size value between different computer size units.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <param name="fromUnit">The fromUnit parameter.</param>
    /// <param name="to">The to parameter.</param>
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
    ///     Zjistí všechny složky rekurzivně z A1 a prvně maže samozřejmě ty, které mají více tokenů
    /// </summary>
    /// <param name="value">The directory path to process</param>
    /// <param name="excludePatterns">Paths containing these strings will not be deleted</param>
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
        if (IsDirectoryEmpty(value, false, true) && !excludePatterns.Any()) TryDeleteDirectory(value);
    }
    //private static List<TWithInt<string>> DirectoriesWithToken(string value, AscDesc desc)
    //{
    //    ThrowEx.NotImplementedMethod();
    //}
    /// <summary>
    /// Compares two TWithInt instances by their count in descending order.
    /// </summary>
    /// <param name="first">The first parameter.</param>
    /// <param name="second">The second parameter.</param>
    public static int CompareTWithInt<T>(TWithInt<T> first, TWithInt<T> second)
    {
        if (first.Count > second.Count)
            return 1;
        if (first.Count < second.Count) return -1;
        return 0;
    }
    /// <summary>
    /// Gets directories sorted by a numeric token in their path.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <param name="sortOrder">The sortOrder parameter.</param>
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
    /// <summary>
    ///     A1 i A2 musí končit backslashem
    ///     Může vyhodit výjimku takže je nutné to odchytávat ve volající metodě
    ///     If destination folder exists, source folder without files keep
    ///     Return message if success, or null
    ///     A5 false
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="from">Source directory path</param>
    /// <param name="to">Destination directory path</param>
    /// <param name="directoryMoveCollisionOption">Directory collision handling option</param>
    /// <param name="fileMoveCollisionOption">File collision handling option</param>
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
    /// <summary>
    /// Moves all files recursively and then deletes the directory structure
    /// Throws exceptions, so must be called from try-catch block
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="sourcePath">Source directory path</param>
    /// <param name="targetPath">Target directory path (root of target folder)</param>
    /// <param name="collisionOption">File collision handling option</param>
    public static void MoveAllRecursivelyAndThenDirectory(ILogger logger, string sourcePath, string targetPath, FileMoveCollisionOption collisionOption)
    {
        CopyMoveAllFilesRecursively(logger, sourcePath, targetPath, collisionOption, true, null!, SearchOption.AllDirectories);
        var dirs = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
        for (var i = dirs.Length - 1; i >= 0; i--) TryDeleteDirectory(dirs[i]);
        TryDeleteDirectory(sourcePath);
    }
    /// <summary>
    /// Moves all files recursively from source to target directory.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="sourcePath">The source directory path.</param>
    /// <param name="targetPath">The target directory path.</param>
    /// <param name="collisionOption">How to handle file name collisions.</param>
    /// <param name="contains">Optional filter string; only files containing this text are moved. Prefix with '!' for negation.</param>
    [Obsolete("Use MoveDirectoryNoRecursive instead")]
    public static void MoveAllFilesRecursively(ILogger logger, string sourcePath, string targetPath, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, sourcePath, targetPath, collisionOption, true, contains!, SearchOption.AllDirectories);
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
        throw new Exception(Translate.FromKey(XlfKeys.OnlyForTestFilesForAnotherApps) + ". ");
    }
    /// <summary>
    /// Deletes files with same content by comparing their content using the provided read function
    /// Currently kept as sync because Func only has Invoke, cannot use async benefits
    /// </summary>
    /// <typeparam name="TContent">The type of content to compare</typeparam>
    /// <typeparam name="ColType">The collection element type</typeparam>
    /// <param name="files">List of file paths to check for duplicates</param>
    /// <param name="readFunc">Function to read file content</param>
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
    /// <param name="list">List of strings to order by natural number sequence</param>
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
    /// <summary>
    /// Sorts file paths alphabetically by their file name.
    /// </summary>
    /// <param name="allCsFilesInFolder">The allCsFilesInFolder parameter.</param>
    /// <param name="onlyOneExtension">The onlyOneExtension parameter.</param>
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
    /// Deletes all files and optionally directories recursively.
    /// </summary>
    /// <param name="path">The file or directory path to delete contents from.</param>
    /// <param name="rootDirectoryToo">Whether to also delete the root directory itself.</param>
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
    /// Deletes folders that do not contain files matching specified patterns.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <param name="folder">The directory path.</param>
    /// <param name="excludedContainingTexts">The excludedContainingTexts parameter.</param>
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
    ///     Vyhazuje výjimky, takže musíš volat value try-catch bloku
    /// </summary>
    /// <param name="path">The directory path to delete</param>
    public static void DeleteAllRecursivelyAndThenDirectory(string path)
    {
        DeleteAllRecursively(path, true);
    }
    /// <summary>
    /// Extracts only the extensions from a list of file paths.
    /// </summary>
    /// <param name="paths">The paths parameter.</param>
    public static List<string> OnlyExtensions(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        //CA.InitFillWith(result, paths.Count);
        for (var i = 0; i < result.Count; i++) result[i] = Path.GetExtension(paths[i]);
        return result;
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
    /// <summary>
    /// Extracts extensions from file paths and converts them to lowercase.
    /// </summary>
    /// <param name="paths">The list of file paths to extract extensions from.</param>
    /// <param name="args">Optional arguments for controlling extension extraction behavior.</param>
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
    /// Extracts lowercase extensions from file paths, preserving the path context.
    /// </summary>
    /// <param name="paths">The paths parameter.</param>
    public static List<string> OnlyExtensionsToLowerWithPath(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        //CA.InitFillWith(result, paths.Count);
        for (var i = 0; i < result.Count; i++) result[i] = OnlyExtensionToLowerWithPath(paths[i]);
        return result;
    }
    /// <summary>
    /// Gets the lowercase extension of a single file path.
    /// </summary>
    /// <param name="data">The data parameter.</param>
    public static string OnlyExtensionToLowerWithPath(string data)
    {
        string path, fn, ext;
        GetPathAndFileName(data, out path, out fn, out ext);
        var result = path + fn + ext.ToLower();
        return result;
    }
    /// <summary>
    /// Gets all unique file extensions found in the specified folders.
    /// </summary>
    /// <param name="so">The so parameter.</param>
    /// <param name="folders">The folders parameter.</param>
    public static List<string> AllExtensionsInFolders(SearchOption so, params string[] folders)
    {
        ThrowEx.NoPassedFolders(folders);
        List<string> filesFull = FSGetFiles.AllFilesInFolders(folders.ToList(), new List<string>(["*"]), so);
        return AllExtensionsInFolders(filesFull);
    }
    /// <summary>
    ///     files as .bowerrc return whole
    /// </summary>
    /// <param name="args">Arguments controlling the operation behavior.</param>
    /// <param name="filesFull">List of full file paths.</param>


    public static List<string> AllExtensionsInFolders(List<string> filesFull, GetExtensionArgs? args = null)
    {
        var result = new List<string>();
#if DEBUG
        //var dx = filesFull.IndexOf(".babelrc");
#endif
        var files = new List<string>(OnlyExtensionsToLower(filesFull, args));
#if DEBUG
        //var dxs = CA.IndexesWithValue(files, "");
        //List<string> count = CA.GetIndexes(filesFull, dxs);
        //ClipboardHelper.SetLines(count);
        //var dx2 = files.IndexOf(".babelrc");
#endif
        foreach (var item in files)
            if (!result.Contains(item))
                result.Add(item);
        return result;
    }
    /// <summary>
    /// Expands a known environment variable to its value.
    /// </summary>
    /// <param name="environmentVariable">The environmentVariable parameter.</param>
    public static string ExpandEnvironmentVariables(EnvironmentVariables environmentVariable)
    {
        return Environment.ExpandEnvironmentVariables(SH.WrapWith(environmentVariable.ToString(), "%"));
    }
    /// <summary>
    ///     Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    /// </summary>
    /// <param name="text">The file path to process</param>
    public static string GetFileNameWithoutExtensionLower(string text)
    {
        return GetFileNameWithoutExtension(text).ToLower();
    }
    /// <summary>
    /// Prepends parent directory references to a relative path.
    /// </summary>
    /// <param name="i2">The i2 parameter.</param>
    /// <param name="file">The file parameter.</param>
    /// <param name="delimiter">The delimiter parameter.</param>
    public static string AddUpfoldersToRelativePath(int i2, string file, char delimiter)
    {
        var jumpUp = ".." + delimiter;
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < i2; i++) stringBuilder.Append(jumpUp);
        stringBuilder.Append(file);
        return stringBuilder.ToString();
        //return SHJoin.JoinTimes(i, jumpUp) + file;
    }
    /// <summary>
    ///     convert to lowercase and remove first dot - to už asi neplatí. Use NormalizeExtension2 for that
    /// </summary>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension(string extension)
    {
        return "." + extension.TrimStart('.');
    }
    /// <summary>
    /// Gets the normalized extension from a file path.
    /// </summary>
    /// <param name="filename">The filename parameter.</param>
    public static string GetNormalizedExtension(string filename)
    {
        return NormalizeExtension(filename);
    }
    /// <summary>
    /// Gets the last modified time of a file as a Unix timestamp.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public static long ModifiedinUnix(string filePath)
    {
        return (long)File.GetLastWriteTimeUtc(filePath).Subtract(DTConstants.UnixFsStart).TotalSeconds;
    }
    /// <summary>
    /// Recursively removes diacritic marks from file and folder names.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="folder">The directory path.</param>
    /// <param name="dirs">The dirs parameter.</param>
    /// <param name="files">The files parameter.</param>
    /// <param name="directoryCollisionOption">The directoryCollisionOption parameter.</param>
    /// <param name="fileCollisionOption">The fileCollisionOption parameter.</param>
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
    ///     A1,2 = with ext
    ///     Physically rename file, this method is different from ChangeFilename in FileMoveCollisionOption A3 which can
    ///     control advanced collision solution
    /// </summary>
    /// <param name="oldPath"></param>
    /// <param name="newFileNameWithoutPath"></param>
    /// <param name="collisionOption"></param>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    public static void RenameFile(ILogger logger, string oldPath, string newFileNameWithoutPath, FileMoveCollisionOption collisionOption)
    {
        var to = ChangeFilename(oldPath, newFileNameWithoutPath, false);
        MoveFile(logger, oldPath, to, collisionOption);
    }
    /// <summary>
    ///     Může výhodit výjimku, proto je nutné používat value try-catch bloku
    ///     Vrátí řetězec se zprávou kterou vypsat nebo null
    /// </summary>
    /// <param name="path">The directory path to rename.</param>
    /// <param name="newname">The new directory name.</param>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="directoryCollisionOption">How to handle directory name collisions.</param>
    /// <param name="fileCollisionOption">How to handle file name collisions during the move.</param>
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


    /// <param name="file"></param>
    /// <param name="ext"></param>
    /// <param name="filePath">The file path.</param>
    public static void GetFileNameWithoutExtensionAndExtension(string filePath, out string file, out string ext)
    {
        file = Path.GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(file);
    }
    /// <summary>
    /// Saves a stream to a file at the specified path.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <param name="text">The text to process.</param>
    public static void SaveStream(string path, Stream text)
    {
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            CopyStream(text, fileStream);
            fileStream.Flush();
        }
    }
    /// <summary>
    /// Returns a new list containing only file names without extensions.
    /// </summary>
    /// <param name="paths">The paths parameter.</param>
    public static List<string> OnlyNamesWithoutExtensionCopy(List<string> paths)
    {
        var result = new List<string>(paths.Count);
        for (var i = 0; i < paths.Count; i++) result.Add(Path.GetFileNameWithoutExtension(paths[i]));
        return result;
    }
    /// <summary>
    /// Checks if a directory exists and contains files or subdirectories.
    /// </summary>
    /// <param name="value">The value to process.</param>
    public static bool DirectoryExistsAndIsNotEmpty(string value)
    {
        if (Directory.Exists(value) && Directory.GetFiles(value, "*", SearchOption.AllDirectories).Length != 0) return true;
        return false;
    }
    /// <summary>
    /// Replaces full paths with file names without extensions.
    /// </summary>
    /// <param name="appendToStart">The appendToStart parameter.</param>
    /// <param name="fullPaths">The fullPaths parameter.</param>
    public static List<string> OnlyNamesWithoutExtension(string appendToStart, List<string> fullPaths)
    {
        var result = new List<string>(fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++)
            result.Add(appendToStart + Path.GetFileNameWithoutExtension(fullPaths[i]));
        return result;
    }
    /// <summary>
    /// Appends a postfix to a file name before the extension.
    /// </summary>
    /// <param name="aPath">The aPath parameter.</param>
    /// <param name="text">The text to process.</param>
    public static string Postfix(string aPath, string text)
    {
        var result = aPath.TrimEnd('\\') + text;
        WithEndSlash(ref result);
        return result;
    }
    /// <summary>
    /// Reads all text content from a file.
    /// </summary>
    /// <param name="path">The file path to read from.</param>
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
    /// <summary>
    /// Gets the file name from a path without the extension.
    /// </summary>
    /// <param name="text">The text to process.</param>
    public static string GetFileNameWithoutExtension(string text)
    {
        return PathMs.GetFileNameWithoutExtension(text.TrimEnd(PathMs.DirectorySeparatorChar));
    }
    /// <summary>
    ///     Problémová metoda
    ///     Píše že nemůže najít SunamoValues, přitom value nugetech je
    /// </summary>
    /// <typeparam name="StorageFile"></typeparam>

    /// <returns></returns>
    /// <param name="text">The text to process.</param>
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
    ///// <summary>
    /////     Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    /////     if have more extension, remove just one
    ///// </summary>
    ///// <param name="s"></param>
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
    /// <summary>
    /// Throws a not-implemented exception for UWP-specific operations.
    /// </summary>
    public static void ThrowNotImplementedUwp()
    {
        throw new Exception("Not implemented in UWP");
    }
    /// <summary>
    /// Determines whether a file is older than the specified number of hours.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <param name="hours">The threshold number of hours.</param>
    /// <param name="mustFileExists">Whether to throw an exception if the file does not exist.</param>
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
    /// Gets file names without extensions from a list of paths.
    /// </summary>
    /// <param name="jpgcka">The jpgcka parameter.</param>
    public static List<string> GetFileNamesWoExtension(List<string> jpgcka)
    {
        var result = new List<string>(jpgcka.Count);
        for (var i = 0; i < jpgcka.Count; i++) result.Add(Path.GetFileNameWithoutExtension(jpgcka[i]));
        return result;
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
    /// <param name="value"></param>
    /// <param name="targetDirectory"></param>
    /// <param name="collisionOption">How to handle file name collisions.</param>
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
    /// <summary>
    ///     change all first (drive) letter to uppercase
    /// </summary>

    /// <param name="folderWithProjectsFolders"></param>
    /// <param name="folderWithTemporaryMovedContentWithoutBackslash"></param>
    /// <param name="path">The file or directory path.</param>
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
    ///     Direct edit
    /// </summary>

    /// <returns></returns>
    /// <param name="path">The file or directory path.</param>
    public static List<string> OnlyNamesWithoutExtension(List<string> path)
    {
        for (var i = 0; i < path.Count; i++) path[i] = Path.GetFileNameWithoutExtension(path[i]);
        return path;
    }
    /// <summary>
    ///     Vrátí cestu a název souboru text ext a ext
    /// </summary>

    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    /// <param name="filePath">The file path.</param>
    public static void GetPathAndFileName(string filePath, out string path, out string file, out string ext)
    {
        path = WithEndSlash(Path.GetDirectoryName(filePath)!);
        file = Path.GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(filePath);
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
    /// Splits a path into its component tokens.
    /// </summary>
    /// <param name="relativePath">The relativePath parameter.</param>
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
    /// Copies data from one stream to another.
    /// </summary>
    /// <param name="input">The input parameter.</param>
    /// <param name="output">The output parameter.</param>
    public static void CopyStream(Stream input, Stream output)
    {
        var buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0) output.Write(buffer, 0, len);
    }
    /// <summary>
    ///     Cant return with end slash becuase is working also with files
    /// </summary>
    /// <param name="text">The text to process.</param>

    public static string CombineWithoutFirstCharUpper(params string[] text)
    {
        return CombineWorker(false, true, text);
    }
    /// <summary>
    /// Saves a memory stream to a file.
    /// </summary>
    /// <param name="mss">The mss parameter.</param>
    /// <param name="path">The file or directory path.</param>
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
    /// <summary>
    /// Removes invalid characters from a directory name.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
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
    /// Removes invalid characters from a file name.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <param name="isPath">The isPath parameter.</param>
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
    /// Checks if a path segment contains invalid characters for mapping.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static bool ContainsInvalidPathCharForPartOfMapPath(string path)
    {
        foreach (var item in invalidCharsForMapPath)
            if (path.IndexOf(item) != -1)
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
    ///     POZOR: Na rozdíl od stejné metody value sunamo tato metoda vrací úplně nové pole a nemodifikuje A1
    /// </summary>
    /// <param name="files2">List of file paths.</param>

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
        var result = new List<string>(fullPaths.Count);
        for (var i = 0; i < fullPaths.Count; i++) result.Add(appendToStart + Path.GetFileName(fullPaths[i]));
        return result;
    }
    /// <summary>
    ///     A2 is path of target file
    /// </summary>



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
    /// <summary>
    /// Creates a directory with collision handling options.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <param name="whenExists">The whenExists parameter.</param>
    /// <param name="serieStyle">The serieStyle parameter.</param>
    /// <param name="reallyCreate">The reallyCreate parameter.</param>
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
    /// <summary>
    /// Converts a stream to a byte array.
    /// </summary>
    /// <param name="stream">The stream parameter.</param>
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
    /// Adds a file extension if the path does not already have one.
    /// </summary>
    /// <param name="file">The file parameter.</param>
    /// <param name="ext">The ext parameter.</param>
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
        var element = Path.GetExtension(orig);
        return Path.Combine(fn + whatInsert + element);
    }
    /// <summary>
    ///     In key are just filename, in value full path to all files
    /// </summary>
    /// <param name="files">List of file paths.</param>


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
    /// Changes the file name of a path, optionally renaming the physical file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="newFileNameWithoutPath">The newFileNameWithoutPath parameter.</param>
    /// <param name="physically">The physically parameter.</param>
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
    /// <summary>
    ///     A2 true - bs to slash. false - slash to bs
    /// </summary>
    /// <param name="path">The path to process</param>
    /// <param name="slash">True to convert backslashes to slashes, false for opposite</param>
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
    ///     Pokusí se max. 10x smazat soubor A1, pokud se nepodaří, GF, jinak GT
    /// </summary>
    /// <param name="filePath">Path to the file to delete</param>
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
    /// Attempts to delete a file, returning success status.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="message">The message parameter.</param>
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
    /// Returns a human-readable file size string with automatic unit selection.
    /// </summary>
    /// <param name="size">The size parameter.</param>
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
    /// Returns a human-readable file size string with automatic unit selection.
    /// </summary>
    /// <param name="value">The value to process.</param>
    /// <param name="fromUnit">The fromUnit parameter.</param>
    public static string GetSizeInAutoString(long value, ComputerSizeUnits fromUnit)
    {
        return GetSizeInAutoString((double)value, fromUnit);
    }
    /// <summary>
    ///     A1 is input unit, not output
    /// </summary>
    /// <param name="value"></param>

    /// <returns></returns>
    /// <param name="fromUnit">The source unit of measurement.</param>
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
    /// <param name="path">The file path to process</param>
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
    ///     ReplaceIncorrectCharactersFile - can specify char for replace with
    ///     ReplaceInvalidFileNameChars - all wrong chars skip
    ///     A2 - can specify more letter in one string
    ///     A3 is applicable only for A2. In general is use replaceIncorrectFor
    /// </summary>
    /// <param name="path">The file path to process</param>
    /// <param name="replaceAllOfThisByA3">Characters to additionally replace</param>
    /// <param name="replaceForThis">Replacement string</param>
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
    ///     Pro odstranění špatných znaků odstraní všechny výskyty A2 za mezery a udělá z více mezere jediné
    /// </summary>
    /// <param name="path">The file path to process</param>
    /// <param name="replaceAllOfThisThen">Characters to replace with empty string</param>
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
        if (parentFolder == null) parentFolder = Path.GetDirectoryName(folder)!;
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
        var path = Path.GetDirectoryName(fileName);
        var fn = Path.GetFileName(fileName);
        return Path.Combine(changeFolderTo, fn);
    }
    /// <summary>
    /// Lists files in a directory matching a pattern.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <param name="mask">The mask parameter.</param>
    /// <param name="so">The so parameter.</param>
    public static List<string> DirectoryListing(string path, string mask, SearchOption so)
    {
        var files = FSGetFiles.GetFiles(path, mask, so, new GetFilesArgsFS { TrimFirstPathAndLeadingBackslashes = true });
        return files;
    }
    /// <summary>
    /// Removes trailing backslash from a path.
    /// </summary>
    /// <param name="value">The value to process.</param>
    public static string WithoutEndSlash(string value)
    {
        return WithoutEndSlash(ref value);
    }
    /// <summary>
    /// Removes trailing backslash from a path.
    /// </summary>
    /// <param name="value">The value to process.</param>
    public static string WithoutEndSlash(ref string value)
    {
        value = value.TrimEnd('\\');
        return value;
    }
    /// <summary>
    /// Creates a wildcard search mask from a file extension.
    /// </summary>
    /// <param name="ext2">The file extension or wildcard pattern to create a mask from.</param>
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
    /// <summary>
    /// Checks if the number of files matching a pattern exceeds a threshold.
    /// </summary>
    /// <param name="folderPath">The folderPath parameter.</param>
    /// <param name="searchPattern">The searchPattern parameter.</param>
    /// <param name="getNullIfThereIsMoreThanXFiles">The getNullIfThereIsMoreThanXFiles parameter.</param>
    public static bool IsCountOfFilesMoreThan(string folderPath, string searchPattern, int getNullIfThereIsMoreThanXFiles)
    {
        var files = FSGetFiles.GetFilesEveryFolder(folderPath, searchPattern, SearchOption.AllDirectories,
            new GetFilesEveryFolderArgsFS { GetNullIfThereIsMoreThanXFiles = getNullIfThereIsMoreThanXFiles });
        return files == null;
    }
    /// <summary>
    /// Gets files from a directory, optionally including subdirectories.
    /// </summary>
    /// <param name="folderPath">The folderPath parameter.</param>
    /// <param name="recursive">The recursive parameter.</param>
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

    /// <param name="fileTo"></param>
    /// <param name="collisionOption"></param>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="sourceFilePath">The sourceFilePath parameter.</param>
    public static void MoveFile(ILogger logger, string sourceFilePath, string fileTo, FileMoveCollisionOption collisionOption)
    {
        if (CopyMoveFilePrepare(ref sourceFilePath, ref fileTo, collisionOption))
            try
            {
                sourceFilePath = MakeUncLongPath(sourceFilePath);
                fileTo = MakeUncLongPath(fileTo);
                if (collisionOption == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo)) return;
                File.Move(sourceFilePath, fileTo, collisionOption == FileMoveCollisionOption.Overwrite);
            }
            catch (Exception ex)
            {
                logger.LogError(sourceFilePath + " : " + ex.Message);
            }
    }
    /// <summary>
    /// Prepares source and target paths for a copy or move operation with collision handling.
    /// </summary>
    /// <param name="sourceFilePath">The sourceFilePath parameter.</param>
    /// <param name="fileTo">The fileTo parameter.</param>
    /// <param name="collisionOption">How to handle collisions.</param>
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
    /// <summary>
    /// Gets the size of a file in bytes.
    /// </summary>
    /// <param name="filePath">The file path.</param>
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
    /// <summary>
    /// Copies all files recursively from source to target directory.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="path">The source directory path.</param>
    /// <param name="to">The target directory path.</param>
    /// <param name="collisionOption">How to handle file name collisions.</param>
    /// <param name="contains">Optional filter string; only files containing this text are copied. Prefix with '!' for negation.</param>
    public static void CopyAllFilesRecursively(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, path, to, collisionOption, false, contains!, SearchOption.AllDirectories);
    }
    /// <summary>
    ///     A4 contains can use ! for negation
    /// </summary>

    /// <param name="to"></param>
    /// <param name="collisionOption"></param>
    /// <param name="contains"></param>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="path">The file or directory path.</param>
    public static void CopyAllFiles(ILogger logger, string path, string to, FileMoveCollisionOption collisionOption, string? contains = null)
    {
        CopyMoveAllFilesRecursively(logger, path, to, collisionOption, false, contains!, SearchOption.TopDirectoryOnly);
    }
    /// <summary>
    ///     If want use which not contains, prefix A4 with !
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="path">The source directory path.</param>
    /// <param name="to">The target directory path.</param>
    /// <param name="collisionOption">How to handle file name collisions.</param>
    /// <param name="move">Whether to move (true) or copy (false) files.</param>
    /// <param name="mustContains">Optional filter; only files containing this text. Prefix with '!' for negation.</param>
    /// <param name="so">Specifies whether to search the current directory or all subdirectories.</param>
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
    /// Copies a file from source to destination.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="sourceFilePath">The source file path to copy from.</param>
    /// <param name="fileTo2">The destination file path to copy to.</param>
    /// <param name="collisionOption">How to handle file name collisions.</param>
    /// <param name="terminateProcessIfIsInUsed">Whether to terminate the process that is locking the file.</param>
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
    ///     Copy file by ordinal way
    ///     tady byly 2 metody CopyFile(string, string, bool)
    ///     jedna text A3 terminateProcessIfIsInUsed, druhá text overwrite
    ///     Ta druhá jen volala A3 text FileMoveCollisionOption.Overwrite
    /// </summary>
    /// <param name="jsFiles"></param>
    /// <param name="value">The value to process.</param>
    /// <param name="terminateProcessIfIsInUsed">Whether to terminate the process if the file is in use.</param>
    /// <param name="logger">Logger instance for diagnostic messages.</param>

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
    /// <summary>
    /// Copies a file from source to destination.
    /// </summary>
    /// <param name="sourceFilePath">The sourceFilePath parameter.</param>
    /// <param name="fileTo2">The fileTo2 parameter.</param>
    /// <param name="collisionOption">How to handle collisions.</param>
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
    /// Gets the last modified date and time of a file.
    /// </summary>
    /// <param name="rel">The rel parameter.</param>
    public static DateTime LastModified(string rel)
    {
        if (File.Exists(rel)) return File.GetLastWriteTime(rel);
        // FileInfo mi držel soubor a vznikali chyby The process cannot access the file
        //var f = new FileInfo(rel);
        //var result = f.LastWriteTime;
        //return result;
        return DateTime.MinValue;
    }
    /// <summary>
    /// Attempts to delete a path whether it is a file or directory.
    /// </summary>
    /// <param name="value">The value to process.</param>
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
    /// <summary>
    ///     Before start you can create instance of PowershellRunner to try do it with PS
    ///     
    /// Vrátí true value případě chyby, jinak false
    /// </summary>

    /// <returns></returns>
    /// <param name="value">The value to process.</param>
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
    /// <summary>
    /// Wraps a search term with wildcards if it contains only letters.
    /// </summary>
    /// <param name="extension">The extension parameter.</param>
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
    /// <summary>
    /// Retun null if serie is not defined
    /// </summary>
    /// <param name="fnwoe"></param>
    /// <param name="ss"></param>
    /// <returns></returns>
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
    ///     Get number higher by one from the number filenames with highest value (as 3.txt)
    /// </summary>
    /// <param name="folder"></param>

    /// <param name="ext"></param>
    /// <param name="fileName">The file name.</param>
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
    public static bool? ExistsDirectoryNull(string directoryPath)
    {
        return ExistsDirectoryNull(directoryPath, false);
    }
    /// <summary>
    /// Returns null if the directory does not exist.
    /// </summary>
    /// <param name="directoryPath">The directory path to check.</param>
    /// <param name="isFalseIfContainsNoFile">Whether to return false if the directory exists but contains no files.</param>
    public static bool? ExistsDirectoryNull(string directoryPath, bool isFalseIfContainsNoFile = false)
    {
        return ExistsDirectory(directoryPath, isFalseIfContainsNoFile);
    }
    /// <summary>
    /// Determines whether the specified directory exists on disk.
    /// </summary>
    /// <param name="directoryPath">The directory path to check.</param>
    /// <param name="isFalseIfContainsNoFile">Whether to return false if the directory exists but contains no files.</param>
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
    /// <summary>
    /// Converts a nullable boolean to a SearchOption value.
    /// </summary>
    /// <param name="recursive">The recursive parameter.</param>
    public static SearchOption ToSearchOption(bool? recursive)
    {
        return recursive.GetValueOrDefault() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    }
    /// <summary>
    /// Writes text content to a file.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <param name="content">The content parameter.</param>
    public static async Task WriteAllText(string path, string content)
    {
        await File.WriteAllTextAsync(path, content);
    }
    /// <summary>
    /// Determines if all paths in the list are in the same folder.
    /// </summary>
    /// <param name="paths">The paths parameter.</param>
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
    /// Creates a file with content based on a template with placeholder replacement.
    /// </summary>
    /// <param name="folder">The directory path.</param>
    /// <param name="files">The files parameter.</param>
    /// <param name="ext">The ext parameter.</param>
    /// <param name="templateFromContent">The templateFromContent parameter.</param>
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
    /// Checks if a string contains invalid file name characters.
    /// </summary>
    /// <param name="arg">The arg parameter.</param>
    public static bool ContainsInvalidFileNameChars(string arg)
    {
        foreach (var item in invalidFileNameStringsReadonly)
            if (arg.Contains(item))
                return true;
        return false;
    }
    /// <summary>
    /// Numbers files by their modification date.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic messages.</param>
    /// <param name="folder">The directory path.</param>
    /// <param name="searchPattern">The searchPattern parameter.</param>
    /// <param name="so">The so parameter.</param>
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
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>

    /// <returns></returns>
    /// <param name="path">The file or directory path.</param>
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
    /// <summary>
    /// Detects the path delimiter used in a path string.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static Tuple<bool, bool> DetectPathDelimiter(string path)
    {
        var containsFs = path.Contains("/");
        var containsBs = path.Contains("\\");
        if (containsBs && containsFs) throw new Exception("Path contains both fs & bs");
        return Tuple.Create(containsFs, containsBs);
    }
    /// <summary>
    /// Detects the path delimiter character used in a path string.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
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
    ///     Usage: Exceptions.IsNotWindowsPathFormat
    /// </summary>
    /// <param name="argValue"></param>
    /// <returns></returns>
    public static bool IsWindowsPathFormat(string argValue)
    {
        PathFormatDetectorService pathFormatDetector = new(NullLogger.Instance);
        return pathFormatDetector.IsWindowsPathFormat(argValue);
    }
    #endregion
    #region MakeUncLongPath
    /// <summary>
    /// Converts a path to UNC long path format to support paths longer than 260 characters.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static string MakeUncLongPath(string path)
    {
        return MakeUncLongPath(ref path);
    }
    /// <summary>
    /// Converts a path to UNC long path format to support paths longer than 260 characters.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static string MakeUncLongPath(ref string path)
    {
        if (!path.StartsWith(@"\\?\"))
        {
            // value ASP.net mi vrátilo u každé directory.exists false. Byl jsem pod ApplicationPoolIdentity value IIS a bylo nastaveno Full Control pro IIS AppPool\DefaultAppPool
#if !ASPNET
            //  asp.net / vps nefunguje, ve windows store apps taktéž, NECHAT TO TRVALE ZAKOMENTOVANÉ
            // value asp.net toto způsobí akorát zacyklení, IIS začne vyhazovat 0xc00000fd, pak už nejde načíst jediná stránka
            //path = @"\\?\" + path;
#endif
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
    /// <summary>
    /// Converts the first character of a path to uppercase.
    /// </summary>
    /// <param name="result">The result parameter.</param>
    public static string FirstCharUpper(ref string result)
    {
        if (IsWindowsPathFormat(result)) result = SH.FirstCharUpper(result);
        return result;
    }
    /// <summary>
    /// Converts the first character of a path to uppercase.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <param name="only">Whether to only uppercase the first character and lowercase the rest.</param>
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
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>

    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="filePath">The file path.</param>
    public static void GetPathAndFileName(string filePath, out string path, out string file)
    {
        path = WithEndSlash(GetDirectoryName(filePath));
        file = GetFileName(filePath);
    }
    /// <summary>
    /// Gets the file name from a full path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public static string GetFileName(string filePath)
    {
        return PathMs.GetFileName(filePath.TrimEnd(Path.DirectorySeparatorChar));
    }
    /// <summary>
    /// Normalizes a file extension by converting to lowercase and removing the leading dot.
    /// </summary>
    /// <param name="extension">The file extension to normalize.</param>
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
    //            if (FilesWhichSurelyExists.Contains(selectedFile))
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
    //                        var count = string.Empty;
    //                        try
    //                        {
    //                            count = File.ReadAllTextAsync(selectedFile);
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
    ///// <summary>
    ///// Cant return with end slash becuase is working also with files
    ///// Use this than Path.Combine which if argument starts with backslash ignore all arguments before this
    ///// </summary>
    ///// <param name="upFolderName"></param>
    ///// <param name="dirNameDecoded"></param>
    //public static string Combine(params string[] text)
    //{
    //    return CombineWorker(true, text);
    //}
    ///// <summary>
    ///// Cant return with end slash becuase is working also with files
    ///// </summary>
    ///// <param name="FirstCharUpper"></param>
    ///// <param name="s"></param>
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
    /// <summary>
    ///     Usage: FileWasntFoundInDirectory
    ///     Vrátí cestu a název souboru text ext
    /// </summary>



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
    /// <summary>
    ///     Do A1 se dává buď celá cesta ke souboru, nebo jen jeho název(může být i včetně neomezeně přípon)
    ///     A2 říká, zda se má vrátit plná cesta ke souboru A1, upraví se pouze samotný název souboru
    ///     Works for brackets, not dash
    /// </summary>
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
    /// <summary>
    /// 1 = filename without serie
    /// 2 = has serie
    /// </summary>

    /// <param name="path"></param>
    /// <param name="serieStyle"></param>
    /// <param name="a1IsWithPath">Whether the first argument includes a path.</param>
    /// <returns></returns>
    public static (string, bool) GetNameWithoutSeriesNoOut(string path, bool a1IsWithPath, SerieStyleFS serieStyle)
    {
        int serie;
        var result = GetNameWithoutSeries(path, a1IsWithPath, out var hasSerie, serieStyle, out serie);
        return (result, hasSerie);
    }
    /// <summary>
    /// Extracts the base name from a file name by removing series suffixes.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    /// <param name="a1IsWithPath">The a1IsWithPath parameter.</param>
    /// <param name="hasSerie">The hasSerie parameter.</param>
    /// <param name="serieStyle">The serieStyle parameter.</param>
    public static string GetNameWithoutSeries(string path, bool a1IsWithPath, out bool hasSerie, SerieStyleFS serieStyle)
    {
        int serie;
        return GetNameWithoutSeries(path, a1IsWithPath, out hasSerie, serieStyle, out serie);
    }
    /// <summary>
    ///     Vrací vždy text příponou
    ///     Do A1 se dává buď celá cesta ke souboru, nebo jen jeho název(může být i včetně neomezeně přípon)
    ///     A2 říká, zda se má vrátit plná cesta ke souboru A1, upraví se pouze samotný název souboru
    ///     When file has unknown extension, return SE
    ///     Default for A4 was bracket
    /// </summary>

    /// <param name="a1IsWithPath">Whether the path includes the directory path.</param>
    /// <param name="hasSerie">Output flag indicating whether a series suffix was found.</param>
    /// <param name="path">The file path to extract the name from.</param>
    /// <param name="serieStyle">The serie naming style to use.</param>
    /// <param name="serie">Output value of the series number found, or -1 if none.</param>
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
    /// <summary>
    /// Removes the underscore-style series suffix from a file name.
    /// </summary>
    /// <param name="data">The data parameter.</param>
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
    /// Deletes a file from disk, attempting to handle locked files.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public static void DeleteFile(string filePath)
    {
        File.Delete(filePath);
    }
    ///// <summary>
    ///// Vrátí cestu a název souboru text ext
    ///// </summary>
    ///// <param name="fn"></param>
    ///// <param name="path"></param>
    ///// <param name="file"></param>
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
    /// <summary>
    ///     Vrátí cestu a název souboru bez ext a ext
    ///     All returned is normal case
    /// </summary>

    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    /// <param name="filePath">The file path.</param>
    public static void GetPathAndFileNameWithoutExtension(string filePath, out string path, out string file, out string ext)
    {
        path = Path.GetDirectoryName(filePath) + '\\';
        file = GetFileNameWithoutExtension(filePath);
        ext = Path.GetExtension(filePath);
    }
    /// <summary>
    /// Returns the full path without the file extension.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static string PathWithoutExtension(string path)
    {
        string path2, file, ext;
        GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
        return Combine(path2, file);
    }
    /// <summary>
    /// Gets the fully qualified path for a relative path.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static string GetFullPath(string path)
    {
        var result = Path.GetFullPath(path);
        FirstCharUpper(ref result);
        return result;
    }
    /// <summary>
    /// Converts a file path to its parent directory path.
    /// </summary>
    /// <param name="dir">The dir parameter.</param>
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
    /// <param name="directoryPath">The directory path to check</param>
    /// <param name="isFalseIfContainsNoFile">Return false if directory contains no files</param>
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
    /// <summary>
    /// List of files that are confirmed to exist on disk.
    /// </summary>
    public static List<string> FilesWhichSurelyExists = new();
    /// <summary>
    ///     Dont check for size
    ///     Into A2 is good put true - when storage was fulled, all new files will be written with zero size. But then failing
    ///     because HtmlNode as null - empty string as input
    ///     But when file is big, like backup of DB, its better false.. Then will be avoid reading whole file to determining
    ///     their size and totally blocking HW resources on VPS
    ///     A2 must be false otherwise read file twice
    ///     Change falseIfSizeZeroOrEmpty = false. Its extremely resource intensive
    /// </summary>
    /// <param name="selectedFile">The file path to check for existence.</param>
    /// <param name="falseIfSizeZeroOrEmpty">Whether to return false if the file exists but has zero size or is empty.</param>
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
#if ASYNC
                    await
#endif
                        File.ReadAllTextAsync(selectedFile);
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
    /// <summary>
    ///     Cant return with end slash becuase is working also with files
    ///     Use this than Path.Combine which if argument starts with backslash ignore all arguments before this
    /// </summary>
    /// <param name="parts">The path parts to combine.</param>


    public static string Combine(params string[] parts)
    {
        //return Path.Combine(paths);
        return CombineWorker(true, false, parts);
    }
    /// <summary>
    /// Combines multiple path segments into a file path.
    /// </summary>
    /// <param name="parts">The parts parameter.</param>
    public static string CombineFile(params string[] parts)
    {
        return CombineWorker(true, true, parts);
    }
    /// <summary>
    /// Combines multiple path segments into a directory path.
    /// </summary>
    /// <param name="parts">The parts parameter.</param>
    public static string CombineDir(params string[] parts)
    {
        return CombineWorker(true, false, parts);
    }
    /// <summary>
    ///     Cant return with end slash becuase is working also with files
    /// </summary>
    /// <param name="isFirstCharUpper">Whether to uppercase the first character</param>
    /// <param name="file">Whether the path is a file (true) or directory (false)</param>
    /// <param name="paths">Path parts to combine</param>
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
    /// <summary>
    /// Calculates the total size of all files in a folder.
    /// </summary>
    /// <param name="path">The file or directory path.</param>
    public static long GetFolderSize(string path)
    {
        return GetFolderSize(new DirectoryInfo(path));
    }
    /// <summary>
    /// Calculates the total size of all files in a folder.
    /// </summary>
    /// <param name="directoryInfo">The directoryInfo parameter.</param>
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
    /// <summary>
    /// Groups file paths by their file name.
    /// </summary>
    /// <param name="filesInSubfolders">The filesInSubfolders parameter.</param>
    public static Dictionary<string, List<string>> GroupFilesByName(List<string> filesInSubfolders)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var item in filesInSubfolders) DictionaryHelper.AddOrCreate(result, Path.GetFileName(item), item);
        return result;
    }
    /// <summary>
    /// Finds which base path contains the specified path.
    /// </summary>
    /// <param name="basePaths">The basePaths parameter.</param>
    /// <param name="path">The file or directory path.</param>
    public static string? BasePath(List<string> basePaths, string path)
    {
        foreach (var item in basePaths)
            if (path.Contains(item))
                return item;
        return null;
    }
    /// <summary>
    /// Determines if a folder contains any files or subdirectories.
    /// </summary>
    /// <param name="folder">The directory path.</param>
    public static bool HasAnyFoldersOrFiles(string folder)
    {
        return Directory.GetFiles(folder).Length > 0 ||
               Directory.GetDirectories(folder).Length > 0;
    }
    /// <summary>
    /// Moves a directory without recursion.
    /// </summary>
    /// <param name="sourcePath">The sourcePath parameter.</param>
    /// <param name="targetPath">The targetPath parameter.</param>
    /// <param name="directoryMoveCollisionOption">The directoryMoveCollisionOption parameter.</param>
    /// <param name="fileMoveCollisionOption">The fileMoveCollisionOption parameter.</param>
    public static void MoveDirectoryNoRecursive(string sourcePath, string targetPath, DirectoryMoveCollisionOption directoryMoveCollisionOption, object fileMoveCollisionOption)
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