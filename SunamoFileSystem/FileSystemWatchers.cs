namespace SunamoFileSystem;

/// <summary>
/// Represents the file system watchers type.
/// </summary>
public class FileSystemWatchers
{
    private static readonly bool Watch = false;

    private FileSystemWatcher _fileSystemWatcher = default!;
    private readonly Action<string, bool> _onStart;
    private readonly Action<string, bool> _onStop;

    // In key are folders (never files), in value instance
    private readonly FsWatcherDictionary<string, FileSystemWatcher> _watchers = new();

    private readonly Dictionary<WatcherChangeTypes, string> _lastProcessedFile = new();
    private readonly Dictionary<WatcherChangeTypes, string> _lastProcessedFileOld = new();

    /// <summary>
    /// Initializes a new instance of the file system watchers class.
    /// </summary>
    /// <param name="onStart">The on start.</param>
    /// <param name="onStop">The on stop.</param>
    public FileSystemWatchers(Action<string, bool> onStart, Action<string, bool> onStop)
    {
        _onStart = onStart;
        _onStop = onStop;

        if (Watch)
        {
            var changeTypes = ((WatcherChangeTypes[])Enum.GetValues(typeof(WatcherChangeTypes)));
            foreach (var item in changeTypes)
            {
                _lastProcessedFile.Add(item, string.Empty);
                _lastProcessedFileOld.Add(item, string.Empty);
            }
        }
    }

    // Checks whether folder is already being monitored
    // Is called from ProcessFile
    /// <summary>
    /// Start.
    /// </summary>
    /// <param name="path">The path.</param>
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

    // Is called only from Start
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
    /// Stop.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="isFromFileSystemWatcher">The is from file system watcher.</param>
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

    private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        if (Watch)
        {
            if (_lastProcessedFile[e.ChangeType] == e.FullPath) return;

            if (_lastProcessedFileOld[e.ChangeType] == e.OldFullPath) return;

            _lastProcessedFile[e.ChangeType] = e.FullPath;
            _lastProcessedFileOld[e.ChangeType] = e.OldFullPath;

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

    private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        if (Watch)
        {
            if (_lastProcessedFile[e.ChangeType] == e.FullPath) return;

            _lastProcessedFile[e.ChangeType] = e.FullPath;

            if (File.Exists(e.FullPath))
            {
                _onStop.Invoke(e.FullPath, true);
                _onStart.Invoke(e.FullPath, true);
            }
        }
    }

    private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        if (Watch)
        {
            if (_lastProcessedFile[e.ChangeType] == e.FullPath) return;

            _lastProcessedFile[e.ChangeType] = e.FullPath;

            _onStop.Invoke(e.FullPath, true);
        }
    }
}
