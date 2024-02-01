
namespace SunamoFileSystem;
using SunamoFileSystem._sunamo;
using SunamoFileSystemShared;
using System.Text.RegularExpressions;



public delegate bool? ExistsDirectory(string path);

public partial class FS : FSSH
{
    public static string WithoutEndSlash(string v)
    {
        return WithoutEndSlash(ref v);
    }




    public static string WithoutEndSlash(ref string v)
    {
        v = v.TrimEnd(AllChars.bs);
        return v;
    }

    public static string MascFromExtension(string ext2 = AllStrings.asterisk)
    {
        if (char.IsLetterOrDigit(ext2[0]))
        {
            // For wildcard, dot (simply non letters) include .
            ext2 = "." + ext2;
        }
        if (!ext2.StartsWith("*"))
        {
            ext2 = "*" + ext2;
        }
        if (!ext2.StartsWith("*.") && ext2.StartsWith(AllStrings.dot))
        {
            ext2 = "*." + ext2;
        }

        return ext2;

        //if (ext2 == ".*")
        //{
        //    return "*.*";
        //}


        //var ext = Path.GetExtension(ext2);
        //var fn = Path.GetFileNameWithoutExtension(ext2);
        //// isContained must be true, in BundleTsFile if false masc will be .ts, not *.ts and won't found any file
        //var isContained = AllExtensionsHelper.IsContained(ext);
        //ComplexInfoString cis = new ComplexInfoString(fn);

        ////Already tried
        ////(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0);
        //// (cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0); - in MoveClassElementIntoSharedFileUC
        //// !(!ext.Contains("*") && !ext.Contains("?") && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0)) - change due to MoveClassElementIntoSharedFileUC

        //// not working for *.aspx.cs
        ////var isNoMascEntered = !(!ext2.Contains("*") && !ext2.Contains("?") && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0));
        //// Is succifient one of inner condition false and whole is true

        //var isNoMascEntered = !((ext2.Contains("*") || ext2.Contains("?")));// && !(cis.QuantityLowerChars > 0 || cis.QuantityUpperChars > 0));
        //// From base of logic - isNoMascEntered must be without !. When something another wont working, edit only evaluate of condition above
        //if (!ext.StartsWith("*.") && isNoMascEntered && isContained && ext == Path.GetExtension( ext2))
        //{
        //    // Dont understand why, when I insert .aspx.cs, then return just .aspx without *
        //    //if (cis.QuantityUpperChars > 0 || cis.QuantityLowerChars > 0)
        //    //{
        //    //    return ext2;
        //    //}

        //    var vr = AllStrings.asterisk + AllStrings.dot + ext2.TrimStart(AllChars.dot);
        //    return vr;
        //}

        //return ext2;
    }

    public static bool IsCountOfFilesMoreThan(string bpMb, string masc, int getNullIfThereIsMoreThanXFiles)
    {
        var f = GetFilesEveryFolder(bpMb, masc, SearchOption.AllDirectories, new GetFilesEveryFolderArgs { getNullIfThereIsMoreThanXFiles = getNullIfThereIsMoreThanXFiles });
        return f == null;

    }

    public static List<string> GetFiles(string folderPath, bool recursive)
    {
        return GetFiles(folderPath, FS.MascFromExtension(), recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
    }

    //    public static
    //#if ASYNC
    //    async Task<string>
    //#else
    //string
    //#endif
    //    ReadAllText(string filename)
    //    {
    //        return
    //#if ASYNC
    //        await
    //#endif
    //        File.ReadAllTextAsync(filename);
    //    }

    /// <summary>
    /// A2 is path of target file
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fileTo"></param>
    /// <param name="co"></param>
    public static void MoveFile(string item, string fileTo, FileMoveCollisionOption co)
    {
        if (CopyMoveFilePrepare(ref item, ref fileTo, co))
        {
            try
            {
                item = FS.MakeUncLongPath(item);
                fileTo = FS.MakeUncLongPath(fileTo);

                if (co == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo))
                {
                    return;
                }
                File.Move(item, fileTo);
            }
            catch (Exception ex)
            {
                ThisApp.Error(item + " : " + ex.Message);
            }
        }
        else
        {
        }
    }

