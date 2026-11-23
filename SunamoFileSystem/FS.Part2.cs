namespace SunamoFileSystem;
using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

public partial class FS
{

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
}