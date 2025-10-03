namespace SunamoFileSystem.Tests;

using SunamoGetFiles._public.SunamoArgs;

/// <summary>
/// 3 možnosti:
/// 1/ zabalit do csproj archívy 
/// 2/ mockovat 
/// 3/ úplně tyhle testy smazat
/// </summary>
public class FSManipulationWithoutMockTests
{
    ILogger logger = NullLogger.Instance;
    //[Fact]
    public void DeleteSerieDirectoryOrCreateNewTest()
    {
        string folder = @"D:\_Test\sunamo\Helpers\FileSystem\DeleteSerieDirectoryOrCreateNew\";
        FS.DeleteSerieDirectoryOrCreateNew(folder);
    }

    //[Fact]
    public async Task AllExtensionsInFolders()
    {
        string folder = @"D:\_Test\sunamo\Helpers\FileSystem\FS\AllExtensionsInFolders\";

        if (!FS.TryDeleteDirectory(folder))
        {
            throw new Exception("Nepodařilo se smazat složku " + folder);
        }
        FS.CreateFoldersPsysicallyUnlessThere(folder);
        var excepted = CA.ToListString(".html", ".bowerrc", ".php");
        excepted.Sort();

        foreach (var item in excepted)
        {
            for (int i = 0; i < 2; i++)
            {
                await FS.WriteAllText(folder + i + item, "");
            }
        }

        var actual = FS.AllExtensionsInFolders(System.IO.SearchOption.TopDirectoryOnly, folder);
        actual.Sort();

        Assert.Equal<string>(excepted, actual);
    }
    //[Fact]
    public void DeleteEmptyFilesTest()
    {
        string folder = @"D:\_Test\sunamo\Helpers\FileSystem\FS\DeleteEmptyFiles\";
        FS.DeleteEmptyFiles(folder, System.IO.SearchOption.TopDirectoryOnly);
        List<string> actual = FS.OnlyNamesNoDirectEdit(FSGetFiles.GetFilesEveryFolder(logger, folder));
        List<string> excepted = CA.ToListString("ab.txt", "DeleteEmptyFiles.zip");
        Assert.Equal(excepted, actual);
    }
    //[Fact]
    public void DeleteFilesWithSameContent()
    {
        string folder = @"D:\_Test\sunamo\Helpers\FileSystem\FS\DeleteFilesWithSameContent\";
        var files = FSGetFiles.GetFilesEveryFolder(logger, folder, "*.txt", System.IO.SearchOption.AllDirectories, new GetFilesEveryFolderArgs { _trimA1AndLeadingBs = true });
        FS.DeleteFilesWithSameContent(files);
        files = FSGetFiles.GetFilesEveryFolder(logger, folder, "*.txt", System.IO.SearchOption.AllDirectories, new GetFilesEveryFolderArgs { _trimA1AndLeadingBs = true });
        var filesExcepted = CA.ToListString(TestDataTxt.a, TestDataTxt.ab);
        Assert.Equal<string>(filesExcepted, files);
    }
    //[Fact]
    public void DeleteFilesWithSameContentBytes()
    {
        string folder = @"D:\_Test\sunamo\Helpers\FileSystem\FS\DeleteFilesWithSameContentBytes\";
        var files = FSGetFiles.GetFilesEveryFolder(logger, folder, "*.txt", System.IO.SearchOption.AllDirectories, new GetFilesEveryFolderArgs { _trimA1AndLeadingBs = false });
        FS.DeleteFilesWithSameContentBytes(files);
        files = FSGetFiles.GetFilesEveryFolder(logger, folder, "*.txt", System.IO.SearchOption.AllDirectories, new GetFilesEveryFolderArgs { _trimA1AndLeadingBs = true });
        var filesExcepted = CA.ToListString(TestDataTxt.a, TestDataTxt.ab);
        Assert.Equal<string>(filesExcepted, files);
    }
    //[Fact]
    public void DeleteAllEmptyDirectoriesTest()
    {
        string folder = @"D:\_Test\sunamo\sunamo\Helpers\FileSystem\FS\DeleteAllEmptyDirectories\";
        FS.DeleteAllEmptyDirectories(folder);
        int actual = FSGetFolders.GetFoldersEveryFolder(logger, folder, "*", SearchOption.AllDirectories).Count;
        Assert.Equal(2, actual);
    }
}