namespace SunamoFileSystem;


public partial class FS : FSSH
{


    #region GetFilesMoreMasc - in thread
    //public static List<string> GetFilesMoreMasc(string path, string masc, SearchOption searchOption, GetFilesMoreMascArgs e = null)
    //{
    //    if (e == null)
    //    {
    //        e = new GetFilesMoreMascArgs();
    //    }

    //    e.path = path;
    //    e.masc = masc;
    //    e.searchOption = searchOption;

    //    return GetFilesMoreMasc(e);
    //}

    //public static List<string> GetFilesMoreMasc(GetFilesMoreMascArgs e = null)
    //{
    //    Thread t = new Thread(new ParameterizedThreadStart(GetFilesMoreMascWorker));


    //    t.Start();

    //}

    //private static void GetFilesMoreMascWorker(object o)
    //{
    //var e = (GetFilesMoreMascArgs)o;
    #endregion



    public static string FilesWithSameName(string vs, string v, SearchOption allDirectories)
    {
        FS.WithEndSlash(ref vs);

        Dictionary<string, List<string>> f = new Dictionary<string, List<string>>();
        var s = FS.GetFiles(vs, v, allDirectories);
        foreach (var item in s)
        {
            DictionaryHelper.AddOrCreate<string, string>(f, FS.GetFileName(item), item);
        }

        TextOutputGenerator tog = new TextOutputGenerator();
        foreach (var item in f)
        {
            if (item.Value.Count > 1)
            {
                tog.List(item.Value);
            }
        }

        return tog.ToString();
    }

    /// <summary>
    ///
    /// When is occur Access denied exception, use GetFilesEveryFolder, which find files in every folder
    /// A1 have to be with ending backslash
    /// A4 must have underscore otherwise is suggested while I try type true
    /// A2 can be delimited by semicolon. In case more extension use FS.GetFilesOfExtensions
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    /// <param name="searchOption"></param>
    public async static Task<List<string>> GetFilesAsync(string folder2, string mask, SearchOption searchOption, GetFilesArgs getFilesArgs = null)
    {
        if (!FS.ExistsDirectory(folder2) && !folder2.Contains(";"))
        {
            ThisApp.Warning(folder2 + "does not exists");
            return new List<string>();
        }

        if (getFilesArgs == null)
        {
            getFilesArgs = new GetFilesArgs();
        }

        var folders = SHSplit.Split(folder2, AllStrings.sc);
        CA.PostfixIfNotEnding(AllStrings.bs, folders);

        List<string> list = new List<string>();
        foreach (var folder in folders)
        {
            if (!FS.ExistsDirectory(folder))
            {

            }
            else
            {
                return FS.GetFilesMoreMasc(folder, mask, searchOption);
            }
        }

        CAChangeContent.ChangeContent0(null, list, d => SH.FirstCharUpper(d));

        if (getFilesArgs._trimA1AndLeadingBs)
        {
            foreach (var folder in folders)
            {
                list = CAChangeContent.ChangeContent0(null, list, d => d = d.Replace(folder, ""));
            }

        }

        if (getFilesArgs._trimA1AndLeadingBs)
        {
            foreach (var folder in folders)
            {
                list = CAChangeContent.ChangeContent0(null, list, d => d = SHParts.RemoveAfterLast(d, AllChars.dot));
            }

        }

        if (getFilesArgs.excludeFromLocationsCOntains != null)
        {
            // I want to find files recursively
            foreach (var item in getFilesArgs.excludeFromLocationsCOntains)
            {
                CA.RemoveWhichContains(list, item, false);
            }
        }

        Dictionary<string, DateTime> dictLastModified = null;

        bool isLastModifiedFromFn = getFilesArgs.LastModifiedFromFn != null;

        if (getFilesArgs.dontIncludeNewest || (getFilesArgs.byDateOfLastModifiedAsc || isLastModifiedFromFn))
        {
            dictLastModified = new Dictionary<string, DateTime>();
            foreach (var item in list)
            {
                DateTime? dt = null;

                if (isLastModifiedFromFn)
                {
                    dt = getFilesArgs.LastModifiedFromFn(Path.GetFileNameWithoutExtension(item));
                }

                if (!dt.HasValue)
                {
                    dt = FS.LastModified(item);
                }

                dictLastModified.Add(item, dt.Value);
            }
            list = dictLastModified.OrderBy(t => t.Value).Select(r => r.Key).ToList();
        }

        if (getFilesArgs.dontIncludeNewest)
        {

            list.RemoveAt(list.Count - 1);
        }



        if (getFilesArgs.excludeWithMethod != null)
        {
            getFilesArgs.excludeWithMethod.Invoke(list);
        }

        return list;
    }

    public static bool IsFileOlderThanXHours(string path, int hours, bool mustFileExists = false)
    {
        var exf = File.Exists(path);

        if (mustFileExists)
        {
            if (!exf)
            {
                ThrowEx.FileDoesntExists(path);
            }
        }
        else
        {
            if (!exf)
            {
                return true;
            }
        }

        var lm = FS.LastModified(path);
        if (lm > DateTime.Now.AddHours(hours * -1))
        {
            return false;
        }
        return true;
    }

    public static List<string> GetFileNamesWoExtension(List<string> jpgcka)
    {
        var dd = new List<string>(jpgcka.Count);
        for (int i = 0; i < jpgcka.Count; i++)
        {
            dd.Add(Path.GetFileNameWithoutExtension(jpgcka[i]));
        }

        return dd;
    }





