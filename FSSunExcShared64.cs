
namespace SunamoFileSystem;

using SunamoFileSystem._sunamo;
using SunamoFileSystemNoDeps;

public partial class FS
{
    #region For easy copy from FSShared64.cs
    /// <summary>
    /// Convert to UNC path
    /// </summary>
    /// <param name="item"></param>
    public static bool ExistsDirectoryWorker(string item, bool _falseIfContainsNoFile = false)
    {
        // Not working, flags from GeoCachingTool wasnt transfered to standard
#if NETFX_CORE
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exc.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
#endif
#if WINDOWS_UWP
ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exc.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
#endif

        if (item == Consts.UncLongPath || item == string.Empty)
        {
            return false;
        }

        var item2 = MakeUncLongPath(item);

        // Directory.Exists if pass SE or only start of Unc return false
        var result = Directory.Exists(item2);
        if (_falseIfContainsNoFile)
        {
            if (result)
            {
                var f = GetFiles(item, "*", SearchOption.AllDirectories).Count;
                result = f > 0;
            }
        }
        return result;
    }

    public static List<string> filesWhichSurelyExists = new List<string>();


    /// <summary>
    /// Dont check for size
    /// Into A2 is good put true - when storage was fulled, all new files will be written with zero size. But then failing because HtmlNode as null - empty string as input
    /// But when file is big, like backup of DB, its better false.. Then will be avoid reading whole file to determining their size and totally blocking HW resources on VPS
    ///
    /// A2 must be false otherwise read file twice
    ///
    /// Change falseIfSizeZeroOrEmpty = false. Its extremely resource intensive
    /// </summary>
    /// <param name="selectedFile"></param>
    public static
#if ASYNC
    async Task<bool>
#else
bool
#endif
    ExistsFile(string selectedFile, bool falseIfSizeZeroOrEmpty)
    {
        selectedFile = SH.FirstCharUpper(selectedFile);
        //ThrowEx.FirstLetterIsNotUpper(selectedFile);

        if (filesWhichSurelyExists.Contains(selectedFile))
        {
            return true;
        }

#if DEBUG

#endif

        if (selectedFile == Consts.UncLongPath || selectedFile == string.Empty)
        {
            return false;
        }

        FS.MakeUncLongPath(ref selectedFile);

        var exists = File.Exists(selectedFile);

        if (falseIfSizeZeroOrEmpty)
        {
            if (!exists)
            {

                return false;
            }
            else
            {
                var ext = Path.GetExtension(selectedFile).ToLower();
                // Musím to kontrolovat jen když je to tmp, logicky
                if (ext == ".tmp")
                {
                    return false;
                }
                else
                {
                    var c = string.Empty;
                    try
                    {
                        c =
#if ASYNC
                        await
#endif
                        File.ReadAllTextAsync(selectedFile);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.StartsWith("The process cannot access the file"))
                        {
                            return true;
                        }

                    }

                    if (c == string.Empty)
                    {
                        // Měl jsem tu chybu že ač exists bylo true, File.ReadAllTextAsync selhalo protože soubor neexistoval.
                        // Vyřešil jsem to kontrolou přípony, snad
                        return false;
                    }
                }
            }
        }
        return exists;
    }


    /// <summary>
    /// Cant return with end slash becuase is working also with files
    /// Use this than Path.Combine which if argument starts with backslash ignore all arguments before this
    /// </summary>
    /// <param name="upFolderName"></param>
    /// <param name="dirNameDecoded"></param>
    public static string Combine(params string[] s)
    {
        //return Path.Combine(s);
        return CombineWorker(true, true, s);
    }

    public static string CombineFile(params string[] s)
    {
        return CombineWorker(true, true, s);
    }

    public static string CombineDir(params string[] s)
    {
        return CombineWorker(true, false, s);
    }

    /// <summary>
    /// Cant return with end slash becuase is working also with files
    /// </summary>
    /// <param name="FirstCharUpper"></param>
    /// <param name="s"></param>
    private static string CombineWorker(bool FirstCharUpper, bool file, params string[] s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            s[i] = s[i].TrimStart(AllChars.bs);
        }
        //s = CA.TrimStartChar(AllChars.bs, s.ToList()).ToArray();
        var result = Path.Combine(s);
        if (result[2] != AllChars.bs)
        {
            result = result.Insert(2, AllStrings.bs);
        }
        if (FirstCharUpper)
        {
            result = SH.FirstCharUpper(ref result);
        }
        else
        {
            result = SH.FirstCharUpper(ref result);
        }
        if (!file)
        {
            // Cant return with end slash becuase is working also with files
            FSND.WithEndSlash(ref result);
        }
        return result;
    }





    public static List<long> GetFilesSizes(List<string> f)
    {
        List<long> sizes = new List<long>();

        foreach (var item in f)
        {
            sizes.Add(new FileInfo(item).Length);
        }

        return sizes;
    }

    public static long GetFolderSize(string path)
    {
        return GetFolderSize(new DirectoryInfo(path));
    }

    public static long GetFolderSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        //
        FileInfo[] fis = null;
        try
        {
            fis = d.GetFiles();
        }
        catch (DirectoryNotFoundException ex)
        {
            fis = new FileInfo[0];
            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api a zbylé složky v něm jsou junctiony které ale ztratily svůj cíl
        }

        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }

        // Add subdirectory sizes.
        DirectoryInfo[] dis = null;
        try
        {
            dis = d.GetDirectories();
        }
        catch (DirectoryNotFoundException ex)
        {
            dis = new DirectoryInfo[0];
            //System.IO.DirectoryNotFoundException: 'Could not find a part of the path 'C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@vbto\api'.' - api a zbylé složky v něm jsou junctiony které ale ztratily svůj cíl
        }

        foreach (DirectoryInfo di in dis)
        {
            size += GetFolderSize(di);
        }
        return size;
    }

    public static Dictionary<string, List<string>> GroupFilesByName(List<string> filesInSubfolders)
    {
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

        foreach (var item in filesInSubfolders)
        {
            DictionaryHelper.AddOrCreate(result, Path.GetFileName(item), item);
        }

        return result;
    }

    public static string BasePath(List<string> ms, string path)
    {
        foreach (var item in ms)
        {
            if (path.Contains(item))
            {
                return item;
            }
        }

        return null;
    }

    public static bool HasAnyFoldersOrFiles(string folderWhereToCreate)
    {
        return GetFiles(folderWhereToCreate).Count > 0 || Directory.GetDirectories(folderWhereToCreate).Length > 0;
    }

    public static
