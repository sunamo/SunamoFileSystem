namespace SunamoFileSystem;
public partial class FS
{
    #region For easy copy from FSShared.cs

    public static void DeleteFile(string item)
    {
        File.Delete(item);
    }


    ///// <summary>
    ///// Vrátí cestu a název souboru s ext
    ///// </summary>
    ///// <param name="fn"></param>
    ///// <param name="path"></param>
    ///// <param name="file"></param>
    //public static void GetPathAndFileName(string fn, out string path, out string file)
    //{
    //    se.FS.GetPathAndFileName(fn, out path, out file);
    //}



    //public static string WithEndSlash(ref string v)
    //{
    //    return se.FSND.WithEndSlash(ref v);
    //}

    //public static string GetDirectoryName(string rp)
    //{
    //    return se.Path.GetDirectoryName(rp);
    //}

    /// <summary>
    /// Vrátí cestu a název souboru bez ext a ext
    /// All returned is normal case
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    {
        path = Path.GetDirectoryName(fn) + AllChars.bs;
        file = GetFileNameWithoutExtension(fn);
        ext = Path.GetExtension(fn);
    }

    public static string PathWithoutExtension(string path)
    {
        string path2, file, ext;
        GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
        return Combine(path2, file);
    }

    public static string GetFullPath(string vr)
    {
        var result = Path.GetFullPath(vr);
        FirstCharUpper(ref result);
        return result;
    }

    public static void FileToDirectory(ref string dir)
    {
        if (!dir.EndsWith(AllStrings.bs))
        {
            dir = GetDirectoryName(dir);
        }
    }

    ///// <summary>
    ///// Cant name GetAbsolutePath because The call is ambiguous between the following methods or properties: 'CAChangeContent.ChangeContent0(null,List<string>, Func<string, string, string>)' and 'CAChangeContent.ChangeContent0(null,List<string>, Func<string, string>)'
    ///// </summary>
    ///// <param name="a"></param>
    //public static string AbsoluteFromCombinePath(string a)
    //{
    //    return se.FS.AbsoluteFromCombinePath(a);
    //}
    #endregion
}
