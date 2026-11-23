namespace SunamoFileSystem;
using PathMs = Path;
using TF = SunamoFileSystem._sunamo.SunamoFileIO.TF;

public partial class FS
{

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
}