    public static Action<string> DeleteFileMaybeLocked;

    public static bool CopyMoveFilePrepare(ref string item, ref string fileTo, FileMoveCollisionOption co)
    {
        //var fileTo = fileTo2.ToString();
        item = Consts.UncLongPath + item;
        MakeUncLongPath(ref fileTo);
        FS.CreateUpfoldersPsysicallyUnlessThere(fileTo);

        // Toto tu je důležité, nevím který kokot to zakomentoval
        if (File.Exists(fileTo))
        {
            if (co == FileMoveCollisionOption.AddFileSize)
            {
                var newFn = FS.InsertBetweenFileNameAndExtension(fileTo, AllStrings.space + new FileInfo(item).Length);
                if (File.Exists(newFn))
                {
                    File.Delete(item);
                    return true;
                }
                fileTo = newFn;
            }
            else if (co == FileMoveCollisionOption.AddSerie)
            {
                int serie = 1;
                while (true)
                {
                    var newFn = FS.InsertBetweenFileNameAndExtension(fileTo, " (" + serie + AllStrings.rb);
                    if (!File.Exists(newFn))
                    {
                        fileTo = newFn;
                        break;
                    }
                    serie++;
                }
            }
            else if (co == FileMoveCollisionOption.DiscardFrom)
            {
                // Cant delete from because then is file deleting
                if (DeleteFileMaybeLocked != null)
                {
                    DeleteFileMaybeLocked(item);
                }
                else
                {
                    File.Delete(item);
                }

            }
            else if (co == FileMoveCollisionOption.Overwrite)
            {
                if (DeleteFileMaybeLocked != null)
                {
                    DeleteFileMaybeLocked(fileTo);
                }
                else
                {
                    File.Delete(fileTo);
                }
            }
            else if (co == FileMoveCollisionOption.LeaveLarger)
            {
                long fsFrom = new FileInfo(item).Length;
                long fsTo = new FileInfo(fileTo).Length;
                if (fsFrom > fsTo)
                {
                    File.Delete(fileTo);
                }
                else //if (fsFrom < fsTo)
                {
                    File.Delete(item);
                }
            }
            else if (co == FileMoveCollisionOption.DontManipulate)
            {
                if (File.Exists(fileTo))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static long GetFileSize(string item)
    {
        FileInfo fi = null;
        try
        {
            fi = new FileInfo(item);
        }
        catch (Exception ex)
        {
            // Například příliš dlouhý název souboru
            return 0;
        }
        if (fi.Exists)
        {
            return fi.Length;
        }
        return 0;
    }






    public static void CopyAllFilesRecursively(string p, string to, FileMoveCollisionOption co, string contains = null)
    {
        CopyMoveAllFilesRecursively(p, to, co, false, contains, SearchOption.AllDirectories);
    }

    /// <summary>
    /// A4 contains can use ! for negation
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    /// <param name="co"></param>
    /// <param name="contains"></param>
    public static void CopyAllFiles(string p, string to, FileMoveCollisionOption co, string contains = null)
    {
        CopyMoveAllFilesRecursively(p, to, co, false, contains, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    /// If want use which not contains, prefix A4 with !
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    /// <param name="co"></param>
    /// <param name="mustContains"></param>
    private static void CopyMoveAllFilesRecursively(string p, string to, FileMoveCollisionOption co, bool move, string mustContains, SearchOption so)
    {
        var files = GetFiles(p, AllStrings.asterisk, so);
        if (!string.IsNullOrEmpty(mustContains))
        {
            foreach (var item in files)
            {
                if (SH.IsContained(item, mustContains))
                {
                    if (item.Contains("node_modules"))
                    {

                    }
                    MoveOrCopy(p, to, co, move, item);
                }
            }
        }
        else
        {
            foreach (var item in files)
            {
                MoveOrCopy(p, to, co, move, item);
            }
        }
    }

    private static void MoveOrCopy(string p, string to, FileMoveCollisionOption co, bool move, string item)
    {
        string fileTo = to + item.Substring(p.Length);
        if (move)
        {
            MoveFile(item, fileTo, co);
        }
        else
        {
            CopyFile(item, fileTo, co);
        }
    }


    public static Func<string, bool, List<Process>> fileUtilWhoIsLocking = null;

    public static

    void
    CopyFile(string item2, string fileTo2, FileMoveCollisionOption co, bool terminateProcessIfIsInUsed = false)
    {
        var fileTo = fileTo2.ToString();
        var item = item2;

        var rr =

        CopyMoveFilePrepare(ref item, ref fileTo, co);

        if (rr)
        {
            if (co == FileMoveCollisionOption.DontManipulate &&

           File.Exists(fileTo))
            {
                return;
            }

            CopyFile(item, fileTo, terminateProcessIfIsInUsed);
        }
    }

    /// <summary>
    /// Copy file by ordinal way
    ///
    /// tady byly 2 metody CopyFile(string, string, bool)
    /// jedna s A3 terminateProcessIfIsInUsed, druhá s overwrite
    /// Ta druhá jen volala A3 s FileMoveCollisionOption.Overwrite
    /// </summary>
    /// <param name="jsFiles"></param>
    /// <param name="v"></param>
    public static void CopyFile(string jsFiles, string v, bool terminateProcessIfIsInUsed = false)
    {
        try
        {
            File.Copy(jsFiles, v, true);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("because it is being used by another process") && terminateProcessIfIsInUsed)
            {
                if (fileUtilWhoIsLocking != null)
                {
                    var pr = fileUtilWhoIsLocking(jsFiles, true);
                    foreach (var item in pr)
                    {
                        item.Kill();
                    }
                }
                else
                {
                    // Používá se i ve web, musel bych tam includovat spoustu metod
                    //PH.ShutdownProcessWhichOccupyFileHandleExe(jsFiles);
                }

                try
                {
                    File.Copy(jsFiles, v, true);
                }
                catch (Exception)
                {
                    // Pokud se to ani na druhý pokus nepodaří, tak už to jebu
                }
            }
            else
            {
                throw;
            }

        }


    }

    public static void CopyFile(string item, string fileTo2, FileMoveCollisionOption co)
    {
        var fileTo = fileTo2.ToString();
        if (CopyMoveFilePrepare(ref item, ref fileTo, co))
        {
            if (co == FileMoveCollisionOption.DontManipulate && File.Exists(fileTo))
            {
                return;
            }

            File.Copy(item, fileTo);


        }
    }

    public static DateTime LastModified(string rel)
    {
        if (File.Exists(rel))
        {
            return File.GetLastWriteTime(rel);

            // FileInfo mi držel soubor a vznikali chyby The process cannot access the file
            //var f = new FileInfo(rel);
            //var r = f.LastWriteTime;
            //return r;
        }
        return DateTime.MinValue;

    }



    public static bool TryDeleteDirectoryOrFile(string v)
    {
        if (!FS.TryDeleteDirectory(v))
        {
            return FS.TryDeleteFile(v);
        }
        return true;
    }

    //public static Func<string, List<string>> InvokePs;

    /// <summary>
    /// Before start you can create instance of PowershellRunner to try do it with PS
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static bool TryDeleteDirectory(string v)
    {
        if (!Directory.Exists(v))
        {
            return true;
        }

        try
        {
            Directory.Delete(v, true);
            return true;
        }
        catch (Exception ex)
        {

            // Je to try takže nevím co tu dělá tohle a
            //ThrowEx.FolderCannotBeDeleted(v, ex);
            //var result = InvokePs(v);
            //if (result.Count > 0)
            //{
            //    return false;
            //}
        }

        var files = GetFiles(v, "*", SearchOption.AllDirectories);
        foreach (var item in files)
        {
            File.SetAttributes(item, FileAttributes.Normal);
        }

        try
        {
            Directory.Delete(v, true);
            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
    }




    public static string AllIncludeIfOnlyLetters(string item)
    {
        item = item.ToLower();
        //if ( SH.ContainsOnlyCase(item.ToLower(), false, false))
        //{
        item = "*." + item;
        //}


        return item;
    }


    public static List<string> GetFilesMoreMasc(string path, string masc, SearchOption searchOption, GetFilesMoreMascArgs e = null)

    {
        if (e == null)
        {
            e = new GetFilesMoreMascArgs();
        }

#if DEBUG
        string d = null;

        if (e.LoadFromFileWhenDebug)
        {
            var s = FS.ReplaceInvalidFileNameChars(string.Join(path, masc, searchOption));
            d = AppData.ci.GetFile(AppFolders.Cache, "GetFilesMoreMasc" + s + ".txt");

            if (File.Exists(d))
            {
                return File.ReadAllLines(path).ToList();
            }
        }
#endif

        var c = AllStrings.comma;
        var sc = AllStrings.sc;
        List<string> result = new List<string>();
        List<string> masks = new List<string>();

        if (masc.Contains(c))
        {
            masks.AddRange(SHSplit.Split(masc, c));
        }
        else if (masc.Contains(sc))
        {
            masks.AddRange(SHSplit.Split(masc, sc));
        }
        else
        {
            masks.Add(masc);
        }

        #region Added 27-8-23
        //if (searchOption == SearchOption.AllDirectories)
        //{
        //    foreach (var item in masks)
        //    {
        //        result.AddRange(GetFiles(path, item, SearchOption.TopDirectoryOnly));
        //    }
        //}

        #endregion

        if (e.deleteFromDriveWhenCannotBeResolved)
        {
            foreach (var item2 in masks)
            {
                //if(SH.ContainsOnlyCase())

                var item = AllIncludeIfOnlyLetters(item2);

                try
                {
                    result.AddRange(GetFiles(path, item, searchOption));
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith(NetFxExceptionsNotTranslateAble.TheNameOfTheFileCannotBeResolvedByTheSystem))
                    {
                        FS.TryDeleteDirectoryOrFile(path);
                    }
                }

            }
        }
        else
        {
            foreach (var item2 in masks)
            {
                var item = AllIncludeIfOnlyLetters(item2);
                result.AddRange(GetFiles(path, item, searchOption));
            }
        }


        if (result.Count > 0)
        {
            result[0] = SH.FirstCharUpper(result[0]);
        }

        CAChangeContent.ChangeContent0(null, result, SH.FirstCharUpper);


#if DEBUG
        if (e.LoadFromFileWhenDebug)
        {
            if (File.Exists(d))
            {
                File.WriteAllLinesAsync(d, result);
            }
        }
#endif

        return result;
    }



    /// <summary>
    ///
    /// When is occur Access denied exception, use GetFilesEveryFolder, which find files in every folder
    /// A1 have to be with ending backslash
    /// A4 must have underscore otherwise is suggested while I try type true
    /// A2 can be delimited by semicolon. In case more extension use GetFilesOfExtensions
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    /// <param name="searchOption"></param>
    public static List<string> GetFiles(string folder2, string mask, SearchOption searchOption, GetFilesArgs a = null)
    {
#if DEBUG
        if (folder2.TrimEnd(AllChars.bs) == @"\monoConsoleSqlClient")
        {

        }
#endif

        if (!Directory.Exists(folder2) && !folder2.Contains(";"))
        {
            ThisApp.Warning(folder2 + "does not exists");
            return new List<string>();
        }

        if (a == null)
        {
            a = new GetFilesArgs();
        }

        var folders = SHSplit.Split(folder2, AllStrings.sc);

        //if (CA.PostfixIfNotEnding != null)
        //{
        //    CA.PostfixIfNotEnding(AllStrings.bs, folders);
        //}

        for (int i = 0; i < folders.Count; i++)
        {
            folders[i] = folders[i].TrimEnd('\\') + "\\";
        }

        List<string> list = new List<string>();
        foreach (var folder in folders)
        {
            if (!Directory.Exists(folder))
            {

            }
            else
            {
                //Task.Run<>(async () => await FunctionAsync());
                //var r = Task.Run<List<string>>(async () => await GetFilesMoreMascAsync(folder, mask, searchOption));
                //return r.Result;
                var l2 = GetFilesMoreMasc(folder, mask, searchOption, new GetFilesMoreMascArgs { followJunctions = a.followJunctions });
                list.AddRange(l2);

                #region Commented
                //if (mask.Contains(AllStrings.sc))
                //{
                //    //list = new List<string>();
                //    var masces = SHSplit.Split(mask, AllStrings.sc);

                //    foreach (var item in masces)
                //    {
                //        var masc = item;
                //        if (getFilesArgs.useMascFromExtension)
                //        {
                //            masc = FS.MascFromExtension(item);
                //        }

                //        try
                //        {
                //            list.AddRange(GetFilesMoreMasc(folder, masc, searchOption));
                //        }
                //        catch (Exception ex)
                //        {
                //        }

                //    }
                //}
                //else
                //{

                //    try
                //    {
                //        var folder3 = FS.WithoutEndSlash(folder);
                //        DirectoryInfo di = new DirectoryInfo(folder3);
                //        var masc = mask;
                //        if (getFilesArgs.useMascFromExtension)
                //        {
                //            masc = FS.MascFromExtension(mask);
                //        }

                //        var files = di.GetFiles(masc, searchOption);
                //        var files2 = files.Select(d => d.FullName);

                //        //list.AddRange(GetFiles(folder3, masc, searchOption));
                //        list.AddRange(files2);
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}
                #endregion
            }
        }
        FilterByGetFilesArgs(list, folders, a);

        return list;
    }

    public static void FilterByGetFilesArgs(List<string> list, List<string> folders, GetFilesArgs a)
    {
        if (a == null)
        {
            a = new GetFilesArgs();
        }

        CAChangeContent.ChangeContent0(null, list, d => SH.FirstCharUpper(d));


        if (a._trimA1AndLeadingBs)
        {
            foreach (var folder in folders)
            {
                list = CAChangeContent.ChangeContent0(null, list, d => d = d.Replace(folder, "").TrimEnd(AllChars.bs));
            }
        }

        if (a._trimExt)
        {
            foreach (var folder in folders)
            {
                list = CAChangeContent.ChangeContent0(null, list, d => d = SHParts.RemoveAfterLast(d, AllChars.dot));
            }
        }

        if (a.excludeFromLocationsCOntains != null)
        {
            // I want to find files recursively
            foreach (var item in a.excludeFromLocationsCOntains)
            {
                list = list.Where(d => !d.Contains(item)).ToList();
                //CA.RemoveWhichContains(list, item, false);
            }
        }

        Dictionary<string, DateTime> dictLastModified = null;

        bool isLastModifiedFromFn = a.LastModifiedFromFn != null;

        if (a.dontIncludeNewest || (a.byDateOfLastModifiedAsc || isLastModifiedFromFn))
        {
            dictLastModified = new Dictionary<string, DateTime>();
            foreach (var item in list)
            {
                DateTime? dt = null;

                if (isLastModifiedFromFn)
                {
                    dt = a.LastModifiedFromFn(Path.GetFileNameWithoutExtension(item));
                }

                if (!dt.HasValue)
                {
                    dt = FS.LastModified(item);
                }

                dictLastModified.Add(item, dt.Value);
            }
            list = dictLastModified.OrderBy(t => t.Value).Select(r => r.Key).ToList();
        }

        if (a.dontIncludeNewest)
        {
            list.RemoveAt(list.Count - 1);
        }



        if (a.excludeWithMethod != null)
        {
            a.excludeWithMethod.Invoke(list);
        }


    }


    /// <summary>
    /// No recursive, all extension
    /// </summary>
    /// <param name="path"></param>
    public static List<string> GetFiles(string path)
    {
        return GetFiles(path, AllStrings.asterisk, SearchOption.TopDirectoryOnly);
    }

    /// <summary>
    /// Get number higher by one from the number filenames with highest value (as 3.txt)
    /// </summary>
    /// <param name="slozka"></param>
    /// <param name="fn"></param>
    /// <param name="ext"></param>
    public static string GetFileSeries(string slozka, string fn, string ext)
    {
        int dalsi = 0;
        var soubory = GetFiles(slozka);
        foreach (string item in soubory)
        {
            int p;
            string withoutFn = new Regex(fn).Replace(Path.GetFileName(item), "", 1); /*SHReplace.ReplaceOnce(Path.GetFileName(item), fn, "")));*/
            string withoutFnAndExt = SHReplace.ReplaceOnce(withoutFn, ext, "");
            withoutFnAndExt = withoutFnAndExt.TrimStart(AllChars.lowbar);
            if (int.TryParse(withoutFnAndExt, out p))
            {
                if (p > dalsi)
                {
                    dalsi = p;
                }
            }
        }

        dalsi++;

        return Path.Combine(slozka, fn + AllStrings.lowbar + dalsi + ext);
    }





    ///// <summary>
    ///// If path ends with backslash, Path.GetDirectoryName returns empty string
    ///// </summary>
    ///// <param name="rp"></param>
    //public static string GetFileName(string rp)
    //{
    //    rp = rp.TrimEnd(AllChars.bs);
    //    int dex = rp.LastIndexOf(AllChars.bs);
    //    return rp.Substring(dex + 1);
    //}







    //public static bool ExistsDirectory<StorageFolder, StorageFile>(string item, AbstractCatalog<StorageFolder, StorageFile> ac = null, bool _falseIfContainsNoFile = false)
    //{
    //    if (ac == null)
    //    {
    //        return ExistsDirectoryWorker(item, _falseIfContainsNoFile);
    //    }
    //    else
    //    {
    //        // Call from Apps
    //        return BTS.GetValueOfNullable(ac.Directory.Exists.Invoke(item));
    //    }
    //}



    public static void MakeUncLongPath<StorageFolder, StorageFile>(ref StorageFile path, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac == null)
        {
            path = (StorageFile)(dynamic)MakeUncLongPath(path.ToString());
        }
        else
        {
            ThrowNotImplementedUwp();
        }
        //return path;
    }



    //public static string MakeUncLongPath(string path)
    //{
    //    return se.FS.MakeUncLongPath(path);
    //}

    //public static string MakeUncLongPath(ref string path)
    //{
    //    return se.FS.MakeUncLongPath(ref path);
    //}

    /// <summary>
    /// For empty or whitespace return false.
    /// </summary>
    /// <param name="selectedFile"></param>
    public static bool ExistsFileAc<StorageFolder, StorageFile>(StorageFile selectedFile, AbstractCatalog<StorageFolder, StorageFile> ac = null)
    {
        if (ac == null)
        {
            return File.Exists(selectedFile.ToString());
        }
        return ac.fs.existsFile.Invoke(selectedFile);
    }



    ///// <summary>
    ///// Create all upfolders of A1, if they dont exist
    ///// </summary>
    ///// <param name="nad"></param>
    //public static void CreateUpfoldersPsysicallyUnlessThereAc<StorageFolder, StorageFile>(StorageFile nad, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        CreateUpfoldersPsysicallyUnlessThere(nad.ToString());
    //    }
    //    else
    //    {
    //        CreateFoldersPsysicallyUnlessThereFolder<StorageFolder, StorageFile>(Path.GetDirectoryName<StorageFolder, StorageFile>(nad, ac), ac);
    //    }
    //}

    ///// <summary>
    ///// Works with and without end backslash
    ///// Return with backslash
    ///// </summary>
    ///// <param name="rp"></param>
    //public static StorageFolder GetDirectoryName<StorageFolder, StorageFile>(StorageFile rp2, AbstractCatalog<StorageFolder, StorageFile> ac)
    //{
    //    if (ac != null)
    //    {
    //        return ac.Path.GetDirectoryName.Invoke(rp2);
    //    }

    //    var rp = rp2.ToString();
    //    return (dynamic)GetDirectoryName(rp);
    //}


    public static void CreateFoldersPsysicallyUnlessThereFolder<StorageFolder, StorageFile>(StorageFolder nad, AbstractCatalog<StorageFolder, StorageFile> ac)
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

    public static bool? ExistsDirectoryNull(string item)
    {
        return ExistsDirectoryNull(item, false);
    }

    public static bool? ExistsDirectoryNull(string item, bool _falseIfContainsNoFile = false)
    {
        return ExistsDirectory(item, _falseIfContainsNoFile);
    }

    public static bool ExistsDirectory(string item, bool _falseIfContainsNoFile = false)
    {
        return Directory.Exists(item);
        //return ExistsDirectory<string, string>(item, null, _falseIfContainsNoFile);
    }





    private static Type type = typeof(FS);

    ///// <summary>
    ///// All occurences Path's method in sunamo replaced
    ///// </summary>
    ///// <param name="v"></param>
    //public static void CreateDirectory(string v)
    //{
    //    try
    //    {
    //        Directory.CreateDirectory(v);
    //    }
    //    catch (NotSupportedException)
    //    {


    //    }
    //}








}
