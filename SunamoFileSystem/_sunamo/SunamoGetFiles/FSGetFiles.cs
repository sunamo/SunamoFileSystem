// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoFileSystem._sunamo.SunamoGetFiles;

internal class FSGetFiles
{
    /// <summary>
    ///     Non recursive
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="fileExt"></param>
    internal static List<string> FilesOfExtension(string folder, string fileExt)
    {
        return GetFiles(folder, "*." + fileExt, SearchOption.TopDirectoryOnly);
    }


    /// <summary>
    ///     Keys returns with normalized ext
    ///     In case zero files of ext wont be included in dict
    /// </summary>
    /// <param name="folderFrom"></param>
    /// <param name="extensions"></param>
    internal static Dictionary<string, List<string>> FilesOfExtensions(string folderFrom, params string[] extensions)
    {
        var dict = new Dictionary<string, List<string>>();
        foreach (var item in extensions)
        {
            var ext = FS.NormalizeExtension(item);
            var files = GetFiles(folderFrom, "*" + ext, SearchOption.AllDirectories);
            if (files.Count != 0) dict.Add(ext, files);
        }

        return dict;
    }

    internal static List<string> GetFiles(string v1, string v2, SearchOption topDirectoryOnly)
    {
        return Directory.GetFiles(v1, v2, topDirectoryOnly).ToList();
    }


    /// <summary>
    ///     In item1 is all directories, in Item2 all files
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="ask"></param>
    /// <param name="searchOption"></param>
    /// <param name="_trimA1"></param>
    internal static List<string> GetFilesEveryFolder(string folder, string mask, SearchOption searchOption,
        GetFilesEveryFolderArgsFS e = null)
    {

        if (e == null) e = new GetFilesEveryFolderArgsFS();
        // TODO: některé soubory vrací vícekrát. toto je workaround než zjistím proč
        // TODO: je důležité se toho zbavit co nejdříve protože při načítání to zbytečně zpomaluje
        var list = new List<string>();
        List<string> dirs = null;
        var measureTime = false;
        //if (measureTime)
        //{
        //    //StopwatchStatic.Start();
        //}
        // There is not exc handle needed, its slowly then
        //try
        //{
        if (e.usePbTime)
        {
            var message = Translate.FromKey(XlfKeys.Loading) + " " + Translate.FromKey(XlfKeys.FoldersTree) + "...";
            e.InsertPbTime(60);
            e.UpdateTbPb(message);
        }

        dirs = FSGetFolders.GetFoldersEveryFolder(folder, new GetFoldersEveryFolderArgs(e)).ToList();
#if DEBUG
        //int before = dirs.Count;
#endif
        if (e.FilterFoundedFolders != null)
        {
            string si = null;
            for (var i = dirs.Count - 1; i >= 0; i--)
            {
                si = dirs[i];
                //if (si.Contains(@"\standard\.git"))
                //{
                //}
                if (!e.FilterFoundedFolders.Invoke(si)) dirs.RemoveAt(i);
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
        //    throw new Exception(Translate.FromKey(XlfKeys.GetFilesWithPath)+": " + folder);
        //}

        #endregion

        //if (measureTime)
        //{
        //    StopwatchStatic.StopAndPrintElapsed("GetFoldersEveryFolder");
        //}
        //if (measureTime)
        //{
        //    StopwatchStatic.Start();
        //}
        if (e.usePb)
        {
            var message = Translate.FromKey(XlfKeys.Loading) + " " + Translate.FromKey(XlfKeys.FilesTree) + "...";
            e.InsertPb(dirs.Count);
            e.UpdateTbPb(message);
        }

        var data = new List<string>();
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
                //TF.WaitD();
#endif
                //data.Clear();
                var f = GetFiles(item, mask, SearchOption.TopDirectoryOnly);
                data.AddRange(f);
                if (e.getNullIfThereIsMoreThanXFiles != -1)
                    if (data.Count > e.getNullIfThereIsMoreThanXFiles)
                    {
                        if (e.usePb) e.Done();
                        return null;
                    }
            }
            catch (Exception ex)
            {
                ThrowEx.Custom(ex);
                // Not throw exception, it's probably Access denied on Documents and Settings etc
                //ThrowEx.FileSystemException( ex);
            }

            if (e.usePb) e.DoneOnePercent();
#if DEBUG
            //before = data.Count;
#endif
            if (e.FilterFoundedFiles != null)
                for (var i = data.Count - 1; i >= 0; i--)
                    if (!e.FilterFoundedFiles(data[i]))
                        data.RemoveAt(i);
#if DEBUG
            //after = data.Count;
            //if (before != 0 && after == 0)
            //{
            //}
#endif
            list.AddRange(data);
            data.Clear();
        }

        list = list.Distinct().ToList();
        if (e.usePb) e.Done();
        //if (measureTime)
        //{
        //    StopwatchStatic.StopAndPrintElapsed("GetFiles");
        //}
        //CAChangeContent.ChangeContent0(null, list, d2 => SH.FirstCharUpper(d2));
        for (var i = 0; i < list.Count; i++) list[i] = SH.FirstCharUpper(list[i]);
        if (e._trimA1AndLeadingBs)
            //list = CAChangeContent.ChangeContent0(null, list, d3 => d3 = d3.Replace(folder, "").TrimStart('\\'));
            for (var i = 0; i < list.Count; i++)
                list[i] = list[i].Replace(folder, "").TrimStart('\\');
        return list;
    }






