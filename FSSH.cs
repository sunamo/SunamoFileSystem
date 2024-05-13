namespace SunamoFileSystem;




public class FSSH : FSND
{
    static FSSH()
    {
        invalidFileNameStrings = new List<string>(invalidFileNameChars.Count);
        foreach (var item in invalidFileNameChars)
        {
            invalidFileNameStrings.Add(item.ToString());
        }

        s_invalidPathChars = new List<char>(Path.GetInvalidPathChars());
        if (!s_invalidPathChars.Contains(AllChars.slash))
        {
            s_invalidPathChars.Add(AllChars.slash);
        }
        if (!s_invalidPathChars.Contains(AllChars.bs))
        {
            s_invalidPathChars.Add(AllChars.bs);
        }


        s_invalidFileNameChars = new List<char>(invalidFileNameChars);
        s_invalidFileNameCharsString = string.Join("", invalidFileNameChars);
        for (char i = (char)65529; i < 65534; i++)
        {
            s_invalidFileNameChars.Add(i);
        }

        s_invalidCharsForMapPath = new List<char>();
        s_invalidCharsForMapPath.AddRange(s_invalidFileNameChars.ToArray());
        foreach (var item in invalidFileNameChars)
        {
            if (!s_invalidCharsForMapPath.Contains(item))
            {
                s_invalidCharsForMapPath.Add(item);
            }
        }

        s_invalidCharsForMapPath.Remove(AllChars.slash);

        s_invalidFileNameCharsWithoutDelimiterOfFolders = new List<char>(s_invalidFileNameChars.ToArray());

        s_invalidFileNameCharsWithoutDelimiterOfFolders.Remove(AllChars.bs);
        s_invalidFileNameCharsWithoutDelimiterOfFolders.Remove(AllChars.slash);
    }

    public static bool TryDeleteFile(string item)
    {
        // TODO: To all code message logging as here

        try
        {
            // If file won't exists, wont throw any exception
            File.Delete(item);
            return true;
        }
        catch
        {
            //ThisApp.Error(sess.i18n(XlfKeys.FileCanTBeDeleted) + ": " + item);
            return false;
        }
    }

    public static async Task WriteAllTextWithExc(string file, string obsah)
    {
        try
        {
            await File.WriteAllTextAsync(file, obsah);
        }
        catch (Exception ex)
        {
            //TypedSunamoLogger.Instance.Error//(Exceptions.TextOfExceptions(ex));
        }
    }

    public static async void CreateFileIfDoesntExists(string path)
    {
        //CreateFileIfDoesntExists<string, string>(path, null);


        if (!File.Exists(path))
        {
            //TF.WriteAllBytes<StorageFolder, StorageFile>(path, CAG.ToList<byte>(), ac);
            File.WriteAllText(path, "");
        }
    }
    //public static async Task CreateFileIfDoesntExists<StorageFolder, StorageFile>(StorageFile path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    await File.WriteAllBytesAsync(path.ToString(), new Byte[] { });

    //    //if (!ExistsFile<StorageFolder, StorageFile>(path, ac))
    //    //{
    //    //    TF.WriteAllBytes<StorageFolder, StorageFile>(path, CAG.ToList<byte>(), ac);
    //    //}
    //}

    public static string InsertBetweenFileNameAndExtension(string orig, string whatInsert)
    {
        //return InsertBetweenFileNameAndExtension<string, string>(orig, whatInsert, null);

        // Cesta by se zde hodila kvůli FS.CiStorageFile
        // nicméně StorageFolder nevím zda se používá, takže to bude umět i bez toho

        var origS = orig.ToString();

        string fn = Path.GetFileNameWithoutExtension(origS);
        string e = GetExtension(origS);

        if (origS.Contains(AllChars.slash) || origS.Contains(AllChars.bs))
        {
            string p = Path.GetDirectoryName(origS);

            return Path.Combine(p, fn + whatInsert + e);
        }
        return fn + whatInsert + e;
    }

    protected readonly static List<char> invalidFileNameChars = Path.GetInvalidFileNameChars().ToList();
    protected readonly static List<string> invalidFileNameStrings;


    protected static List<char> s_invalidPathChars = null;

    /// <summary>
    /// Field as string because I dont have array and must every time ToArray() to construct string
    /// </summary>
    public static string s_invalidFileNameCharsString = null;
    public static List<char> s_invalidFileNameChars = null;
    protected static List<char> s_invalidCharsForMapPath = null;
    protected static List<char> s_invalidFileNameCharsWithoutDelimiterOfFolders = null;

    /// <summary>
    /// ReplaceIncorrectCharactersFile - can specify char for replace with
    /// ReplaceInvalidFileNameChars - all wrong chars skip
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string ReplaceInvalidFileNameChars(string filename, params char[] ch)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in filename)
        {
            if (!s_invalidFileNameChars.Contains(item) || ch.Contains(item))
            {
                sb.Append(item);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Vrátí vč. cesty
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="whatInsert"></param>
    //public static StorageFile InsertBetweenFileNameAndExtension<StorageFolder, StorageFile>(StorageFile orig, string whatInsert, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    // Cesta by se zde hodila kvůli FS.CiStorageFile
    //    // nicméně StorageFolder nevím zda se používá, takže to bude umět i bez toho

    //    var origS = orig.ToString();

    //    string fn = Path.GetFileNameWithoutExtension(origS);
    //    string e = GetExtension(origS);

    //    if (origS.Contains(AllChars.slash) || origS.Contains(AllChars.bs))
    //    {
    //        string p = Path.GetDirectoryName(origS);

    //        return CiStorageFile<StorageFolder, StorageFile>(Path.Combine(p, fn + whatInsert + e), ac);
    //    }
    //    return CiStorageFile<StorageFolder, StorageFile>(fn + whatInsert + e, ac);
    //}

    /// <summary>
    /// .babelrc etc. return as is. but files which not contains only alphanumeric will be returned when A3 (and A2 is then ignored)
    ///
    /// ALL EXT. HAVE TO BE ALWAYS LOWER
    /// Return in lowercase
    /// </summary>
    /// <param name="v"></param>
    public static string GetExtension(string v, GetExtensionArgs a = null)
    {
        if (a == null)
        {
            a = new GetExtensionArgs();
        }

        string result = "";
        int lastDot = v.LastIndexOf(AllChars.dot);
        if (lastDot == -1)
        {
            return string.Empty;
        }
        int lastSlash = v.LastIndexOf(AllChars.slash);
        int lastBs = v.LastIndexOf(AllChars.bs);
        if (lastSlash > lastDot)
        {
            return string.Empty;
        }
        if (lastBs > lastDot)
        {
            return string.Empty;
        }
        result = v.Substring(lastDot);

        if (!IsExtension(result))
        {
            if (a.filesWoExtReturnAsIs)
            {
                return result;
            }
            return string.Empty;
        }

        if (!a.returnOriginalCase)
        {
            result = result.ToLower();
        }



        return result;
    }

    public static bool IsExtension(string result)
    {
        if (string.IsNullOrWhiteSpace(result))
        {
            return false;
        }
        if (!result.TrimStart('.').ToLower().All(c => char.IsLetter(c) && char.IsLower(c) || char.IsDigit(c)))
        {
            return false;
        }
        return true;
    }

    //public static StorageFile CiStorageFile<StorageFolder, StorageFile>(string path, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        return (dynamic)path.ToString();
    //    }
    //    return ac.fs.ciStorageFile.Invoke(path);
    //}

    public static bool ExistsFile(string p)
    {
        return File.Exists(p);
    }
}
