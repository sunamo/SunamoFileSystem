using SunamoFileSystem.Tests;

namespace RunnerFileSystem;

internal class Program
{
    static void Main()
    {
        FSTests t = new FSTests();
        //t.RenameDirectoryTest();
        //t.DeleteAllEmptyDirectoriesTest(false);
        //t.MoveDirectoryNoRecursiveTest();
        //t.CombineTest();
        //t.MoveFileTest();
        t.GetFileSerieTest();
    }
}