    /// <summary>
    ///     When is occur Access denied exception, use GetFilesEveryFolder, which find files in every folder
    ///     A1 have to be with ending backslash
    ///     A4 must have underscore otherwise is suggested while I try type true
    ///     A2 can be delimited by semicolon. In case more extension use GetFilesOfExtensions
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    /// <param name="searchOption"></param>
    internal static List<string> GetFiles(string folder2, string mask, SearchOption searchOption,
        GetFilesArgsFS a = null)
    {
#if DEBUG
        if (folder2.TrimEnd('\\') == @"\monoConsoleSqlClient")
        {
        }
#endif
        if (!Directory.Exists(folder2) && !folder2.Contains(";"))
            //ThisApp.Warning(folder2 + "does not exists");
            return new List<string>();
        if (a == null) a = new GetFilesArgsFS();
        var folders = SHSplit.Split(folder2, ";");
        //if (CA.PostfixIfNotEnding != null)
        //{
        //    CA.PostfixIfNotEnding("\"", folders);
        //}
        for (var i = 0; i < folders.Count; i++) folders[i] = folders[i].TrimEnd('\\') + "\\";
        var list = new List<string>();
        foreach (var folder in folders)
            if (!Directory.Exists(folder))
            {
            }
            else
            {
                //Task.Run<>(async () => await FunctionAsync());
                //var result = Task.Run<List<string>>(async () => await GetFilesMoreMascAsync(folder, mask, searchOption));
                //return result.Result;
                var l2 = GetFilesMoreMasc(folder, mask, searchOption,
                    new GetFilesMoreMascArgs { followJunctions = a.followJunctions });
                list.AddRange(l2);

                #region Commented

                //if (mask.Contains(";"))
                //{
                //    //list = new List<string>();
                //    var masces = SHSplit.Split(mask, ";");
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
                //        var files2 = files.Select(data => data.FullName);
                //        //list.AddRange(GetFiles(folder3, masc, searchOption));
                //        list.AddRange(files2);
                //    }
                //    catch (Exception ex)
                //    {
                //    }
                //}

                #endregion
            }

        FilterByGetFilesArgs(list, folders, a);
        return list;
    }


