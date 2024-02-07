namespace SunamoFileSystem;

public class FSND
{
    public static void CreateUpfoldersPsysicallyUnlessThere(string nad)
    {
        CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(nad));
    }

    #region GetDirectoryName
    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="rp"></param>
    /// <returns></returns>
    public static string GetDirectoryName(string rp)
    {
        // Zde zároveň vyhazuji výjimky
        var deli = DetectPathDelimiterChar(rp);



        if (string.IsNullOrEmpty(rp))
        {
            ThrowEx.IsNullOrEmpty("rp", rp);
        }

        if (!IsWindowsPathFormat(rp))
        {
            ThrowEx.IsNotWindowsPathFormat("rp", rp);
        }

        rp = rp.TrimEnd(deli);
        int dex = rp.LastIndexOf(deli);
        if (dex != -1)
        {
            string result = rp.Substring(0, dex + 1);
            FirstCharUpper(ref result);
            return result;
        }



        return "";
    }

    public static Tuple<bool, bool> DetectPathDelimiter(string rp)
    {
        var containsFs = rp.Contains("/");
        var containsBs = rp.Contains("\\");

        if (containsBs && containsFs)
        {
            throw new Exception("Path contains both fs & bs");
        }

        return Tuple.Create(containsFs, containsBs);
    }

    public static char DetectPathDelimiterChar(string rp)
    {
        var t = DetectPathDelimiter(rp);

        var containsFs = t.Item1;
        var containsBs = t.Item2;

        char deli = 'a';

        if (containsBs)
        {
            deli = AllCharsSE.bs;
        }
        else if (containsFs)
        {
            deli = AllCharsSE.slash;
        }
        else
        {
            throw new Exception("Path contains no delimiter");
        }

        return deli;
    }

    /// <summary>
    ///     Usage: Exceptions.IsNotWindowsPathFormat
    /// </summary>
    /// <param name="argValue"></param>
    /// <returns></returns>
    public static bool IsWindowsPathFormat(string argValue)
    {
        if (string.IsNullOrWhiteSpace(argValue))
        {
            return false;
        }

        bool badFormat = false;

        if (argValue.Length < 3)
        {
            return badFormat;
        }

        if (!char.IsLetter(argValue[0]))
        {
            badFormat = true;
        }

        if (char.IsLetter(argValue[1]))
        {
            badFormat = true;
        }

        if (argValue.Length > 2)
        {
            if (argValue[1] != '\\' && argValue[2] != '\\')
            {
                badFormat = true;
            }
        }

        return !badFormat;
    }
    #endregion

    public static bool ExistsDirectory(string p)
    {
        return Directory.Exists(p);
    }

    public static string MakeUncLongPath(ref string path)
    {
        if (!path.StartsWith(Consts.UncLongPath))
        {
            // V ASP.net mi vrátilo u každé directory.exists false. Byl jsem pod ApplicationPoolIdentity v IIS a bylo nastaveno Full Control pro IIS AppPool\DefaultAppPool
#if !ASPNET
            //  asp.net / vps nefunguje, ve windows store apps taktéž, NECHAT TO TRVALE ZAKOMENTOVANÉ
            // v asp.net toto způsobí akorát zacyklení, IIS začne vyhazovat 0xc00000fd, pak už nejde načíst jediná stránka
            //path = Consts.UncLongPath + path;
#endif
        }

        return path;
    }

    /// <summary>
    ///     Create all upfolders of A1 with, if they dont exist
    /// </summary>
    /// <param name="nad"></param>
    public static void CreateFoldersPsysicallyUnlessThere(string nad)
    {
        ThrowEx.IsNullOrEmpty("nad", nad);
        ThrowEx.IsNotWindowsPathFormat("nad", nad);


        if (Directory.Exists(nad))
        {
            return;
        }

        List<string> slozkyKVytvoreni = new List<string>
{
nad
};

        while (true)
        {
            nad = Path.GetDirectoryName(nad);

            // TODO: Tady to nefunguje pro UWP/UAP apps protoze nemaji pristup k celemu disku. Zjistit co to je UWP/UAP/... a jak v nem ziskat/overit jakoukoliv slozku na disku
            if (Directory.Exists(nad))
            {
                break;
            }

            string kopia = nad;
            slozkyKVytvoreni.Add(kopia);
        }

        slozkyKVytvoreni.Reverse();
        foreach (string item in slozkyKVytvoreni)
        {
            string folder = item;
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
    /// <summary>
    /// All occurences Path's method in sunamo replaced
    /// </summary>
    /// <param name="v"></param>
    public static void CreateDirectory(string v)
    {
        try
        {
            Directory.CreateDirectory(v);
        }
        catch (NotSupportedException)
        {


        }
    }

    /// <summary>
    ///     Usage: ExistsDirectoryWorker
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string MakeUncLongPath(string path)
    {
        return MakeUncLongPath(ref path);
    }

    public static void CreateDirectoryIfNotExists(string p)
    {
        MakeUncLongPath(ref p);

        if (!ExistsDirectory(p))
        {
            Directory.CreateDirectory(p);
        }
    }

    public static string WithEndSlash(string v)
    {
        return WithEndSlash(ref v);
    }

    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static string WithEndSlash(ref string v)
    {
        if (v != string.Empty)
        {
            v = v.TrimEnd(AllCharsSE.bs) + AllCharsSE.bs;
        }

        FirstCharUpper(ref v);
        return v;
    }

    public static List<string> FoldersWithSubfolder(string solutionFolder, string folderName)
    {
        var subFolders = Directory.GetDirectories(solutionFolder, "*", SearchOption.AllDirectories);
        List<string> result = new List<string>();

        foreach (var item in subFolders)
        {
            /*
Zde mám chybu:
System.IO.DirectoryNotFoundException: 'Could not find a part of the path
            'E:\vs\Projects\sunamoWithoutLocalDep.net\Clients\node_modules\napi-wasm'.'

            to musí být nějaká <|>, protože zde se mi to má dostat jen při sunamo nebo swod
            nikoliv při sunamo.net
            */

            var subf = Directory.GetDirectories(item, folderName, SearchOption.TopDirectoryOnly).ToList();
            if (subf.Count == 1)
            {
                result.Add(item);
            }
        }

        return result;
    }

    public static void FirstCharUpper(ref string nazevPP)
    {
        nazevPP = FirstCharUpper(nazevPP);
    }

    public static string FirstCharUpper(string nazevPP)
    {
        if (nazevPP.Length == 1)
        {
            return nazevPP.ToUpper();
        }

        string sb = nazevPP.Substring(1);
        return nazevPP[0].ToString().ToUpper() + sb;
    }
}
