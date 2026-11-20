namespace SunamoFileSystem;
using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

public partial class FS
{

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