    internal static List<string> GetFilesMoreMasc(string path, string masc, SearchOption searchOption,
        GetFilesMoreMascArgs e = null)
    {
        if (e == null) e = new GetFilesMoreMascArgs();
#if DEBUG
        string data = null;
        if (e.LoadFromFileWhenDebug)
        {
            var text = FS.ReplaceInvalidFileNameChars(string.Join(path, masc, searchOption));
            //d = AppData.ci.GetFile(AppFolders.Cache, "GetFilesMoreMasc" + text + ".txt");
            //if (File.Exists(data))
            //{
            //    return File.ReadAllText(path).ToList();
            //}
        }
#endif
        var count = ",";
        var sc = ";";
        var result = new List<string>();
        var masks = new List<string>();
        if (masc.Contains(count))
            masks.AddRange(SHSplit.Split(masc, count));
        else if (masc.Contains(sc))
            masks.AddRange(SHSplit.Split(masc, sc));
        else
            masks.Add(masc);

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
            foreach (var item2 in masks)
            {
                //if(SH.ContainsOnlyCase())
                var item = FS.AllIncludeIfOnlyLetters(item2);
                try
                {
                    result.AddRange(GetFiles(path, item, searchOption));
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith(NetFxExceptionsNotTranslateAble
                            .TheNameOfTheFileCannotBeResolvedByTheSystem))
                    {
                        // Nesmysl, celou dobu musím vědět text čím pracuji
                        //FS.TryDeleteDirectoryOrFile(path);
                    }

                    ThrowEx.Custom(ex);
                }
            }
        else
            foreach (var item2 in masks)
            {
                var item = FS.AllIncludeIfOnlyLetters(item2);
                result.AddRange(GetFiles(path, item, searchOption));
            }

        if (result.Count > 0) result[0] = SH.FirstCharUpper(result[0]);
        CAChangeContent.ChangeContent0(null, result, SH.FirstCharUpper);
#if DEBUG
        if (e.LoadFromFileWhenDebug)
            if (File.Exists(data))
                File.WriteAllLinesAsync(data, result);
#endif
        return result;
    }

    internal static void FilterByGetFilesArgs(List<string> list, IEnumerable<string> folders, GetFilesArgsFS a)
    {
        if (a == null) a = new GetFilesArgsFS();
        CAChangeContent.ChangeContent0(null, list, data => SH.FirstCharUpper(data));
        if (a._trimA1AndLeadingBs)
            foreach (var folder in folders)
                list = CAChangeContent.ChangeContent0(null, list, data => data = data.Replace(folder, "").TrimEnd('\\'));
        if (a._trimExt)
            foreach (var folder in folders)
                list = CAChangeContent.ChangeContent0(null, list, data => data = SHParts.RemoveAfterLast(data, '.'));
        if (a.excludeFromLocationsCOntains != null)
            // I want to find files recursively
            foreach (var item in a.excludeFromLocationsCOntains)
                list = list.Where(data => !data.Contains(item)).ToList();
        //CA.RemoveWhichContains(list, item, false);
        Dictionary<string, DateTime> dictLastModified = null;
        var isLastModifiedFromFn = a.LastModifiedFromFn != null;
        if (a.dontIncludeNewest || a.byDateOfLastModifiedAsc || isLastModifiedFromFn)
        {
            dictLastModified = new Dictionary<string, DateTime>();
            foreach (var item in list)
            {
                DateTime? dt = null;
                if (isLastModifiedFromFn) dt = a.LastModifiedFromFn(Path.GetFileNameWithoutExtension(item));
                if (!dt.HasValue) dt = FS.LastModified(item);
                dictLastModified.Add(item, dt.Value);
            }

            list = dictLastModified.OrderBy(t => t.Value).Select(result => result.Key).ToList();
        }

        if (a.dontIncludeNewest) list.RemoveAt(list.Count - 1);
        if (a.excludeWithMethod != null) a.excludeWithMethod.Invoke(list);
    }

    /// <summary>
    ///     No recursive, all extension
    /// </summary>
    /// <param name="path"></param>
    internal static List<string> GetFiles(string path)
    {
        return GetFiles(path, "*", SearchOption.TopDirectoryOnly);
    }

    internal static List<string> AllFilesInFolders(IList<string> folders, IList<string> exts, SearchOption so,
        GetFilesArgsFS a = null)
    {
        var files = new List<string>();
        foreach (var item in folders)
            foreach (var ext in exts)
            {
                var data = FS.MascFromExtension(ext);
                files.AddRange(GetFiles(item, data, so, a));
            }
        return files;
    }



}