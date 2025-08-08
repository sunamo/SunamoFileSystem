namespace sunamo.Tests.Helpers.FileSystem;

public partial class FSTests
{
    const string bp = "E:\\";

    #region GetNameWithoutSeries
    /*
     * 
1/ Brackets,
2/ Dash,
3/ Underscore,
    */
    #region Auto recognized style => brackets
    /// <summary>
    /// Passed
    /// 
    /// </summary>
    [Fact]
    public void GetNameWithoutSeries()
    {
        string input1 = bp + "abc(1).txt";
        string expected = bp + "abc.txt";
        string result1 = FS.GetNameWithoutSeries(input1, true);
        Assert.Equal(expected, result1);
    }
    #endregion

    #region 1/ Brackets
    /// <summary>
    /// Passed
    /// 1,2,7 - also with All => OK => OK
    /// </summary>
    [Fact]
    public void GetNameWithoutSeries1()
    {
        bool hasSerie;
        int serie;
        string input = bp + "abc(1).txt";
        string result = "";
        string expected = bp + "abc.txt";
        result = FS.GetNameWithoutSeries(input, true, out hasSerie, SerieStyle.Brackets, out serie);
        Assert.Equal(expected, result);
        Assert.True(hasSerie);
        Assert.Equal(1, serie);

        result = FS.GetNameWithoutSeries(input, true, out hasSerie, SerieStyle.All, out serie);
        Assert.Equal(expected, result);
        Assert.True(hasSerie);
        Assert.Equal(1, serie);
    }
    #endregion

    #region 2/ Dash
    /// <summary>
    /// Passed
    /// 1,2,7 - also with All => OK
    /// </summary>
    [Fact]
    public void GetNameWithoutSeries2()
    {
        bool hasSerie;
        int serie;
        string input = bp + "abc-1.txt";
        string result = "";
        string expected = bp + "abc.txt";
        result = FS.GetNameWithoutSeries(input, true, out hasSerie, SerieStyle.Dash, out serie);
        Assert.Equal(expected, result);
        Assert.True(hasSerie);
        Assert.Equal(1, serie);

        result = FS.GetNameWithoutSeries(input, true, out hasSerie, SerieStyle.All, out serie);
        Assert.Equal(expected, result);
        Assert.True(hasSerie);
        Assert.Equal(1, serie);
    }
    #endregion

    #region 3/ Underscore
    /// <summary>
    /// Passed
    /// 1,2,7 - also with All => OK
    /// </summary>
    [Fact]
    public void GetNameWithoutSeries7()
    {
        bool hasSerie;
        int serie;
        string input = bp + @"MainPage.xaml_008.cs";
        string result = "";
        string expected = bp + @"MainPage.xaml.cs";

        result = FS.GetNameWithoutSeries(input, false, out hasSerie, SerieStyle.Underscore, out serie);
        Assert.Equal(expected, result);
        Assert.True(hasSerie);
        Assert.Equal(8, serie);

        result = FS.GetNameWithoutSeries(input, false, out hasSerie, SerieStyle.All, out serie);
        Assert.Equal(expected, result);
        Assert.True(hasSerie);
        Assert.Equal(8, serie);
    }
    #endregion

    #region Without serie
    /// <summary>
    /// Passed
    /// Test whether work OK when is specified SerieStyle!=All and contains no serie
    /// </summary>
    [Fact]
    public void GetNameWithoutSeries8()
    {
        bool hasSerie;
        int serie;
        string input = @"DSC00711.JPG";
        string result = "";
        string expected = @"DSC00711.jpg";
        result = FS.GetNameWithoutSeries(input, false, out hasSerie, SerieStyle.Brackets, out serie);
        Assert.Equal(expected, result);
        Assert.False(hasSerie);
        Assert.Equal(-1, serie);

        input = bp + "abc.txt";
        expected = bp + "abc.txt";
        result = FS.GetNameWithoutSeries(input, true, out hasSerie, SerieStyle.Dash, out serie);
        Assert.Equal(expected, result);
        Assert.False(hasSerie);
        Assert.Equal(-1, serie);
    }
    #endregion
    #endregion
}
