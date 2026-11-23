namespace SunamoFileSystem;
using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

public partial class FS
{


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
}