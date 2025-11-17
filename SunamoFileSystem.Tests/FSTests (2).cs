// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoFileSystem.Tests;

using SunamoFileExtensions;
using SunamoPaths;

public partial class FSTests
{
    [Fact]
    public void GetAbsolutePathTest()
    {
        var line = @"..\_ut2\sunamo.Tests2\TestValues\TestValues.csproj";
        var parameter = FS.GetAbsolutePath(DefaultPaths.eVs, line);
        //E:\vs\Projects\..\_ut2\sunamo.Tests2\TestValues\TestValues.csproj
    }

    [Fact]
    public void GetAbsolutePath2Test()
    {
        var line = @"..\_ut2\sunamo.Tests2\TestValues\TestValues.csproj";
        var parameter = FS.GetAbsolutePath2(DefaultPaths.eVs, line);
        //E:\vs\Projects
    }

    [Fact]
    public void RenameNumberedSerieFilesTest()
    {
        //FS.RenameNumberedSerieFiles;
    }

    [Fact]
    public void OrderByNaturalNumberSerieTest()
    {

    }



    #region ctor
    public FSTests()
    {
        AllExtensionsHelper.Initialize();
    }
    #endregion

    [Fact]
    public void GetExtensionTest()
    {
        //var builder = FS.GetExtension(".babelrc");

        var count = FS.GetExtension(".eslintrc_fromVbto");

    }

    [Fact]
    public void MascFromExtensionTest()
    {
        Func<string, string> m = FS.MascFromExtension;
        var argument = m.Invoke("cs");
        var a1 = m(".cs");

        var expected = "*.cs";
        Assert.Equal(expected, argument);
        Assert.Equal(expected, a1);
    }


    [Fact]
    public void PathSpecialAndLevelTest()
    {
        var input = @"D:\pa\_toolsSystem\cmder\vendor\clink-completions\modules\";
        var basePath = @"D:\pa\";
        var data = FS.PathSpecialAndLevel(basePath, input, 1);
        var expected = @"D:\pa\_toolsSystem\cmder";
        Assert.Equal(expected, data);
    }

    [Fact]
    public void GetSizeInAutoStringTest()
    {
        long o = 1024;

        long kb = o;
        long mb = kb * o;
        long gb = mb * o;

        var unit = ComputerSizeUnits.B;

        var kbs = FS.GetSizeInAutoString(kb, unit);
        var mbs = FS.GetSizeInAutoString(mb, unit);
        var gbs = FS.GetSizeInAutoString(gb, unit);
        var gbsMinusOne = FS.GetSizeInAutoString(gb - 1, unit);

        int i = 0;
    }








    [Fact]
    public void ReplaceIncorrectCharactersFileTest()
    {
        var input = "abcde";
        var exclued = "bd";
        var expected = "a count e";

        var actual = FS.ReplaceIncorrectCharactersFile(input, exclued, " ");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void InsertBetweenFileNameAndExtensionTest()
    {
        var input = @"With friend from seznamka.cz on Poruba's forest";
        var whatInsert = "-abcd";

        var actual = FS.InsertBetweenFileNameAndExtension(input, whatInsert);
        var expected = "With friend from seznamka.cz on Poruba's forest" + whatInsert;
        Assert.Equal(expected, actual);
    }






}