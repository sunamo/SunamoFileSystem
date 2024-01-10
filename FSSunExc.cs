namespace SunamoFileSystem;
public partial class FS
{
    #region For easy copy
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static string NormalizeExtension2(string item)
    //{
    //    return se.FS.NormalizeExtension2(item);
    //}

    public static string NonSpacesFilename(string nameOfPage)
    {
        var v = ConvertCamelConventionWithNumbers.ToConvention(nameOfPage);
        v = FS.ReplaceInvalidFileNameChars(v);
        return v;
    }

    public static bool IsFileHasKnownExtension(string relativeTo)
    {
        AllExtensionsHelper.Initialize(true);

        var ext = Path.GetExtension(relativeTo);
        ext = FS.NormalizeExtension2(ext);

        return AllExtensionsHelperWithoutDot.allExtensionsWithoutDot.ContainsKey(ext);
    }

    #endregion
}
