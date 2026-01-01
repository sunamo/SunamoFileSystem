namespace SunamoFileSystem._sunamo.SunamoShared.Args;

/// <summary>
/// Arguments for getting files from every folder
/// </summary>
internal class GetFilesEveryFolder : GetFilesMoreMascArgs
{
    /// <summary>
    /// If true, trims the first argument path
    /// </summary>
    internal bool TrimFirstPath { get; set; } = false;
}