namespace sunamo.Tests.Helpers.FileSystem;

public partial class FSTests
{
    #region GetRelativePath
    [Fact]
    public void GetRelativePathTest()
    {
        // potřebuji z A1 abych viděl A2
        var a1 = @"E:\vs\Projects\EverythingClient\"; //c.csproj
        var a2 = @"E:\vs\Projects\PlatformIndependentNuGetPackages\sunamo\sunamo.csproj";


        // 1/ 
        var expected = @"..\sunamo\sunamo\sunamo.csproj";

        var result = string.Empty;

        //a1 a2 ..\sunamo\sunamo\sunamo.csproj
        //a2 a1 ..\..\..\EverythingClient\ - working perfectly even if path ending with backslash
        result = Path.GetRelativePath(a1, a2);
        Assert.Equal(expected, result);


        // 2/ 
        expected = @"..\..\..\EverythingClient\";
        result = Path.GetRelativePath(a2, a1);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetRelativePathTest2()
    {
        // In difference with GetRelativePathTest missing \ on end
        var a1 = @"E:\vs\Projects\EverythingClient";
        var a2 = @"E:\vs\Projects\PlatformIndependentNuGetPackages\sunamo\sunamo.csproj";

        var expected = @"..\sunamo\sunamo\sunamo.csproj";

        var result = string.Empty;

        //a1 a2 ..\sunamo\sunamo\sunamo.csproj
        //a2 a1 ..\..\..\EverythingClient\ - working perfectly even if path ending with backslash
        result = Path.GetRelativePath(a1, a2);
        Assert.Equal(expected, result);

        // In difference with GetRelativePathTest missing \ on end
        expected = @"..\..\..\EverythingClient";
        result = Path.GetRelativePath(a2, a1);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetRelativePathTest3()
    {
        var a1 = @"E:\vs\Projects\PlatformIndependentNuGetPackages";
        var a2 = @"E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoCef\SunamoCef.csproj";

        var expected = @"SunamoCef\SunamoCef.csproj";

        var result = string.Empty;

        //a1 a2 ..\sunamo\sunamo\sunamo.csproj
        //a2 a1 ..\..\..\EverythingClient\ - working perfectly even if path ending with backslash
        result = Path.GetRelativePath(a1, a2);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetRelativePathTest4()
    {
        var a1 = @"E:\vs\Projects\SunamoCzAdmin";
        var a2 = @"E:\vs\Projects\PlatformIndependentNuGetPackages.notmine.web\SearchTextBox.web\SearchTextBox.web.csproj";

        var expected = @"..\sunamo.notmine.web\SearchTextBox.web\SearchTextBox.web.csproj";

        var result = string.Empty;

        //a1 a2 ..\sunamo\sunamo\sunamo.csproj
        //a2 a1 ..\..\..\EverythingClient\ - working perfectly even if path ending with backslash
        result = Path.GetRelativePath(a1, a2);
        Assert.Equal(expected, result);

        var result2 = Path.GetRelativePath(a1, a2);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetRelativePathTest5()
    {
        var a1 = @"C:\repos\_marvin\Module.IASAdministration\Clients\src\packages\data\src\index.tsx";
        var a2 = @"C:\repos\_marvin\Module.IASAdministration\Clients\src\packages\data\src\dataNull\IIdName.ts";

        var expected = @".\dataNull\IIdName.ts";

        var result = Path.GetRelativePath(a1, a2, true);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetRelativePathTest6()
    {
        var a1 = @"C:\repos\_marvin\Module.IASAdministration\Clients\src\packages\data\index.tsx";
        var a2 = @"C:\repos\_marvin\Module.IASAdministration\Clients\src\packages\data\src\dataNull\IIdName.ts";

        var expected = @".\src\dataNull\IIdName.ts";

        var result = Path.GetRelativePath(a1, a2, true);
        Assert.Equal(expected, result);
    }
    #endregion


}
