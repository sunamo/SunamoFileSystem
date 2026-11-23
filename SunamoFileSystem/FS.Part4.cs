namespace SunamoFileSystem;
using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

public partial class FS
{

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
}