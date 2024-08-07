namespace SunamoFileSystem;

public class SelectFromManyHelper<T>
{
    private readonly ISelectFromMany<T> _selectFromManyControl;
    public string defaultFileForLeave;
    public string defaultFileSize;

    public Dictionary<string, string> filesWithSize = new();
    public bool sufficientFileName;

    public SelectFromManyHelper(ISelectFromMany<T> selectFromManyControl)
    {
        _selectFromManyControl = selectFromManyControl;
    }

    #region Files

    public void InitializeByFolder(bool sufficientFileName, string defaultFileForLeave, string folderForSearch)
    {
        filesWithSize.Clear();
        SetBasicVariable(sufficientFileName, defaultFileForLeave);

        var fn = Path.GetFileName(defaultFileForLeave);
        var files = Directory.GetFiles(folderForSearch, fn, SearchOption.AllDirectories).ToList();

        ProcessFilesWithoutSize(files);
        _selectFromManyControl.AddControls();
    }

    private void ProcessFilesWithoutSize(List<string> files)
    {
        if (sufficientFileName)
            foreach (var item in files)
                filesWithSize.Add(item, null);
        else
            foreach (var item in files)
                filesWithSize.Add(item, FS.GetSizeInAutoString(new FileInfo(item).Length, ComputerSizeUnits.B));
    }

    private void SetBasicVariable(bool sufficientFileName, string defaultFileForLeave)
    {
        this.sufficientFileName = sufficientFileName;
        this.defaultFileForLeave = defaultFileForLeave;

        if (!sufficientFileName)
            defaultFileSize = FS.GetSizeInAutoString(new FileInfo(defaultFileForLeave).Length, ComputerSizeUnits.B);
    }

    public void InitializeByFiles(bool sufficientFileName, string defaultFileForLeave, List<string> files)
    {
        filesWithSize.Clear();
        SetBasicVariable(sufficientFileName, defaultFileForLeave);

        ProcessFilesWithoutSize(files);
        _selectFromManyControl.AddControls();
    }

    public void InitializeByFilesWithSize(bool sufficientFileName, string defaultFileForLeave,
        Dictionary<string, long> files)
    {
        filesWithSize.Clear();
        SetBasicVariable(sufficientFileName, defaultFileForLeave);

        foreach (var item in files)
            filesWithSize.Add(item.Key, FS.GetSizeInAutoString(item.Value, ComputerSizeUnits.B));
        _selectFromManyControl.AddControls();
    }

    #endregion
}