    /// <summary>
    /// path + file
    /// </summary>
    public static string GetTempFilePath()
    {
        return Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName());
    }



    /// <summary>
    /// Copy file A1 into A2
    /// </summary>
    /// <param name="v"></param>
    /// <param name="nad"></param>
    public static void CopyTo(string v, string nad, FileMoveCollisionOption o)
    {
        var fileTo = Path.Combine(nad, FS.GetFileName(v));
        CopyFile(v, fileTo, o);
    }








    public static StorageFolder GetDirectoryNameFolder<StorageFolder, StorageFile>(StorageFolder rp2, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            return ac.fs.getDirectoryNameFolder.Invoke(rp2);
        }
        //ThrowEx.Custom("GetDirectoryName");
        var rp = rp2.ToString();
        return (dynamic)GetDirectoryName(rp);
    }


    public static void CreateFoldersPsysicallyUnlessThere<StorageFolder, StorageFile>(StorageFile nad, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac == null)
        {
            CreateFoldersPsysicallyUnlessThere(nad.ToString());
        }
        else
        {
            ThrowNotImplementedUwp();
        }
    }
    /// <summary>
    /// change all first (drive) letter to uppercase
    /// </summary>
    /// <param name="p"></param>
    /// <param name="folderWithProjectsFolders"></param>
    /// <param name="folderWithTemporaryMovedContentWithoutBackslash"></param>
    public static string ReplaceDirectoryThrowExceptionIfFromDoesntExists(string p, string folderWithProjectsFolders, string folderWithTemporaryMovedContentWithoutBackslash)
    {
        p = SH.FirstCharUpper(p);
        folderWithProjectsFolders = SH.FirstCharUpper(folderWithProjectsFolders);
        folderWithTemporaryMovedContentWithoutBackslash = SH.FirstCharUpper(folderWithTemporaryMovedContentWithoutBackslash);

        if (!ThrowEx.NotContains(p, folderWithProjectsFolders))
        {
            // Here can never accomplish when exc was throwed
            return p;
        }

        // Here can never accomplish when exc was throwed
        return p.Replace(folderWithProjectsFolders, folderWithTemporaryMovedContentWithoutBackslash);
    }

    /// <summary>
    /// Direct edit
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static List<string> OnlyNamesWithoutExtension(List<string> p)
    {
        for (int i = 0; i < p.Count; i++)
        {
            p[i] = Path.GetFileNameWithoutExtension(p[i]);
        }
        return p;
    }


    /// <summary>
    /// Vrátí cestu a název souboru s ext a ext
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    public static void GetPathAndFileName(string fn, out string path, out string file, out string ext)
    {
        path = FS.WithEndSlash(FS.GetDirectoryName(fn));
        file = Path.GetFileNameWithoutExtension(fn);
        ext = FS.GetExtension(fn);
    }



    /// <summary>
    /// Not working - see unit tests
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static string GetAbsolutePath2(string relativePath, string dir)
    {
        var ap = GetAbsolutePath(dir, relativePath);
        return Path.GetFullPath(ap);

    }

    /// <summary>
    /// Working - see unit tests
    /// if A1 not ending with \, GetDirectoryName
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string GetAbsolutePath(string dir, string relativePath)
    {
        FileToDirectory(ref dir);

        var ds = AllStrings.ds;
        var dds = AllStrings.dds;

        var dds2 = 0;
        while (true)
        {
            if (relativePath.StartsWith(ds))
            {
                relativePath = relativePath.Substring(ds.Length);
            }
            else if (relativePath.StartsWith(dds))
            {
                dds2++;
                relativePath = relativePath.Substring(dds.Length);
            }
            else
            {
                break;
            }
        }

        var tokens = FS.GetTokens(relativePath);
        tokens = tokens.Skip(dds2).ToList();
        tokens.Insert(0, dir);

        var vr = Combine(tokens.ToArray());
        vr = GetFullPath(vr);
        return vr;
    }





    public static List<string> GetTokens(string relativePath)
    {
        var deli = "";
        if (relativePath.Contains(AllStrings.bs))
        {
            deli = AllStrings.bs;
        }
        else if (relativePath.Contains(AllStrings.slash))
        {
            deli = AllStrings.slash;
        }

        return SHSplit.Split(relativePath, deli);
    }



    public static void CopyStream(Stream input, Stream output)
    {
        byte[] buffer = new byte[8 * 1024];
        int len;
        while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, len);
        }
    }

    /// <summary>
    /// Remove path to project folder as are in DefaultPaths.AllPathsToProjects
    /// A2 is here to remove also solution
    /// </summary>
    /// <param name="fullPathOriginalFile"></param>
    /// <param name="combineWithA1"></param>
    /// <param name="empty"></param>
    public static string ReplaceVsProjectFolder(string fullPathOriginalFile, string combineWithA1, string empty)
    {
        DefaultPaths.InitAllPathsToProjects();

        fullPathOriginalFile = SH.FirstCharUpper(fullPathOriginalFile);
        foreach (var item in DefaultPaths.AllPathsToProjects)
        {
            string replace = FS.WithEndSlash(Path.Combine(item, combineWithA1));
            if (fullPathOriginalFile.StartsWith(replace))
            {
                return fullPathOriginalFile.Replace(replace, empty);
            }
        }
        return fullPathOriginalFile;
    }

    /// <summary>
    /// Cant return with end slash becuase is working also with files
    /// </summary>
    /// <param name="s"></param>
    public static string CombineWithoutFirstCharUpper(params string[] s)
    {
        return CombineWorker(false, true, s);
    }









    public static void SaveMemoryStream(System.IO.MemoryStream mss, string path)
    {
        SaveMemoryStream<string, string>(mss, path, null);
    }

    public static void SaveMemoryStream<StorageFolder, StorageFile>(System.IO.MemoryStream mss, StorageFile path, AbstractCatalog<StorageFolder, StorageFile> ac)
    {

        if (!FS.ExistsFileAc(path, ac))
        {
            if (ac == null)
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(path.ToString(), System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    byte[] matriz = mss.ToArray();
                    fs.Write(matriz, 0, matriz.Length);
                }
            }
            else
            {
                ThrowEx.Custom("SaveMemoryStream");
            }
        }
    }





    public static StorageFolder CiStorageFolder<StorageFolder, StorageFile>(string path, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac == null)
        {
            var ps = path.ToString();
            ps = FS.WithEndSlash(ps);
            return (dynamic)ps;
        }
        return ac.fs.ciStorageFolder.Invoke(path);
    }


    public static string DeleteWrongCharsInDirectoryName(string p)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char item in p)
        {
            if (!s_invalidPathChars.Contains(item))
            {
                sb.Append(item);
            }
        }
        var result = sb.ToString();
        FS.FirstCharUpper(ref result);
        return result;
    }

    public static string DeleteWrongCharsInFileName(string p, bool isPath)
    {
        List<char> invalidFileNameChars2 = null;

        if (isPath)
        {
            invalidFileNameChars2 = s_invalidFileNameCharsWithoutDelimiterOfFolders;
        }
        else
        {
            invalidFileNameChars2 = s_invalidFileNameChars;
        }

        StringBuilder sb = new StringBuilder();
        foreach (char item in p)
        {
            if (!invalidFileNameChars2.Contains(item))
            {
                sb.Append(item);
            }
        }

        var result = sb.ToString();
        FS.FirstCharUpper(ref result);
        return result;
    }

    public static bool ContainsInvalidPathCharForPartOfMapPath(string p)
    {
        foreach (var item in s_invalidCharsForMapPath)
        {
            if (p.IndexOf(item) != -1)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Odstraňuje samozřejmě ve výjimce
    /// </summary>
    /// <param name="path"></param>
    public static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// No direct edit
    /// </summary>
    /// <param name="files2"></param>
    /// <returns></returns>
    public static List<string> OnlyNamesNoDirectEdit(String[] files2)
    {
        var tl = CA.ToListMoreString(files2);
        return OnlyNamesNoDirectEdit(tl);
    }

    /// <summary>
    /// No direct edit
    /// Returns with extension
    /// POZOR: Na rozdíl od stejné metody v sunamo tato metoda vrací úplně nové pole a nemodifikuje A1
    /// </summary>
    /// <param name="files"></param>
    public static List<string> OnlyNamesNoDirectEdit(List<string> files2)
    {
        List<string> files = new List<string>(files2.Count);
        for (int i = 0; i < files2.Count; i++)
        {
            files.Add(FS.GetFileName(files2[i]));
        }
        return files;
    }

    /// <summary>
    /// No direct edit
    /// </summary>
    /// <param name="appendToStart"></param>
    /// <param name="fullPaths"></param>
    /// <returns></returns>
    public static List<string> OnlyNamesNoDirectEdit(string appendToStart, List<string> fullPaths)
    {
        List<string> ds = new List<string>(fullPaths.Count);
        CASE.InitFillWith(ds, fullPaths.Count);
        for (int i = 0; i < fullPaths.Count; i++)
        {
            ds[i] = appendToStart + FS.GetFileName(fullPaths[i]);
        }
        return ds;
    }

    public static List<string> GetFolders(string folder)
    {
        return GetFolders(folder, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    /// Return only subfolder if A3, a1 not include
    /// Must have backslash on end - is folder
    ///
    ///
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="masc"></param>
    /// <param name="so"></param>
    /// <param name="_trimA1"></param>
    public static List<string> GetFolders(string folder, string masc, SearchOption so, bool _trimA1AndLeadingBs = false)
    {
        List<string> dirs = null;
        try
        {
            dirs = Directory.GetDirectories(folder, masc, so).ToList();
        }
        catch (Exception ex)
        {
            ThrowEx.CustomWithStackTrace(ex);
        }

        if (dirs == null)
        {
            return new List<string>();
        }


        CAChangeContent.ChangeContent0(null, dirs, d => SH.FirstCharUpper(d));


        if (_trimA1AndLeadingBs)
        {
            CA.Replace(dirs, folder, string.Empty);
            CA.TrimEnd(dirs, new Char[] { AllChars.bs });
        }
        else
        {
            // Must have backslash on end - is folder
            if (CA.PostfixIfNotEnding != null)
            {
                CA.PostfixIfNotEnding(@"\", dirs);
            }
        }

        return dirs;
    }
    public static List<string> GetFolders(string folder, SearchOption so)
    {
        return GetFolders(folder, AllStrings.asterisk, so);
    }
    public static List<string> GetFolders(string v, string contains)
    {
        var folders = GetFolders(v);
        CA.TrimEnd(folders, new char[] { AllChars.bs });
        for (int i = folders.Count - 1; i >= 0; i--)
        {
            if (!SunamoRegex.Wildcard.IsMatch(FS.GetFileName(folders[i]), contains))
            {
                folders.RemoveAt(i);
            }
        }

        return folders;
    }

    /// <summary>
    /// A2 is path of target file
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fileTo"></param>
    /// <param name="co"></param>
    public static void CopyFile<StorageFolder, StorageFile>(string item, string fileTo2, FileMoveCollisionOption co, AbstractCatalog<StorageFolder, StorageFile> ac = null)
    {
        if (ac == null)
        {
            CopyFile(item, fileTo2, co);
        }
        else
        {
            ThrowNotImplementedUwp();
        }
    }



    /// <summary>
    /// A1,2 isnt  working like ref
    /// </summary>
    /// <typeparam name="StorageFolder"></typeparam>
    /// <typeparam name="StorageFile"></typeparam>
    /// <param name="item"></param>
    /// <param name="fileTo"></param>
    /// <param name="co"></param>
    /// <param name="ac"></param>
    public static bool CopyMoveFilePrepare<StorageFolder, StorageFile>(ref StorageFile item, ref StorageFile fileTo, FileMoveCollisionOption co, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac == null)
        {
            var item2 = item.ToString();
            var fileTo2 = fileTo.ToString();
            return CopyMoveFilePrepare(ref item2, ref fileTo2, co);
        }

        ThrowNotImplementedUwp();
        MakeUncLongPath(ref item, ac);
        MakeUncLongPath<StorageFolder, StorageFile>(ref fileTo, ac);
        FS.CreateUpfoldersPsysicallyUnlessThereAc<StorageFolder, StorageFile>(fileTo, ac);
        if (FS.ExistsFileAc<StorageFolder, StorageFile>(fileTo, ac))
        {
        }
        return false;
    }

    public static bool CopyMoveFilePrepare<StorageFolder, StorageFile>(ref string item, ref StorageFile fileTo2, FileMoveCollisionOption co, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac == null)
        {
            var fileTo = fileTo2.ToString();
            return CopyMoveFilePrepare(ref item, ref fileTo, co);
        }

        ThrowNotImplementedUwp();
        return false;
    }

    public static string ChangeExtension(string item, string newExt, bool physically)
    {
        if (UH.HasHttpProtocol(item))
        {
            return UH.ChangeExtension(item, FS.GetExtension(item, new GetExtensionArgs { returnOriginalCase = true }), newExt);
        }

        string cesta = FS.GetDirectoryName(item);
        string fnwoe = Path.GetFileNameWithoutExtension(item);
        string nova = Path.Combine(cesta, fnwoe + newExt);

        if (physically)
        {
            try
            {
                if (File.Exists(nova))
                {
                    File.Delete(nova);
                }
                File.Move(item, nova);
            }
            catch
            {
            }
        }
        FirstCharUpper(ref nova);
        return nova;
    }



    public static string CreateDirectory(string v, DirectoryCreateCollisionOption whenExists, SerieStyle serieStyle, bool reallyCreate)
    {
        if (FS.ExistsDirectory(v))
        {
            bool hasSerie;
            string nameWithoutSerie = FS.GetNameWithoutSeries(v, false, out hasSerie, serieStyle);
            if (hasSerie)
            {
            }

            if (whenExists == DirectoryCreateCollisionOption.AddSerie)
            {
                int serie = 1;
                while (true)
                {
                    string newFn = nameWithoutSerie + " (" + serie + AllStrings.rb;
                    if (!FS.ExistsDirectory(newFn))
                    {
                        v = newFn;
                        break;
                    }
                    serie++;
                }
            }
            else if (whenExists == DirectoryCreateCollisionOption.Delete)
            {
            }
            else if (whenExists == DirectoryCreateCollisionOption.Overwrite)
            {
            }
            else
            {
                ThrowEx.NotImplementedCase(whenExists);
            }
        }
        if (reallyCreate)
        {
            Directory.CreateDirectory(v);
        }

        return v;
    }





    public static List<string> GetFilesWithoutArgs(string folderPath, string masc, bool? rec)
    {
        return GetFiles(folderPath, masc, rec, null);
    }

    public static List<string> GetFiles(string folderPath, string masc, bool? rec, GetFilesArgs a = null)
    {
        SearchOption so = SearchOption.TopDirectoryOnly;

        var b = BTS.GetValueOfNullable(rec);
        if (b)
        {
            so = SearchOption.AllDirectories;
        }

        return
        GetFiles(folderPath, masc, so, a);
    }

    public static List<string> GetFiles(string folderPath, string masc)
    {
        return FS.GetFiles(folderPath, masc, SearchOption.TopDirectoryOnly);
    }




    /// <summary>
    /// It's always recursive
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    public static List<string> GetFoldersEveryFolder(string folder, string mask, GetFilesArgs e = null)
    {
        if (e == null)
        {
            e = new GetFilesArgs();
        }

        List<string> list = new List<string>();
        // zde progress bar nedává smysl. načítám to rekurzivně, tedy nevím na začátku kolik těch složek bude
        //IProgressBarHelper pbh = null;

        //if (a.progressBarHelper != null)
        //{
        //    pbh = a.progressBarHelper.CreateInstance(a.pb, files.Count, this);
        //}

        GetFoldersEveryFolder(folder, list, e);

        if (e._trimA1AndLeadingBs)
        {
            list = CAChangeContent.ChangeContent0(null, list, d => d = d.Replace(folder, "").TrimStart(AllChars.bs));
        }
        if (e.excludeFromLocationsCOntains != null)
        {
            // I want to find files recursively
            foreach (var item in e.excludeFromLocationsCOntains)
            {
                CA.RemoveWhichContains(list, item, e.wildcard);
            }
        }

        return list;
    }

    /// <summary>
    /// A3 must be GetFilesArgs, not GetFoldersEveryFolder because is calling from GetFiles
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="list"></param>
    /// <param name="e"></param>
    private static void GetFoldersEveryFolder(string folder, List<string> list, GetFilesArgs e = null)
    {
        List<string> folders = null;

#if DEBUG
        if (folder == @"E:\vs\Projects\AllProjectsSearch\Aps.Projs\")
        {

        }
#endif

        try
        {
            folders = Directory.GetDirectories(folder).ToList();

            folders = CAChangeContent.ChangeContent0(null, folders, FS.WithEndSlash);
            //#if DEBUG
            //            if (e.writeToDebugEveryLoadedFolder)
            //            {
            //                DebugLogger.Instance.WriteLine("GetFoldersEveryFolder: " + folder);
            //            }
            //#endif
        }
        catch (Exception ex)
        {
            ThrowEx.DummyNotThrow(ex);
            // Not throw exception, it's probably Access denied  on Documents and Settings etc
            //ThrowEx.Custom("GetFoldersEveryFolder with path: " + folder, ex);
        }

        if (folders != null)
        {
            CA.RemoveWhichContainsList(folders, e.excludeFromLocationsCOntains, e.wildcard);

            list.AddRange(folders);

            for (int i = 0; i < folders.Count; i++)
            {
                GetFoldersEveryFolder(folders[i], list, e);
            }

            //foreach (var item in folders)
            //{

            //}
        }
    }

    private static void GetFoldersEveryFolder(string folder, string mask, List<string> list)
    {
        try
        {
            var folders = Directory.GetDirectories(folder, mask, SearchOption.TopDirectoryOnly);
            list.AddRange(folders);

            foreach (var item in folders)
            {
                GetFoldersEveryFolder(item, mask, list);
            }
        }
        catch (Exception ex)
        {
            ThrowEx.DummyNotThrow(ex);
            // Not throw exception, it's probably Access denied  on Documents and Settings etc
            //ThrowEx.Custom("GetFoldersEveryFolder with path: " + folder, ex);
        }
    }

    //public static List<string> GetFilesEveryFolder(string folder, string mask, SearchOption searchOption, bool _trimA1 = false)
    //{


    //    var d = Task.Run<List<string>>(async () => await GetFilesEveryFolderAsync(folder, mask, searchOption, new GetFilesEveryFolderArgs {_trimA1 =  _trimA1 })).Result;
    //    return d;
    //}

    /// <summary>
    /// In item1 is all directories, in Item2 all files
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="ask"></param>
    /// <param name="searchOption"></param>
    /// <param name="_trimA1"></param>
    public static List<string> GetFilesEveryFolder(string folder, string mask, SearchOption searchOption, GetFilesEveryFolderArgs e = null)
    {
#if DEBUG
        if (folder == @"D:\_Test\EveryLine\EveryLine\SearchCodeElementsUC\")
        {

        }
#endif

        if (e == null)
        {
            e = new GetFilesEveryFolderArgs();
        }

        // TODO: některé soubory vrací vícekrát. toto je workaround než zjistím proč
        // TODO: je důležité se toho zbavit co nejdříve protože při načítání to zbytečně zpomaluje
        List<string> list = new List<string>();
        List<string> dirs = null;

        bool measureTime = false;

        if (measureTime)
        {
            StopwatchStatic.Start();
        }

        // There is not exc handle needed, its slowly then
        //try
        //{
        if (e.usePbTime)
        {
            var m = sess.i18n(XlfKeys.Loading) + AllStrings.space + sess.i18n(XlfKeys.FoldersTree) + Consts.dots3;

            e.InsertPbTime(60);
            e.UpdateTbPb(m);
        }

        dirs = GetFoldersEveryFolder(folder, "*", new GetFoldersEveryFolderArgs(e)).ToList();


#if DEBUG
        //int before = dirs.Count;
#endif

        if (e.FilterFoundedFolders != null)
        {
            string si = null;

            for (int i = dirs.Count - 1; i >= 0; i--)
            {
                si = dirs[i];
                //if (si.Contains(@"\standard\.git"))
                //{

                //}

                if (!e.FilterFoundedFolders.Invoke(si))
                {
                    dirs.RemoveAt(i);
                }
            }
        }



#if DEBUG
        //int after = dirs.Count;
#endif

        #region MyRegion
        //ClipboardHelper.SetLines(dirs);

        //}
        //catch (Exception ex)
        //{
        //    ThrowEx.Custom(sess.i18n(XlfKeys.GetFilesWithPath)+": " + folder);
        //}
        #endregion

        if (measureTime)
        {
            StopwatchStatic.StopAndPrintElapsed("GetFoldersEveryFolder");
        }

        if (measureTime)
        {
            StopwatchStatic.Start();
        }

        if (e.usePb)
        {
            var m = sess.i18n(XlfKeys.Loading) + AllStrings.space + sess.i18n(XlfKeys.FilesTree) + Consts.dots3;

            e.InsertPb(dirs.Count);
            e.UpdateTbPb(m);
        }

        List<string> d = new List<string>();

        //Není třeba, již volám dole e.Done(); / e.DonePartially();
        //IProgressBarHelper pbh = null;

        //if (e.progressBarHelper != null)
        //{
        //    pbh = e.progressBarHelper.CreateInstance(e.pb, dirs.Count, e.pb);
        //}

        dirs.Insert(0, folder);
        foreach (var item in dirs)
        {
            try
            {
#if ASYNC
                TF.WaitD();
#endif

                //d.Clear();
                var f = FS.GetFiles(item, mask, SearchOption.TopDirectoryOnly);


                d.AddRange(f);

                if (e.getNullIfThereIsMoreThanXFiles != -1)
                {
                    if (d.Count > e.getNullIfThereIsMoreThanXFiles)
                    {
                        if (e.usePb)
                        {
                            e.Done();
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowEx.DummyNotThrow(ex);
                // Not throw exception, it's probably Access denied on Documents and Settings etc
                //ThrowEx.FileSystemException( ex);
            }

            if (e.usePb)
            {
                e.DoneOnePercent();
            }
#if DEBUG
            //before = d.Count;
#endif

            if (e.FilterFoundedFiles != null)
            {
                for (int i = d.Count - 1; i >= 0; i--)
                {
                    if (!e.FilterFoundedFiles(d[i]))
                    {
                        d.RemoveAt(i);
                    }
                }
            }

#if DEBUG
            //after = d.Count;

            //if (before != 0 && after == 0)
            //{

            //}
#endif

            list.AddRange(d);
            d.Clear();
        }

        list = list.Distinct().ToList();

        if (e.usePb)
        {
            e.Done();
        }

        if (measureTime)
        {
            StopwatchStatic.StopAndPrintElapsed("GetFiles");
        }

        CAChangeContent.ChangeContent0(null, list, d2 => SH.FirstCharUpper(d2));

        if (e._trimA1AndLeadingBs)
        {
            list = CAChangeContent.ChangeContent0(null, list, d3 => d3 = d3.Replace(folder, "").TrimStart(AllChars.bs));
        }
        return list;
    }







    public static byte[] StreamToArrayBytes(System.IO.Stream stream)
    {
        if (stream == null)
        {
            return new byte[0];
        }

        long originalPosition = 0;

        if (stream.CanSeek)
        {
            originalPosition = stream.Position;
            stream.Position = 0;
        }

        try
        {
            byte[] readBuffer = new byte[4096];

            int totalBytesRead = 0;
            int bytesRead;

            while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;

                if (totalBytesRead == readBuffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte != -1)
                    {
                        byte[] temp = new byte[readBuffer.Length * 2];
                        Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                        Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                        readBuffer = temp;
                        totalBytesRead++;
                    }
                }
            }

            byte[] buffer = readBuffer;
            if (readBuffer.Length != totalBytesRead)
            {
                buffer = new byte[totalBytesRead];
                Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
            }
            return buffer;
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = originalPosition;
            }
        }
    }


    public static string AddExtensionIfDontHave(string file, string ext)
    {
        // For *.* and git paths {dir}/*
        if (file[file.Length - 1] == AllChars.asterisk)
        {
            return file;
        }
        if (GetExtension(file) == string.Empty)
        {
            return file + ext;
        }

        return file;
    }

    /// <summary>
    /// Vratí bez cesty, pouze název souboru
    /// Earlier name InsertBetweenFileNameAndExtension2
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="whatInsert"></param>
    public static string InsertBetweenFileNameAndExtensionRemovePath(string orig, string whatInsert)
    {
        string fn = Path.GetFileNameWithoutExtension(orig);
        string e = FS.GetExtension(orig);
        return Path.Combine(fn + whatInsert + e);
    }

    /// <summary>
    /// In key are just filename, in value full path to all files
    /// </summary>
    /// <param name="linesFiles"></param>
    /// <param name="searchOnlyWithExtension"></param>
    public static Dictionary<string, List<string>> GetDictionaryByFileNameWithExtension(List<string> files)
    {
        Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        foreach (var item in files)
        {
            string filename = FS.GetFileName(item);
            DictionaryHelper.AddOrCreateIfDontExists<string, string>(result, filename, item);
        }

        return result;
    }




    public static string ChangeFilename(string item, string newFileNameWithoutPath, bool physically)
    {
        string cesta = FS.GetDirectoryName(item);
        string nova = Path.Combine(cesta, newFileNameWithoutPath);

        if (physically)
        {
            try
            {
                if (File.Exists(nova))
                {
                    File.Delete(nova);
                }
                File.Move(item, nova);
            }
            catch
            {
            }
        }
        return nova;
    }


    /// <summary>
    /// Zmeni nazev souboru na A2
    /// Pro A3 je výchozí z minulosti true - jakoby s false se chovala metoda ReplaceFileName
    /// Pokud nechci nazev souboru uplne menit, ale pouze v nem neco nahradit, pouziva se metoda ReplaceInFileName
    /// </summary>
    /// <param name="item"></param>
    /// <param name="newFileName"></param>
    /// <param name="onDrive"></param>
    public static string ChangeFilename<StorageFolder, StorageFile>(StorageFile item, string newFileName, bool physically, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac == null)
        {
            ChangeFilename(item.ToString(), newFileName, physically);
        }
        ThrowNotImplementedUwp();
        return null;
    }

    /// <summary>
    /// A2 true - bs to slash. false - slash to bs
    /// </summary>
    /// <param name="path"></param>
    /// <param name="v"></param>
    public static string Slash(string path, bool slash)
    {
        string result = null;
        if (slash)
        {
            result = SHReplace.ReplaceAll2(path, AllStrings.slash, AllStrings.bs);
        }
        else
        {
            result = SHReplace.ReplaceAll2(path, AllStrings.bs, AllStrings.slash);
        }

        FS.FirstCharUpper(ref result);
        return result;
    }

    /// <summary>
    /// Pokusí se max. 10x smazat soubor A1, pokud se nepodaří, GF, jinak GT
    /// </summary>
    /// <param name="item"></param>
    public static bool TryDeleteWithRepetition(string item)
    {
        int pokusyOSmazani = 0;
        while (true)
        {
            try
            {
                File.Delete(item);
                return true;
            }
            catch
            {
                pokusyOSmazani++;
                if (pokusyOSmazani == 9)
                {
                    return false;
                }
            }
        }
    }

    public static bool TryDeleteFile(string item, out string message)
    {
        message = null;
        try
        {
            File.Delete(item);
            return true;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return false;
        }
    }

    private static string GetSizeInAutoString(double size)
    {


        ComputerSizeUnits unit = ComputerSizeUnits.B;
        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.KB;
            size /= NumConsts.kB;
        }
        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.MB;
            size /= NumConsts.kB;
        }
        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.GB;
            size /= NumConsts.kB;
        }
        if (size > NumConsts.kB)
        {
            unit = ComputerSizeUnits.TB;
            size /= NumConsts.kB;
        }

        return size + " " + unit.ToString();
    }
    public static string GetSizeInAutoString(long value, ComputerSizeUnits b)
    {
        return GetSizeInAutoString((double)value, b);
    }

    /// <summary>
    /// A1 is input unit, not output
    /// </summary>
    /// <param name="value"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static string GetSizeInAutoString(double value, ComputerSizeUnits b)
    {
        if (b != ComputerSizeUnits.B)
        {
            // Získám hodnotu v bytech
            value = ConvertToSmallerComputerUnitSize(value, b, ComputerSizeUnits.B);
        }


        if (value < 1024)
        {
            return value + " B";
        }

        double previous = value;
        value /= 1024;

        if (value < 1)
        {
            return previous + " B";
        }

        previous = value;
        value /= 1024;

        if (value < 1)
        {
            return previous + " KB";
        }

        previous = value;
        value /= 1024;
        if (value < 1)
        {
            return previous + " MB";
        }

        previous = value;
        value /= 1024;

        if (value < 1)
        {
            return previous + " GB";
        }

        return value + " TB";
    }

    private static long ConvertToSmallerComputerUnitSize(long value, ComputerSizeUnits b, ComputerSizeUnits to)
    {
        return ConvertToSmallerComputerUnitSize(value, b, to);
    }
    private static double ConvertToSmallerComputerUnitSize(double value, ComputerSizeUnits b, ComputerSizeUnits to)
    {
        if (to == ComputerSizeUnits.Auto)
        {
            ThrowEx.Custom("Byl specifikov\u00E1n v\u00FDstupn\u00ED ComputerSizeUnit, nem\u016F\u017Eu toto nastaven\u00ED zm\u011Bnit");
        }
        else if (to == ComputerSizeUnits.KB && b != ComputerSizeUnits.KB)
        {
            value *= 1024;
        }
        else if (to == ComputerSizeUnits.MB && b != ComputerSizeUnits.MB)
        {
            value *= 1024 * 1024;
        }
        else if (to == ComputerSizeUnits.GB && b != ComputerSizeUnits.GB)
        {
            value *= 1024 * 1024 * 1024;
        }
        else if (to == ComputerSizeUnits.TB && b != ComputerSizeUnits.TB)
        {
            value *= (1024L * 1024L * 1024L * 1024L);
        }

        return value;
    }



    /// <summary>
    /// txt files (*.txt)|*.txt|All files (*.*)|*.*"
    /// </summary>
    /// <param name="filter"></param>
    public static string RepairFilter(string filter)
    {
        if (!filter.Contains(AllStrings.verbar))
        {
            filter = filter.TrimStart(AllChars.asterisk);
            return AllStrings.asterisk + filter + AllStrings.verbar + AllStrings.asterisk + filter;
        }
        return filter;
    }





    /// <summary>
    /// Replacement can be configured with replaceIncorrectFor
    ///
    /// </summary>
    /// <param name="p"></param>
    public static string ReplaceIncorrectCharactersFile(string p)
    {
        string t = p;
        foreach (char item in invalidFileNameChars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char item2 in t)
            {
                if (item != item2)
                {
                    sb.Append(item2);
                }
                else
                {
                    sb.Append(AllStrings.space);
                }
            }
            t = sb.ToString();
        }
        return t;
    }
    /// <summary>
    /// ReplaceIncorrectCharactersFile - can specify char for replace with
    /// ReplaceInvalidFileNameChars - all wrong chars skip
    ///
    /// A2 - can specify more letter in one string
    /// A3 is applicable only for A2. In general is use replaceIncorrectFor
    /// </summary>
    /// <param name="p"></param>
    /// <param name="replaceAllOfThisByA3"></param>
    /// <param name="replaceForThis"></param>
    public static string ReplaceIncorrectCharactersFile(string p, string replaceAllOfThisByA3, string replaceForThis)
    {
        string t = p;
        foreach (char item in invalidFileNameChars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char item2 in t)
            {
                if (item != item2)
                {
                    sb.Append(item2);
                }
                else
                {
                    sb.Append(replaceForThis);
                }
            }
            t = sb.ToString();
        }
        if (!string.IsNullOrEmpty(replaceAllOfThisByA3))
        {
            foreach (char item in replaceAllOfThisByA3)
            {
                t = SHReplace.ReplaceAll(t, replaceForThis, item.ToString());
            }

        }
        return t;
    }
    /// <summary>
    /// Pro odstranění špatných znaků odstraní všechny výskyty A2 za mezery a udělá z více mezere jediné
    /// </summary>
    /// <param name="p"></param>
    /// <param name="replaceAllOfThisThen"></param>
    public static string ReplaceIncorrectCharactersFile(string p, string replaceAllOfThisThen)
    {
        string replaceFor = AllStrings.space;
        string t = p;
        foreach (char item in invalidFileNameChars)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char item2 in t)
            {
                if (item != item2)
                {
                    sb.Append(item2);
                }
                else
                {
                    sb.Append(replaceFor);
                }
            }
            t = sb.ToString();
        }
        if (!string.IsNullOrEmpty(replaceAllOfThisThen))
        {
            t = SHReplace.ReplaceAll(t, replaceFor, replaceAllOfThisThen);
            t = SHReplace.ReplaceAll(t, replaceFor, AllStrings.doubleSpace);
        }
        return t;
    }

    /// <summary>
    /// either A1 or A2 can be null
    /// When A2 is null, will get from file path A1
    /// </summary>
    /// <param name="item"></param>
    /// <param name="folder"></param>
    /// <param name="insert"></param>
    public static string InsertBetweenFileNameAndPath(string item, string folder, string insert)
    {
        if (folder == null)
        {
            folder = FS.GetDirectoryName(item);
        }
        var outputFolder = Path.Combine(folder, insert);
        FS.CreateFoldersPsysicallyUnlessThere(outputFolder);
        return Path.Combine(outputFolder, FS.GetFileName(item));
    }

    /// <summary>
    /// Pokud hledáš metodu ReplacePathToFile, je to tato. Sloučeny protože dělali totéž.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="changeFolderTo"></param>
    public static string ChangeDirectory(string fileName, string changeFolderTo)
    {
        string p = FS.GetDirectoryName(fileName);
        string fn = FS.GetFileName(fileName);
        return Path.Combine(changeFolderTo, fn);
    }

    #region For easy copy - GetNameWithoutSeries
    /// <summary>
    /// Do A1 se dává buď celá cesta ke souboru, nebo jen jeho název(může být i včetně neomezeně přípon)
    /// A2 říká, zda se má vrátit plná cesta ke souboru A1, upraví se pouze samotný název souboru
    /// Works for brackets, not dash
    /// </summary>
    public static string GetNameWithoutSeries(string p, bool path)
    {
        int serie;
        bool hasSerie = false;
        return GetNameWithoutSeries(p, path, out hasSerie, SerieStyle.Brackets, out serie);
    }
    //public static string GetNameWithoutSeries(string p, bool path, out bool hasSerie, SerieStyle serieStyle)
    //{
    //    int serie;
    //    return GetNameWithoutSeries(p, path, out hasSerie, serieStyle, out serie);
    //}

    public static (string, bool) GetNameWithoutSeriesNoOut(string p, bool path, SerieStyle serieStyle)
    {
        int serie;
        var result = GetNameWithoutSeries(p, path, out var hasSerie, serieStyle, out serie);
        return (result, hasSerie);
    }

    public static string GetNameWithoutSeries(string p, bool path, out bool hasSerie, SerieStyle serieStyle)
    {
        int serie;
        return GetNameWithoutSeries(p, path, out hasSerie, serieStyle, out serie);
    }

    /// <summary>
    ///
    /// Vrací vždy s příponou
    /// Do A1 se dává buď celá cesta ke souboru, nebo jen jeho název(může být i včetně neomezeně přípon)
    /// A2 říká, zda se má vrátit plná cesta ke souboru A1, upraví se pouze samotný název souboru
    /// When file has unknown extension, return SE
    /// Default for A4 was bracket
    /// </summary>
    /// <param name="p"></param>
    /// <param name="a1IsWithPath"></param>
    /// <param name="hasSerie"></param>
    public static string GetNameWithoutSeries(string p, bool a1IsWithPath, out bool hasSerie, SerieStyle serieStyle, out int serie)
    {
        serie = -1;
        hasSerie = false;
        string dd = string.Empty;

        if (a1IsWithPath)
        {
            dd = FS.WithEndSlash(FS.GetDirectoryName(p));
        }

        StringBuilder sbExt = new StringBuilder();
        string ext = FS.GetExtension(p);
        if (ext == string.Empty)
        {
            return p;
        }

        int pocetSerii = 0;

        p = SHParts.RemoveAfterLast(p, AllStrings.dot);
        sbExt.Append(ext);

        ext = sbExt.ToString();

        string g = p;

        if (dd.Length != 0)
        {
            g = g.Substring(dd.Length);
        }

        // Nejdříve ořežu všechny přípony a to i tehdy, má li soubor více přípon

        if (serieStyle == SerieStyle.Brackets || serieStyle == SerieStyle.All)
        {
            while (true)
            {
                g = g.Trim();
                int lb = g.LastIndexOf(AllChars.lb);
                int rb = g.LastIndexOf(AllChars.rb);

                if (lb != -1 && rb != -1)
                {
                    string between = SH.GetTextBetweenTwoCharsInts(g, lb, rb);
                    if (SH.IsNumber(between, EmptyArrays.Chars))
                    {
                        serie = int.Parse(between);
                        pocetSerii++;
                        // s - 4, on end (1) -
                        g = g.Substring(0, lb);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        if (serieStyle == SerieStyle.Dash || serieStyle == SerieStyle.All)
        {
            int dex = g.IndexOf(AllChars.dash);

            if (g[g.Length - 3] == AllChars.dash)
            {
                serie = int.Parse(g.Substring(g.Length - 2));
                g = g.Substring(0, g.Length - 3);
            }
            else if (g[g.Length - 2] == AllChars.dash)
            {
                serie = int.Parse(g.Substring(g.Length - 1));
                g = g.Substring(0, g.Length - 2);
            }
            if (serie != -1)
            {
                // To true hasSerie
                pocetSerii++;
            }
        }

        if (serieStyle == SerieStyle.Underscore || serieStyle == SerieStyle.All)
        {
            RemoveSerieUnderscore(ref serie, ref g, ref pocetSerii);
        }

        if (pocetSerii != 0)
        {
            hasSerie = true;
        }
        g = g.Trim();
        if (a1IsWithPath)
        {
            return dd + g + ext;
        }
        return g + ext;
    }

    public static string RemoveSerieUnderscore(string d)
    {
        int serie = 0;
        int pocetSerii = 0;
        RemoveSerieUnderscore(ref serie, ref d, ref pocetSerii);
        return d;
    }
    private static void RemoveSerieUnderscore(ref int serie, ref string g, ref int pocetSerii)
    {
        while (true)
        {
            int dex = g.LastIndexOf(AllChars.lowbar);
            if (dex != -1)
            {
                string serieS = g.Substring(dex + 1);
                g = g.Substring(0, dex);

                if (int.TryParse(serieS, out serie))
                {
                    pocetSerii++;
                }
            }
            else
            {
                break;
            }
        }
    }
    #endregion




    public static List<string> DirectoryListing(string path, string mask, SearchOption so)
    {
        var p = FS.GetFiles(path, mask, so, new GetFilesArgs { _trimA1AndLeadingBs = true });

        return p;
    }
}
