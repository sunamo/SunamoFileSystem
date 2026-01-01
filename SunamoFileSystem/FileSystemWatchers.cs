namespace SunamoFileSystem;

/// <summary>
/// File system watcher for monitoring file changes in multiple directories
/// </summary>
public class FileSystemWatchers
{
    private static readonly bool Watch = false;

    private FileSystemWatcher _fileSystemWatcher = default!;
    private readonly Action<string, bool> _onStart;
    private readonly Action<string, bool> _onStop;

    /// <summary>
    ///     In key are folders (never files), in value instance
    /// </summary>
    private readonly FsWatcherDictionary<string, FileSystemWatcher> _watchers = new();


    private readonly Dictionary<WatcherChangeTypes, string> lastProcessedFile = new();
    private readonly Dictionary<WatcherChangeTypes, string> lastProcessedFileOld = new();

    /// <summary>
    /// Initializes a new instance of the FileSystemWatchers class
    /// </summary>
    /// <param name="onStart">Action to invoke when file monitoring starts</param>
    /// <param name="onStop">Action to invoke when file monitoring stops</param>
    public FileSystemWatchers(Action<string, bool> onStart, Action<string, bool> onStop)
    {
        _onStart = onStart;
        _onStop = onStop;

        if (Watch)
        {

            var changeTypes = Enum.GetValues<WatcherChangeTypes>();
            foreach (var item in changeTypes)
            {
                lastProcessedFile.Add(item, string.Empty);
                lastProcessedFileOld.Add(item, string.Empty);
            }
        }
    }

    /// <summary>
    /// Starts monitoring the specified folder for file changes
    /// Checks whether folder is already being monitored
    /// Is called from ProcessFile
    /// </summary>
    /// <param name="path">The folder path to start monitoring</param>
    public void Start(string path)
    {
        if (Watch)
        {
            // Adding handlers - must wrap up all

            if (!_watchers.ContainsKey(path))
            {
                var fileSystemWatcher = RegisterSingleFolder(path);


                DictionaryHelper.AddOrSet(_watchers, path, fileSystemWatcher);
            }
            else
            {
                _watchers[path].EnableRaisingEvents = true;
            }
        }
    }

    /// <summary>
    /// Registers a single folder for file system monitoring
    /// Is called only from Start
    /// </summary>
    /// <param name="path">The folder path to register</param>
    /// <returns>The configured FileSystemWatcher instance</returns>
    private FileSystemWatcher RegisterSingleFolder(string path)
    {
        if (Watch)
        {
            // A1 must be directory, never file
            _fileSystemWatcher = new FileSystemWatcher(path);
            _fileSystemWatcher.Filter = "*.cs";

            _fileSystemWatcher.IncludeSubdirectories = true;

            _fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes |
                                              NotifyFilters.CreationTime |
                                              NotifyFilters.FileName |
                                              NotifyFilters.LastAccess |
                                              NotifyFilters.LastWrite |
                                              NotifyFilters.Size |
                                              NotifyFilters.Security;

            _fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        return _fileSystemWatcher;
    }

    /// <summary>
    /// Stops monitoring the specified folder
    /// </summary>
    /// <param name="path">The folder path to stop monitoring</param>
    /// <param name="isFromFileSystemWatcher">Indicates if this was called from FileSystemWatcher event</param>
    public void Stop(string path, bool isFromFileSystemWatcher = false)
    {
        if (Watch)
        {
            _onStop.Invoke(path, isFromFileSystemWatcher);

            var fileSystemWatcher = _watchers[path];

            _watchers.Remove(path);

            fileSystemWatcher.EnableRaisingEvents = false;
        }
    }

    /// <summary>
    /// Handles file rename events
    /// </summary>
    private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        if (Watch)
        {
            if (lastProcessedFile[e.ChangeType] == e.FullPath) return;

            if (lastProcessedFileOld[e.ChangeType] == e.OldFullPath) return;

            lastProcessedFile[e.ChangeType] = e.FullPath;
            lastProcessedFileOld[e.ChangeType] = e.OldFullPath;

            var existsNew = false;
            var existsOld = false;

            try
            {
                existsNew = File.Exists(e.FullPath);
            }
            catch (Exception)
            {
            }

            try
            {
                existsOld = File.Exists(e.OldFullPath);
            }
            catch (Exception)
            {
            }

            if (existsOld || existsNew)
            {
                _onStop.Invoke(e.OldFullPath, true);
                _onStart.Invoke(e.FullPath, true);
            }
        }
    }

    /// <summary>
    /// Handles file change events
    /// </summary>
    private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (Watch)
        {
            if (lastProcessedFile[e.ChangeType] == e.FullPath) return;

            lastProcessedFile[e.ChangeType] = e.FullPath;


            if (File.Exists(e.FullPath))
            {
                _onStop.Invoke(e.FullPath, true);
                _onStart.Invoke(e.FullPath, true);
            }
        }
    }

    /// <summary>
    /// Handles file deletion events
    /// </summary>
    private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        if (Watch)
        {
            if (lastProcessedFile[e.ChangeType] == e.FullPath) return;

            lastProcessedFile[e.ChangeType] = e.FullPath;


            _onStop.Invoke(e.FullPath, true);
        }
    }
}