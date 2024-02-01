
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
        throw new NotImplementedException();

        //var v = ConvertCamelConventionWithNumbers.ToConvention(nameOfPage);
        //v = FS.ReplaceInvalidFileNameChars(v);
        //return v;
    }

    public static bool IsFileHasKnownExtension(string relativeTo)
    {
        throw new NotImplementedException();

        //AllExtensionsHelper.Initialize(true);

        //var ext = Path.GetExtension(relativeTo);
        //ext = FS.NormalizeExtension2(ext);

        //return AllExtensionsHelperWitshoutDot.allExtensionsWithoutDot.ContainsKey(ext);
    }

    #endregion
}
