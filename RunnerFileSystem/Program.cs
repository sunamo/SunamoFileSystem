namespace RunnerFileSystem;

using Microsoft.Extensions.DependencyInjection;
using sunamo.Tests.Helpers.FileSystem;
using SunamoCl.SunamoCmd;
using SunamoDependencyInjection;

internal partial class Program
{
    const string appName = "ToNugets.Cmd.Roslyn";

    static ServiceCollection Services = new();
    static ServiceProvider Provider;

    static Program()
    {
        Services.AddServicesEndingWithService();

        CmdBootStrap.AddILogger(Services, true, null, appName);

        Provider = Services.BuildServiceProvider();
    }

    static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }

    static async Task MainAsync(string[] args)
    {
        var runnedAction = await CmdBootStrap.RunWithRunArgs(new SunamoCl.SunamoCmd.Args.RunArgs
        {
            AddGroupOfActions = AddGroupOfActions,
            AskUserIfRelease = true,
            Args = args,
            RunInDebugAsync = RunInDebugAsync,
            ServiceCollection = Services,
            IsDebug =
#if DEBUG
          true
#else
false
#endif
        });

        Console.WriteLine("Finished");
        Console.ReadLine();
    }

    static async Task RunInDebugAsync()
    {
        await Task.Delay(1);

        //FSTests t = new FSTests();
        //t.RenameDirectoryTest();
        //t.DeleteAllEmptyDirectoriesTest(false);
        //t.MoveDirectoryNoRecursiveTest();
        //t.CombineTest();
        //t.MoveFileTest();
        //t.GetFileSerieTest();

        FSManipulationWithoutMockTests t = new FSManipulationWithoutMockTests();
        await t.AllExtensionsInFolders();
    }
}