#if ASYNC
    async Task<Dictionary<string, string>>
#else
Dictionary<string, string>
#endif
    GetFilesWithContentInDictionary(string item, string v, SearchOption allDirectories)
    {
        Dictionary<string, string> r = new Dictionary<string, string>();

        var f = GetFiles(item, v, allDirectories);
        foreach (var item2 in f)
        {
            r.Add(item2,
#if ASYNC
            await
#endif
            File.ReadAllTextAsync(item2));
        }

        return r;
    }

    public static List<string> GetFoldersWhichContainsFiles(string d, string masc, SearchOption topDirectoryOnly)
    {
        var f = GetFolders(d);
        List<string> result = new List<string>();

        foreach (var item in f)
        {
            var files = GetFiles(item, masc, topDirectoryOnly);
            if (files.Count != 0)
            {
                result.Add(item);
            }
        }

        return result;
    }

    ///// <summary>
    ///// Use FirstCharUpper instead
    ///// </summary>
    ///// <param name="result"></param>
    //private static string FirstCharUpper(ref string result)
    //{
    //    return se.SH.FirstCharUpper(ref result);

    //}

    //public static bool IsWindowsPathFormat(string argValue)
    //{
    //    return se.FS.IsWindowsPathFormat(argValue);
    //}
    #endregion
}
