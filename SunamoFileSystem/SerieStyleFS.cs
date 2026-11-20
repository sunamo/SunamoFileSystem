namespace SunamoFileSystem;

// All styles here are taken from FSTests
/// <summary>
/// Represents the serie style FS type.
/// </summary>
public enum SerieStyleFS
{
    // abc-1.txt
    // abc-1.txt.txt
    /// <summary>
    /// The dash value.
    /// </summary>
    Dash,

    // abc(1).txt
    /// <summary>
    /// The brackets value.
    /// </summary>
    Brackets,

    // MainPage.xaml_008.cs
    // abc_001_01.txt.txt
    /// <summary>
    /// The underscore value.
    /// </summary>
    Underscore,

    // Match all serie styles.
    /// <summary>
    /// The all value.
    /// </summary>
    All
}
