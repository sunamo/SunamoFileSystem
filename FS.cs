using SunamoCollectionWithoutDuplicates;
using SunamoStringJoin;

namespace SunamoFileSystem;





public partial class FS
{
    public static void TrimBasePathAndTrailingBs(List<string> s, string basePath)
    {
        for (int i = 0; i < s.Count; i++)
        {
            s[i] = s[i].Substring(basePath.Length);

            s[i] = s[i].TrimEnd('\\');
        }
    }

    public static string GetFileNameWithoutOneExtension(string path)
    {
        return SH.RemoveAfterLast(path, "\\");
    }

    public static string GetActualDateTime()
    {
        DateTime dt = DateTime.Now;
        return ReplaceIncorrectCharactersFile(dt.ToString());
    }

    public static string GetDirectoryNamePath(string s)
    {
        return Path.GetDirectoryName(s);
    }

    public static
#if ASYNC
    async Task<List<string>>
#else
List<string>
#endif
    KeepOnlyWhichIsNotInFiles(List<string> opts, List<string> paths)
    {
        CollectionWithoutDuplicates<string> c = new CollectionWithoutDuplicates<string>();
        foreach (var item in paths)
        {
            c.AddRange(
#if ASYNC
            await
#endif
            TF.ReadAllLines(item));
        }

        CAG.CompareList(opts, c.c);

        return opts;
    }

    public static List<string> GetFilesWithoutNodeModules(string item, string masc, bool? rec, GetFilesArgs a = null)
    {
        if (a == null)
        {
            a = new GetFilesArgs();
        }

        a.excludeFromLocationsCOntains = CA.AddOrCreateInstance(a.excludeFromLocationsCOntains, "de_mo");

        return GetFiles(item, masc, rec, a);
    }

    /// <summary>
    /// C:\repos\EOM-7\Marvin\Module.VBtO\Clients\src\apps\vbto\src\pages\Administration\Administration.test.tsx
    /// ../../../../../../../node_modules/@mui/material/Switch/Switch
    ///
    /// => C:\repos\EOM-7\Marvin\Module.VBtO\Clients\node_modules\@mui\material\Switch\Switch
    /// => OK
    /// </summary>
    /// <param name="fullPathToSecondFile"></param>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public static string RelativeToAbsolutePath(string fullPathToSecondFile, string relativePath)
    {
        string fullPathToFirstFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(fullPathToSecondFile), relativePath));
        return fullPathToFirstFile;
    }

    // Proč to volám zde? Má se to volat v aplikacích kde to potřebuji
    //static AllExtensionsHelper()
    //{
    //    // Must call Initialize here, not in Loaded of Window. when I run auto code in debug, it wont be initialized as is needed.
    //    Initialize();
    //}

    public static void Initialize(bool callAlsoAllExtensionsHelperWithoutDotInitialize = false)
    {
        if (callAlsoAllExtensionsHelperWithoutDotInitialize)
        {
            AllExtensionsHelperWithoutDot.Initialize();
        }
    }

    /// <summary>
    ///     Usage: SunamoFubuCsprojFileHelper.GetProjectsInSlnFile
    ///     Cant name GetAbsolutePath because The call is ambiguous between the following methods or properties:
    ///     'CAChangeContent.ChangeContent0(null,List
    ///     <string>, Func<string, string, string>)' and 'CAChangeContent.ChangeContent0(null,List<string>, Func<string, string>)'
    /// </summary>
    /// <param name="a"></param>
    public static string AbsoluteFromCombinePath(string a)
    {
        string r = Path.GetFullPath(new Uri(a).LocalPath);
        return r;
    }


    public static string WrapWithQm(string item, bool? forceNotIncludeQm)
    {
        if (item.Contains(" ") && !forceNotIncludeQm.GetValueOrDefault())
        {
            return SH.WrapWithQm(item);
        }
        return item;
    }

    public const string dEndsWithReplaceInFile = "SubdomainHelperSimple.cs";

    public static List<string> FilterInRootAndInSubFolder(string rf, List<string> fs)
    {
        FS.WithEndSlash(ref rf);

        var c = rf.Length;

        List<string> subFolder = new List<string>(fs.Count);

        for (int i = fs.Count - 1; i >= 0; i--)
        {
            var item = fs[i];
            if (item.Substring(c).Contains(AllStrings.bs))
            {
                subFolder.Add(item);
                fs.RemoveAt(i);
            }
        }

        return subFolder;
    }

    public static void OnlyNames(List<string> subfolders)
    {
        for (int i = 0; i < subfolders.Count; i++)
        {
            subfolders[i] = FS.GetFileName(subfolders[i]);
        }
    }




    public static List<string> FilesWhichContainsAll(object sunamo, string masc, params string[] mustContains)
    {
        return FilesWhichContainsAll(sunamo, masc, mustContains);
    }

    public static
#if ASYNC
    async Task<List<string>>
#else
List<string>
#endif
    FilesWhichContainsAll(object sunamo, string masc, IList<string> mustContains)
    {
        var mcl = mustContains.Count();

        List<string> ls = new List<string>();
        IList<string> f = null;

        if (sunamo is IList<string>)
        {
            f = (IList<string>)sunamo;
        }
        else
        {
            f = FS.GetFiles(sunamo.ToString(), masc, true);
        }

        foreach (var item in f)
        {
            var c =
#if ASYNC
            await
#endif
            TF.ReadAllText(item);
            if (CA.ContainsAnyFromElement(c, mustContains).Count == mcl)
            {
                ls.Add(item);
            }
        }

        return ls;
    }



    public static string PathSpecialAndLevel(string basePath, string item, int v)
    {
        basePath = basePath.Trim(AllChars.bs);

        item = item.Trim(AllChars.bs);

        item = item.Replace(basePath, string.Empty);
        var pBasePath = SHSplit.Split(basePath, AllStrings.bs);
        var basePathC = pBasePath.Count;

        var p = SHSplit.Split(item, AllStrings.bs);
        int i = 0;
        for (; i < p.Count; i++)
        {
            if (p[i].StartsWith(AllStrings.lowbar))
            {
                pBasePath.Add(p[i]);
            }
            else
            {
                //i--;
                break;
            }
        }
        for (int y = 0; y < i; y++)
        {
            p.RemoveAt(0);
        }

        var h = p.Count - i + basePathC;
        var to = Math.Min(v, h);
        i = 0;
        for (; i < to; i++)
        {
            pBasePath.Add(p[i]);
        }

        return string.Join(AllStrings.bs, pBasePath);
    }

    public static string GetDirectoryNameIfIsFile(string f)
    {
        if (File.Exists(f))
        {
            return Path.GetDirectoryName(f);
        }
        return f;
    }

    public static string MaskFromExtensions(List<string> allExtensions)
    {
        CA.Prepend(AllStrings.asterisk, allExtensions);
        return string.Join(AllStrings.comma, allExtensions);
    }


    /// <summary>
    /// C:\Users\w\AppData\Roaming\sunamo\
    /// </summary>
    /// <param name="item2"></param>
    /// <param name="exts"></param>
    public static List<string> GetFilesOfExtensions(string item2, SearchOption so, params string[] exts)
    {
        List<string> vr = new List<string>();
        foreach (string item in exts)
        {
            vr.AddRange(FS.GetFiles(item2, AllStrings.asterisk + item, so));
        }
        return vr;
    }

    //public static string GetRelativePath(string relativeTo, string path)
    //{
    //    return SunamoExceptions.Path.GetRelativePath(relativeTo, path);
    //}

    //public static bool IsAbsolutePath(string path)
    //{
    //    return SunamoExceptions.FS.IsAbsolutePath(path);
    //}



    /// <summary>
    /// RenameNumberedSerieFiles - Rename files by linear names - 0,1,...,x
    /// </summary>
    /// <param name="d"></param>
    /// <param name="p"></param>
    /// <param name="startFrom"></param>
    /// <param name="ext"></param>
    public static void RenameNumberedSerieFiles(List<string> d, string p, int startFrom, string ext)
    {
        var masc = FS.MascFromExtension(ext);
        var f = FS.GetFiles(p, masc, SearchOption.TopDirectoryOnly);
        RenameNumberedSerieFiles(d, f, startFrom, ext);
    }

    /// <summary>
    /// A1 is new names of files without extension. Can use LinearHelper
    /// </summary>
    /// <param name="d"></param>
    /// <param name="p"></param>
    /// <param name="startFrom"></param>
    /// <param name="ext"></param>
    public static void RenameNumberedSerieFiles(List<string> d, List<string> f, int startFrom, string ext)
    {
        var p = FS.GetDirectoryName(f[0]);

        if (f.Count >= d.Count)
        {
            var fCountMinusONe = f.Count - 1;

            //var r = f.First();
            for (int i = startFrom; ; i++)
            {
                if (fCountMinusONe < i)
                {
                    break;
                }
                var r = f[i];
                var t = p + i + ext;
                if (f.Contains(t))
                {
                    //break;
                    continue;
                }
                else
                {
                    // AddSerie is useless coz file never will be exists
                    //FS.RenameFile(t, d[i - startFrom] + ext, FileMoveCollisionOption.AddSerie);
                    FS.RenameFile(r, t, FileMoveCollisionOption.AddSerie);
                }

            }
        }
    }

    /// <summary>
    /// Get path A2/name folder of file A1/name A1
    ///
    /// </summary>
    /// <param name="var"></param>
    /// <param name="zmenseno"></param>
    public static string PlaceInFolder(string var, string zmenseno)
    {
        //return Slozka.ci.PridejNadslozku(var, zmenseno);
        string nad = Path.GetDirectoryName(var);
        string naz = FS.GetFileName(nad);
        return Path.Combine(zmenseno, Path.Combine(naz, FS.GetFileName(var)));
    }
    public static FileInfo[] GetFileInfosOfExtensions(string item2, SearchOption so, params string[] exts)
    {
        List<FileInfo> vr = new List<FileInfo>();
        DirectoryInfo di = new DirectoryInfo(item2);
        foreach (string item in exts)
        {
            vr.AddRange(di.GetFiles(AllStrings.asterisk + item, so));
        }
        return vr.ToArray();
    }


    /// <summary>
    /// A1 MUST BE WITH EXTENSION
    /// A4 can be null if !A5
    /// In A1 will keep files which doesnt exists in A3
    /// A4 is files from A1 which wasnt founded in A2
    /// A7 is files
    /// </summary>
    /// <param name="filesFrom"></param>
    /// <param name="folderFrom"></param>
    /// <param name="folderTo"></param>
    /// <param name="wasntExistsInFrom"></param>
    /// <param name="mustExistsInTarget"></param>
    /// <param name="copy"></param>
    public static void CopyMoveFilesInList(List<string> filesFrom, string folderFrom, string folderTo, List<string> wasntExistsInFrom, bool mustExistsInTarget, bool copy, Dictionary<string, List<string>> files, bool overwrite = true)
    {
        FS.WithoutEndSlash(folderFrom);
        FS.WithoutEndSlash(folderTo);
        CA.RemoveStringsEmpty2(filesFrom);
        bool existsFileTo = false;
        for (int i = filesFrom.Count - 1; i >= 0; i--)
        {
            filesFrom[i] = filesFrom[i].Replace(folderFrom, string.Empty);
            var oldPath = folderFrom + filesFrom[i];
            if (files != null)
            {
                var oldPath2 = files[filesFrom[i]].FirstOrDefault();
                if (oldPath2 != null)
                {
                    oldPath = oldPath2.ToString();
                }
            }
#if DEBUG
            ///DebugLogger.DebugWriteLine("Taken: " + oldPath);
#endif
            var newPath = folderTo + filesFrom[i];
            if (!File.Exists(oldPath))
            {
                if (wasntExistsInFrom != null)
                {
                    wasntExistsInFrom.Add(filesFrom[i]);
                }
                filesFrom.RemoveAt(i);
                continue;
            }
            if (!File.Exists(newPath) && mustExistsInTarget)
            {
                continue;
            }
            else
            {
                existsFileTo = File.Exists(newPath);
                if ((existsFileTo && overwrite) || !existsFileTo)
                {
                    if (copy)
                    {
                        FS.CopyFile(oldPath, newPath, FileMoveCollisionOption.Overwrite);
                    }
                    else
                    {
                        FS.MoveFile(oldPath, newPath, FileMoveCollisionOption.Overwrite);
                    }
                }
                filesFrom.RemoveAt(i);
            }
        }
    }

    public static void CopyMoveFilesInListSimple(List<string> f, string basePathCjHtml1, string basePathCjHtml2, bool copy, bool overwrite = true)
    {
        List<string> wasntExistsInFrom = null;
        bool mustExistsInTarget = false;
        CopyMoveFilesInList(f, basePathCjHtml1, basePathCjHtml2, wasntExistsInFrom, mustExistsInTarget, copy, null, false);
    }

    public static void CreateInOtherLocationSameFolderStructure(string from, string to)
    {
        FS.WithEndSlash(from);
        FS.WithEndSlash(to);
        var folders = FS.GetFolders(from, SearchOption.AllDirectories);
        foreach (var item in folders)
        {
            string nf = item.Replace(from, to);
            FS.CreateFoldersPsysicallyUnlessThere(nf);
        }
    }

    /// <summary>
    /// A1 must be with extensions!
    /// </summary>
    /// <param name="files"></param>
    /// <param name="folderFrom"></param>
    /// <param name="folderTo"></param>
    public static void CopyMoveFromMultiLocationIntoOne(List<string> files, string folderFrom, string folderTo)
    {

        List<string> wasntExists = new List<string>();

        Dictionary<string, List<string>> files2 = new Dictionary<string, List<string>>();
        var getFiles = FS.GetFiles(folderFrom, "*.cs", SearchOption.AllDirectories, new GetFilesArgs { excludeFromLocationsCOntains = CA.ToListString("TestFiles") });
        foreach (var item in files)
        {
            files2.Add(item, getFiles.Where(d => FS.GetFileName(d) == item).ToList());
        }
        FS.CopyMoveFilesInList(files, folderFrom, folderTo, wasntExists, false, true, files2);
        ////DebugLogger.Instance.WriteList(wasntExists);
    }



    public static string StorageFilePath<StorageFolder, StorageFile>(StorageFile item, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            ac.fs.storageFilePath.Invoke(item);
        }
        return item.ToString();
    }

    public static List<StorageFile> GetFilesOfExtensionCaseInsensitiveRecursively<StorageFolder, StorageFile>(StorageFolder sf, string ext, AbstractCatalog<StorageFolder, StorageFile> ac)
    {

        if (ac != null)
        {
            return ac.fs.getFilesOfExtensionCaseInsensitiveRecursively.Invoke(sf, ext);
        }
        List<StorageFile> files = new List<StorageFile>();
        files = FS.GetFilesInterop<StorageFolder, StorageFile>(sf, AllStrings.asterisk, true, ac);
        for (int i = files.Count - 1; i >= 0; i--)
        {
            dynamic file = files[i];
            if (!file.ToLower().EndsWith(ext))
            {
                files.RemoveAt(i);
            }
        }
        return files;
    }
    public static List<StorageFile> GetFilesInterop<StorageFolder, StorageFile>(StorageFolder folder, string mask, bool recursive, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            return ac.fs.getFiles.Invoke(folder, mask, recursive);
        }
        // folder is StorageFolder
        var files = Directory.GetFiles(folder.ToString(), mask, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        return CAG.ToList<StorageFile>((dynamic)files);
    }

    public static Stream OpenStreamForReadAsync<StorageFolder, StorageFile>(StorageFile file, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            return ac.fs.openStreamForReadAsync.Invoke(file);
        }
        return FS.OpenStream(file.ToString());
    }

    private static Stream OpenStream(string v)
    {
        return new FileStream(v, FileMode.OpenOrCreate);
    }

    public static bool IsFoldersEquals<StorageFolder, StorageFile>(StorageFolder parent, StorageFolder path, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            return ac.fs.isFoldersEquals.Invoke(parent, path);
        }
        var f1 = parent.ToString();
        var f2 = path.ToString();
        return f1 == f2;
    }

    public static string GetFileName<StorageFolder, StorageFile>(StorageFile item, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            return ac.fs.getFileName.Invoke(item);
        }
        return FS.GetFileName(item.ToString());
    }

    /// <summary>
    /// A1 must be sunamo.Data.StorageFolder or uwp StorageFolder
    /// Return fixed string is here right
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="v"></param>
    public static StorageFile GetStorageFile<StorageFolder, StorageFile>(StorageFolder folder, string v, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        if (ac != null)
        {
            return ((dynamic)ac.fs.getStorageFile(folder, v)).Path;
        }
        return (dynamic)Path.Combine(folder.ToString(), v);
    }

    public static
#if ASYNC
    async Task
#else
void
#endif
    DeleteEmptyFiles(string folder, SearchOption so)
    {
        var files = FS.GetFiles(folder, FS.MascFromExtension(), so);
        foreach (var item in files)
        {
            var fs = FS.GetFileSize(item);
            if (fs == 0)
            {
                FS.TryDeleteFile(item);
            }
            else if (fs < 4)
            {
                if ((
#if ASYNC
                await
#endif
                TF.ReadAllText(item)).Trim() == string.Empty)
                {
                    FS.TryDeleteFile(item);
                }
            }

        }
    }

    static void ReplaceInAllFilesWorker(object o)
    {
        ReplaceInAllFilesArgs p = (ReplaceInAllFilesArgs)o;

        #region ReplaceInAllFilesArgsBase - Zkopírovat i do ReplaceInAllFilesWorker. Viz comment níže
        // musím to rozdělit na jednotlivé proměnné abych viděl co se používá a co ne. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
        var fasterMethodForReplacing = p.fasterMethodForReplacing;
        var files = p.files;
        var inDownloadedFolders = p.inDownloadedFolders;
        var inFoldersToDelete = p.inFoldersToDelete;
        var inGitFiles = p.inGitFiles;
        var isMultilineWithVariousIndent = p.isMultilineWithVariousIndent;
        var writeEveryReadedFileAsStatus = p.writeEveryReadedFileAsStatus;
        var writeEveryWrittenFileAsStatus = p.writeEveryWrittenFileAsStatus;
        #endregion

        #region ReplaceInAllFilesArgs
        var from = p.from;
        var to = p.to;
        var pairLinesInFromAndTo = p.pairLinesInFromAndTo;
        var replaceWithEmpty = p.replaceWithEmpty;
        var isNotReplaceInTemporaryFiles = p.isNotReplaceInTemporaryFiles;
        #endregion

        if (isMultilineWithVariousIndent)
        {
            from = SHReplace.ReplaceAllDoubleSpaceToSingle2(from, false);
            to = SHReplace.ReplaceAllDoubleSpaceToSingle2(to, false);
        }

        if (pairLinesInFromAndTo)
        {
            var from2 = SHSplit.Split(from, Environment.NewLine);
            var to2 = SHSplit.Split(to, Environment.NewLine);

            if (replaceWithEmpty)
            {
                to2.Clear();
                foreach (var item in from2)
                {
                    to2.Add(string.Empty);
                }
            }

            ThrowEx.DifferentCountInLists("from2", from2, "to2", to2);
            ReplaceInAllFiles(from2, to2, o as ReplaceInAllFilesArgsBase);
        }
        else
        {
            ReplaceInAllFiles(CA.ToListString(from), CA.ToListString(to), o as ReplaceInAllFilesArgsBase);
        }
    }


    public static void ReplaceInAllFiles(string from, string to, ReplaceInAllFilesArgsBase o)
    {
        ReplaceInAllFilesArgs r = new ReplaceInAllFilesArgs(o);
        r.from = from;
        r.to = to;

        ReplaceInAllFilesWorker(r);

        //Thread t = new Thread(new ParameterizedThreadStart(ReplaceInAllFilesWorker));
        //t.Start(r);
    }



    public static void ReplaceInAllFiles(string folder, string extension, List<string> replaceFrom, List<string> replaceTo, bool isMultilineWithVariousIndent)
    {
        var files = FS.GetFiles(folder, FS.MascFromExtension(extension), SearchOption.AllDirectories);
        ThrowEx.DifferentCountInLists("replaceFrom", replaceFrom, "replaceTo", replaceTo);
        Func<StringBuilder, IList<string>, IList<string>, StringBuilder> fasterMethodForReplacing = null;
        ReplaceInAllFiles(replaceFrom, replaceTo, new ReplaceInAllFilesArgsBase { files = files, isMultilineWithVariousIndent = isMultilineWithVariousIndent, fasterMethodForReplacing = fasterMethodForReplacing });
    }
    /// <summary>
    /// A4 - whether use s.Contains. A4 - SHReplace.ReplaceAll2
    /// </summary>
    /// <param name="replaceFrom"></param>
    /// <param name="replaceTo"></param>
    /// <param name="files"></param>
    /// <param name="dontReplaceAll"></param>
    public static
#if ASYNC
    async Task
#else
void
#endif
    ReplaceInAllFiles(IList<string> replaceFrom, IList<string> replaceTo, ReplaceInAllFilesArgsBase p)
    {
        #region ReplaceInAllFilesArgsBase - Zkopírovat i do ReplaceInAllFilesWorker. Viz comment níže
        // musím to rozdělit na jednotlivé proměnné abych viděl co se používá a co ne. Deconstructing object is not available in .net 48 https://www.faesel.com/blog/deconstruct-objects-in-csharp-like-in-javascript
        var fasterMethodForReplacing = p.fasterMethodForReplacing;
        var files = p.files;
        var inDownloadedFolders = p.inDownloadedFolders;
        var inFoldersToDelete = p.inFoldersToDelete;
        var inGitFiles = p.inGitFiles;
        var isMultilineWithVariousIndent = p.isMultilineWithVariousIndent;
        var writeEveryReadedFileAsStatus = p.writeEveryReadedFileAsStatus;
        var writeEveryWrittenFileAsStatus = p.writeEveryWrittenFileAsStatus;
        var dRemoveGitFiles = p.dRemoveGitFiles;
        #endregion

        if (!inGitFiles || !inFoldersToDelete || !inDownloadedFolders)
        {
            dRemoveGitFiles(files, inGitFiles, inDownloadedFolders, inFoldersToDelete);
        }

        foreach (var item in files)
        {
#if DEBUG
            if (item.EndsWith(dEndsWithReplaceInFile))
            {

            }
#endif

            if (!EncodingHelper.isBinary(item))
            {
                if (writeEveryReadedFileAsStatus)
                {
                    SunamoTemplateLogger.Instance.LoadedFromStorage(item);
                }

                // TF.ReadAllText is 20x faster than TF.ReadAllText
                var content =
#if ASYNC
                await
#endif
                TF.ReadAllText(item);
                var content2 = string.Empty;

                if (fasterMethodForReplacing == null)
                {
                    content2 = SHReplace.ReplaceAll3(replaceFrom, replaceTo, isMultilineWithVariousIndent, content);
                }
                else
                {
                    content2 = fasterMethodForReplacing.Invoke(new StringBuilder(content), replaceFrom, replaceTo).ToString();
                }

                if (content != content2)
                {
                    //PpkOnDrive ppk = PpkOnDrive.WroteOnDrive;
                    //ppk.Add(DateTime.Now.ToString() + " " + item);

                    TF.WriteAllText(item, content2);

                    if (writeEveryReadedFileAsStatus)
                    {
                        SunamoTemplateLogger.Instance.SavedToDrive(item);
                    }
                }
            }
            else
            {
                ThisApp.Warning(sess.i18n(XlfKeys.ContentOf) + " " + item + " couldn't be replaced - contains control chars.");
            }

        }
    }
    /// <summary>
    /// Jen kvuli starým aplikacím, at furt nenahrazuji.
    /// </summary>
    /// <param name="v"></param>
    public static string GetFileInStartupPath(string v)
    {
        return AppPaths.GetFileInStartupPath(v);
    }
    public static
#if ASYNC
    async Task
#else
void
#endif
    RemoveDiacriticInFileContents(string folder, string mask)
    {
        var files = FS.GetFiles(folder, mask, SearchOption.AllDirectories);
        foreach (string item in files)
        {
            string df2 =
#if ASYNC
            await
#endif
            TF.ReadAllText(item, Encoding.Default);
            if (true) //SH.ContainsDiacritic(df2))
            {

#if ASYNC
                await
#endif
                TF.WriteAllText(item, SH.TextWithoutDiacritic(df2));
                df2 = SHReplace.ReplaceOnce(df2, "\u010F\u00BB\u017C", "");

#if ASYNC
                await
#endif
                TF.WriteAllText(item, df2);
            }
        }
    }

    public static List<string> PathsOfStorageFiles<StorageFolder, StorageFile>(IList<StorageFile> files1, AbstractCatalog<StorageFolder, StorageFile> ac)
    {
        List<string> d = new List<string>(files1.Count());

        foreach (var item in files1)
        {
            d.Add(FS.StorageFilePath(item, ac));
        }

        return d;
    }

    public static string RemoveFile(string fullPathCsproj)
    {
        // Most effecient way to handle csproj and dir
        var ext = FS.GetExtension(fullPathCsproj);
        if (ext != string.Empty)
        {
            fullPathCsproj = FS.GetDirectoryName(fullPathCsproj);
        }
        var result = FS.WithoutEndSlash(fullPathCsproj);
        FS.FirstCharUpper(ref result);
        return result;
    }


    public static string MakeFromLastPartFile(string fullPath, string ext)
    {
        FS.WithoutEndSlash(ref fullPath);
        return fullPath + ext;
    }
    /// <summary>
    /// Remove all extensions, not only one
    /// </summary>
    /// <param name="item"></param>
    public static string GetFileNameWithoutExtensions(string item)
    {
        while (Path.HasExtension(item))
        {
            item = Path.GetFileNameWithoutExtension(item);
        }
        return item;
    }
    public static void CopyAs0KbFilesSubfolders
    (string pathDownload, string pathVideos0Kb)
    {
        FS.WithEndSlash(ref pathDownload);
        FS.WithEndSlash(ref pathVideos0Kb);
        var folders = FS.GetFolders(pathDownload);
        foreach (var item in folders)
        {
            CopyAs0KbFiles(item, item.Replace(pathDownload, pathVideos0Kb));
        }
    }
    public static void CopyAs0KbFiles(string pathDownload, string pathVideos0Kb)
    {
        FS.WithEndSlash(ref pathDownload);
        FS.WithEndSlash(ref pathVideos0Kb);
        var files = FS.GetFiles(pathDownload, true);
        foreach (var item in files)
        {
            var path = item.Replace(pathDownload, pathVideos0Kb);
            FS.CreateUpfoldersPsysicallyUnlessThere(path);
            TF.WriteAllText(path, string.Empty);
        }
    }

    public static string ShrinkLongPath(string actualFilePath)
    {
        // .NET 4.7.1
        // Originally - 265 chars, 254 also too long: E:\Documents\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\Facebook.cs
        // 4+265 - OK: @"\\?\D:\_NewlyRecovered\Visual Studio 2020\Projects\vs\Projects\Recovered data 03-23 12_11_44\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\Facebook.cs"
        // 216 - OK: D:\Recovered data 03-23 12_11_44012345678901234567890123456\Deep Scan result\Lost Partition1(NTFS)\Other lost files\c# projects - před odstraněním stejných souborů z duplicitních projektů\vs\Projects\merge-obří temp\temp1\temp\
        // for many API is different limits: https://stackoverflow.com/questions/265769/maximum-filename-length-in-ntfs-windows-xp-and-windows-vista
        // 237+11 - bad
        return Consts.UncLongPath + actualFilePath;
    }
    public static string CreateNewFolderPathWithEndingNextTo(string folder, string ending)
    {
        string pathToFolder = FS.GetDirectoryName(folder.TrimEnd(AllChars.bs)) + AllStrings.bs;
        string folderWithCaretFiles = pathToFolder + FS.GetFileName(folder.TrimEnd(AllChars.bs)) + ending;
        var result = folderWithCaretFiles;
        FS.FirstCharUpper(ref result);
        return result;
    }
    public static void CopyFilesOfExtensions(string folderFrom, string FolderTo, params string[] extensions)
    {
        folderFrom = FS.WithEndSlash(folderFrom);
        FolderTo = FS.WithEndSlash(FolderTo);
        var filesOfExtension = FS.FilesOfExtensions(folderFrom, extensions);
        foreach (var item in filesOfExtension)
        {
            foreach (var path in item.Value)
            {
                string newPath = path.Replace(folderFrom, FolderTo);
                FS.CreateUpfoldersPsysicallyUnlessThere(newPath);
                File.Copy(path, newPath);
            }
        }
    }
    /// <summary>
    /// Kromě jmen také zbavuje diakritiky složky.
    /// </summary>
    /// <param name="folder"></param>
    public static void RemoveDiacriticInFileSystemEntryNames(string folder)
    {
        List<string> folders = new List<string>(FS.GetFolders(folder, AllStrings.asterisk, SearchOption.AllDirectories));
        folders.Reverse();
        foreach (string item in folders)
        {
            string directory = FS.GetDirectoryName(item);
            string filename = FS.GetFileName(item);
            if (SH.ContainsDiacritic(filename))
            {
                filename = SH.TextWithoutDiacritic(filename);
                string newpath = Path.Combine(directory, filename);
                string realnewpath = newpath.TrimEnd(AllChars.bs);
                string realnewpathcopy = realnewpath;
                int i = 0;
                while (FS.ExistsDirectory(realnewpath))
                {
                    realnewpath = realnewpathcopy + i.ToString();
                    i++;
                }
                Directory.Move(item, realnewpath);
            }
        }
        var files = FS.GetFiles(folder, AllStrings.asterisk, SearchOption.AllDirectories);
        foreach (string item in files)
        {
            string directory = FS.GetDirectoryName(item);
            string filename = FS.GetFileName(item);
            if (SH.ContainsDiacritic(filename))
            {
                filename = SH.TextWithoutDiacritic(filename);
                string newpath = null;
                try
                {
                    newpath = Path.Combine(directory, filename);
                }
                catch (Exception ex)
                {
                    ThrowEx.DummyNotThrow(ex);
                    File.Delete(item);
                    continue;
                }
                string realNewPath = string.Copy(newpath);
                int vlozeno = 0;
                while (FS.ExistsFile(realNewPath))
                {
                    realNewPath = FS.InsertBetweenFileNameAndExtension(newpath, vlozeno.ToString());
                    vlozeno++;
                }
                File.Move(item, realNewPath);
            }
        }
    }
    public static string GetFilesSize(List<string> winrarFiles)
    {
        long size = 0;
        foreach (var item in winrarFiles)
        {
            size += FS.GetFileSize(item);
        }

        return GetSizeInAutoString(size);
    }
    public static string GetUpFolderWhichContainsExtension(string path, string fileExt)
    {
        while (FilesOfExtension(path, fileExt).Count == 0)
        {
            if (path.Length < 4)
            {
                return null;
            }
            path = FS.GetDirectoryName(path);
        }
        return path;
    }
    /// <summary>
    /// Non recursive
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="fileExt"></param>
    public static List<string> FilesOfExtension(string folder, string fileExt)
    {
        return FS.GetFiles(folder, "*." + fileExt, SearchOption.TopDirectoryOnly);
    }
    public static void TrimContentInFilesOfFolder(string slozka, string searchPattern, SearchOption searchOption)
    {
        var files = FS.GetFiles(slozka, searchPattern, searchOption);
        foreach (var item in files)
        {
            FileStream fs = new FileStream(item, FileMode.Open);
            StreamReader sr = new StreamReader(fs, true);
            string content = sr.ReadToEnd();
            Encoding enc = sr.CurrentEncoding;
            //sr.Close();
            sr.Dispose();
            sr = null;
            string contentTrim = content.Trim();
            TF.WriteAllText(item, contentTrim, enc);
            //}
        }
    }
    /// <summary>
    /// Náhrada za metodu ReplaceFileName se stejnými parametry
    /// </summary>
    /// <param name="oldPath"></param>
    /// <param name="what"></param>
    /// <param name="forWhat"></param>
    public static string ReplaceInFileName(string oldPath, string what, string forWhat)
    {
        string p2, fn;
        GetPathAndFileName(oldPath, out p2, out fn);
        string vr = p2 + AllStrings.bs + fn.Replace(what, forWhat);
        FS.FirstCharUpper(ref vr);
        return vr;
    }

    public static long GetSizeIn(long value, ComputerSizeUnits b, ComputerSizeUnits to)
    {
        if (b == to)
        {
            return value;
        }
        bool toLarger = ((byte)b) < ((byte)to);
        if (toLarger)
        {
            value = ConvertToSmallerComputerUnitSize(value, b, ComputerSizeUnits.B);
            if (to == ComputerSizeUnits.Auto)
            {
                ThrowEx.Custom("Byl specifikov\u00E1n v\u00FDstupn\u00ED ComputerSizeUnit, nem\u016F\u017Eu toto nastaven\u00ED zm\u011Bnit");
            }
            else if (to == ComputerSizeUnits.KB && b != ComputerSizeUnits.KB)
            {
                value /= 1024;
            }
            else if (to == ComputerSizeUnits.MB && b != ComputerSizeUnits.MB)
            {
                value /= 1024 * 1024;
            }
            else if (to == ComputerSizeUnits.GB && b != ComputerSizeUnits.GB)
            {
                value /= 1024 * 1024 * 1024;
            }
            else if (to == ComputerSizeUnits.TB && b != ComputerSizeUnits.TB)
            {
                value /= (1024L * 1024L * 1024L * 1024L);
            }
        }
        else
        {
            value = ConvertToSmallerComputerUnitSize(value, b, to);
        }
        return value;
    }
    /// <summary>
    /// Zjistí všechny složky rekurzivně z A1 a prvně maže samozřejmě ty, které mají více tokenů
    /// </summary>
    /// <param name="v"></param>
    public static void DeleteAllEmptyDirectories(string v, params string[] doNotDeleteWhichContains)
    {
        List<TWithInt<string>> dirs = FS.DirectoriesWithToken(v, AscDesc.Desc);
        foreach (var item in dirs)
        {
            if (FS.IsDirectoryEmpty(item.t, true, true))
            {
                if (doNotDeleteWhichContains.Length > 0)
                {
                    if (!CANew.ContainsAnyFromArray(item.t, doNotDeleteWhichContains))
                    {
                        FS.TryDeleteDirectory(item.t);
                    }
                }
                else
                {
                    FS.TryDeleteDirectory(item.t);
                }

            }
        }
    }
    public static List<TWithInt<string>> DirectoriesWithToken(string v, AscDesc sb)
    {
        var dirs = FS.GetFolders(v, AllStrings.asterisk, SearchOption.AllDirectories);
        List<TWithInt<string>> vr = new List<TWithInt<string>>();
        foreach (var item in dirs)
        {
            vr.Add(new TWithInt<string> { t = item, count = SH.OccurencesOfStringIn(item, AllStrings.bs) });
        }
        if (sb == AscDesc.Asc)
        {
            vr.Sort(new SunamoComparerICompare.TWithIntComparer.Asc<string>(new SunamoComparer.TWithIntSunamoComparer<string>()));
        }
        else if (sb == AscDesc.Desc)
        {
            vr.Sort(new SunamoComparerICompare.TWithIntComparer.Desc<string>(new SunamoComparer.TWithIntSunamoComparer<string>()));
        }
        return vr;
    }
    public static List<string> AllFilesInFolders(IList<string> folders, IList<string> exts, SearchOption so, GetFilesArgs a = null)
    {
        List<string> files = new List<string>();
        foreach (var item in folders)
        {
            foreach (var ext in exts)
            {
                files.AddRange(FS.GetFiles(item, FS.MascFromExtension(ext), so, a));
            }
        }
        return files;
    }
    /// <summary>
    /// A1 i A2 musí končit backslashem
    /// Může vyhodit výjimku takže je nutné to odchytávat ve volající metodě
    /// If destination folder exists, source folder without files keep
    /// Return message if success, or null
    /// A5 false
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    /// <param name="co"></param>
    public static string MoveDirectoryNoRecursive(string item, string nova, DirectoryMoveCollisionOption co, FileMoveCollisionOption co2)
    {
        string vr = null;
        if (FS.ExistsDirectory(nova))
        {
            if (co == DirectoryMoveCollisionOption.AddSerie)
            {
                int serie = 1;
                while (true)
                {
                    string newFn = nova + " (" + serie + AllStrings.rb;
                    if (!FS.ExistsDirectory(newFn))
                    {
                        vr = sess.i18n(XlfKeys.FolderHasBeenRenamedTo) + " " + FS.GetFileName(newFn);
                        nova = newFn;
                        break;
                    }
                    serie++;
                }
            }
            else if (co == DirectoryMoveCollisionOption.DiscardFrom)
            {
                Directory.Delete(item, true);
                return vr;
            }
            else if (co == DirectoryMoveCollisionOption.Overwrite)
            {
            }
        }
        var files = FS.GetFiles(item, AllStrings.asterisk, SearchOption.TopDirectoryOnly);
        FS.CreateFoldersPsysicallyUnlessThere(nova);
        foreach (var item2 in files)
        {
            string fileTo = nova + item2.Substring(item.Length);
            MoveFile(item2, fileTo, co2);
        }
        try
        {
            Directory.Move(item, nova);
        }
        catch (Exception ex)
        {
            ThrowEx.CannotMoveFolder(item, nova, ex);
        }
        if (FS.IsDirectoryEmpty(item, true, true))
        {
            FS.TryDeleteDirectory(item);
        }
        return vr;
    }
    private static bool IsDirectoryEmpty(string item, bool folders, bool files)
    {
        int fse = 0;
        if (folders)
        {
            fse += FS.GetFolders(item, AllStrings.asterisk, SearchOption.TopDirectoryOnly).Count;
        }
        if (files)
        {
            fse += FS.GetFiles(item, AllStrings.asterisk, SearchOption.TopDirectoryOnly).Count;
        }
        return fse == 0;
    }
    /// <summary>
    /// Vyhazuje výjimky, takže musíš volat v try-catch bloku
    /// A2 is root of target folder
    /// </summary>
    /// <param name="p"></param>
    /// <param name="to"></param>
    public static void MoveAllRecursivelyAndThenDirectory(string p, string to, FileMoveCollisionOption co)
    {
        MoveAllFilesRecursively(p, to, co, null);
        var dirs = FS.GetFolders(p, AllStrings.asterisk, SearchOption.AllDirectories);
        for (int i = dirs.Count - 1; i >= 0; i--)
        {
            FS.TryDeleteDirectory(dirs[i]);

        }
        FS.TryDeleteDirectory(p);
    }
    public static void MoveAllFilesRecursively(string p, string to, FileMoveCollisionOption co, string contains = null)
    {
        CopyMoveAllFilesRecursively(p, to, co, true, contains, SearchOption.AllDirectories);
    }
    /// <summary>
    /// Unit tests = OK
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteFilesWithSameContentBytes(List<string> files)
    {
        DeleteFilesWithSameContentWorking<List<byte>, byte>(files, TF.ReadAllBytesSync);
    }
    /// <summary>
    /// Unit tests = OK
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteDuplicatedImages(List<string> files)
    {
        ThrowEx.Custom(sess.i18n(XlfKeys.OnlyForTestFilesForAnotherApps) + ". ");
    }

    /// <summary>
    /// zde to zatím nechám jako sync
    /// ¨Func má jen Invoke, nemohl bych užívat výhody asyncu
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="ColType"></typeparam>
    /// <param name="files"></param>
    /// <param name="readFunc"></param>
    public static void DeleteFilesWithSameContentWorking<T, ColType>(List<string> files, Func<string, T> readFunc)
    {
        Dictionary<string, T> dictionary = new Dictionary<string, T>(files.Count);
        foreach (var item in files)
        {
            dictionary.Add(item, readFunc.Invoke(item));
        }
        Dictionary<T, List<string>> sameContent = DictionaryHelper.GroupByValues<string, T, ColType>(dictionary);
        foreach (var item in sameContent)
        {
            if (item.Value.Count > 1)
            {
                item.Value.RemoveAt(0);
                item.Value.ForEach(d => File.Delete(d));
            }
        }
    }
    /// <summary>
    /// Working fine, verified by Unit tests
    /// </summary>
    /// <param name="files"></param>
    public static void DeleteFilesWithSameContent(List<string> files)
    {
        DeleteFilesWithSameContentWorking<string, object>(files, TF.ReadAllTextSync);
    }

    /// <summary>
    /// Normally: 11,12,1,2,...
    /// This: 1,2,...,11,12
    ///
    /// non direct edit
    ///  working with full paths or just filenames
    /// </summary>
    /// <param name="l"></param>
    public static List<string> OrderByNaturalNumberSerie(List<string> l)
    {
        List<Tuple<string, int, string>> filenames = new List<Tuple<string, int, string>>();
        List<string> dontHaveNumbersOnBeginning = new List<string>();
        string path = null;
        for (int i = l.Count - 1; i >= 0; i--)
        {
            var backup = l[i];
            var p = SHSplit.SplitToPartsFromEnd(l[i], 2, new Char[] { AllChars.bs });
            if (p.Count == 1)
            {
                path = string.Empty;
            }
            else
            {
                path = p[0];
                l[i] = p[1];
            }
            var fn = l[i];
            var (sh, fnNew) = NH.NumberIntUntilWontReachOtherChar(fn);
            fn = fnNew;
            if (sh == int.MaxValue)
            {
                dontHaveNumbersOnBeginning.Add(backup);
            }
            else
            {
                filenames.Add(new Tuple<string, int, string>(path, sh, fn));
            }
        }
        var sorted = filenames.OrderBy(d => d.Item2);
        List<string> result = new List<string>(l.Count);
        foreach (var item in sorted)
        {
            result.Add(Path.Combine(item.Item1, item.Item2 + item.Item3));
        }
        result.AddRange(dontHaveNumbersOnBeginning);
        return result;
    }
    public static Dictionary<string, List<string>> SortPathsByFileName(List<string> allCsFilesInFolder, bool onlyOneExtension)
    {
        Dictionary<string, List<string>> vr = new Dictionary<string, List<string>>();
        foreach (var item in allCsFilesInFolder)
        {
            string fn = null;
            if (onlyOneExtension)
            {
                fn = Path.GetFileNameWithoutExtension(item);
            }
            else
            {
                fn = FS.GetFileName(item);
            }
            DictionaryHelper.AddOrCreate<string, string>(vr, fn, item);
        }
        return vr;
    }

    public static void DeleteAllRecursively(string p, bool rootDirectoryToo = false)
    {
        if (FS.ExistsDirectory(p))
        {
            var files = FS.GetFiles(p, AllStrings.asterisk, SearchOption.AllDirectories);
            foreach (var item in files)
            {
                FS.TryDeleteFile(item);
            }
            var dirs = FS.GetFolders(p, AllStrings.asterisk, SearchOption.AllDirectories);
            for (int i = dirs.Count - 1; i >= 0; i--)
            {
                FS.TryDeleteDirectory(dirs[i]);
            }
            if (rootDirectoryToo)
            {
                FS.TryDeleteDirectory(p);
            }
            // Commented due to NI
            //FS.DeleteFoldersWhichNotContains(BasePathsHelper.vs + @"", "bin", CA.ToListString( "node_modules"));
        }
    }

    public static void DeleteFoldersWhichNotContains(string v, string folder, IList<string> v2)
    {


        //var f = FS.GetFolders(v, folder, SearchOption.AllDirectories);
        //for (int i = f.Count - 1; i >= 0; i--)
        //{
        //    if (CA.ReturnWhichContainsIndexes( f[i], v2).Count != 0)
        //    {
        //        f.RemoveAt(i);
        //    }
        //}
        //ClipboardHelper.SetLines(f);
        //foreach (var item in f)
        //{
        //    //FS.DeleteF
        //}
    }

    /// <summary>
    /// Vyhazuje výjimky, takže musíš volat v try-catch bloku
    /// </summary>
    /// <param name="p"></param>
    public static void DeleteAllRecursivelyAndThenDirectory(string p)
    {
        DeleteAllRecursively(p, true);
    }

    public static List<string> OnlyExtensions(List<string> cesta)
    {
        List<string> vr = new List<string>(cesta.Count);
        CASE.InitFillWith(vr, cesta.Count);
        for (int i = 0; i < vr.Count; i++)
        {
            vr[i] = FS.GetExtension(cesta[i]);
        }
        return vr;
    }
    /// <summary>
    /// Both filenames and extension convert to lowercase
    /// Filename is without extension
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    /// <param name="searchOption"></param>
    public static Dictionary<string, List<string>> GetDictionaryByExtension(string folder, string mask, SearchOption searchOption)
    {
        Dictionary<string, List<string>> extDict = new Dictionary<string, List<string>>();
        foreach (var item in FS.GetFiles(folder, mask, searchOption))
        {
            string ext = FS.GetExtension(item);
            string fn = FS.GetFileNameWithoutExtensionLower(item);

            if (fn == string.Empty)
            {
                fn = ext;
                ext = "";
            }

            DictionaryHelper.AddOrCreate<string, string>(extDict, ext, fn);
        }
        return extDict;
    }
    public static List<string> OnlyExtensionsToLower(List<string> cesta, GetExtensionArgs a = null)
    {
        if (a == null)
        {
            a = new GetExtensionArgs();
        }

        a.returnOriginalCase = false;

        List<string> vr = new List<string>(cesta.Count);
        CASE.InitFillWith(vr, cesta.Count);
        for (int i = 0; i < vr.Count; i++)
        {
            vr[i] = FS.GetExtension(cesta[i], a).ToLower();
        }
        return vr;
    }
    public static List<string> OnlyExtensionsToLowerWithPath(List<string> cesta)
    {
        List<string> vr = new List<string>(cesta.Count);
        CASE.InitFillWith(vr, cesta.Count);
        for (int i = 0; i < vr.Count; i++)
        {

            vr[i] = FS.OnlyExtensionToLowerWithPath(cesta[i]);
        }
        return vr;
    }

    public static string OnlyExtensionToLowerWithPath(string d)
    {
        string path, fn, ext;
        FS.GetPathAndFileName(d, out path, out fn, out ext);
        var result = path + fn + ext.ToLower();
        return result;
    }

    public static Dictionary<TypeOfExtension, List<string>> AllExtensionsInFolderByCategory(List<string> files, GetExtensionArgs gea = null)
    {
        AllExtensionsHelper.Initialize(true);

        var exts = AllExtensionsInFolders(files, gea);

        Dictionary<TypeOfExtension, List<string>> dict = new Dictionary<TypeOfExtension, List<string>>();

        foreach (var item in exts)
        {
            var type = AllExtensionsHelper.FindTypeWithDot(item);
            DictionaryHelper.AddOrCreate(dict, type, item);
        }

        return dict;
        //return TextOutputGeneratorStatic.DictionaryWithCount(dict);
    }


    public static List<string> AllExtensionsInFolders(SearchOption so, params string[] folders)
    {
        ThrowEx.NoPassedFolders(folders);

        List<string> filesFull = AllFilesInFolders(CA.ToListMoreString(folders), CA.ToListString("*"), so);

        return AllExtensionsInFolders(filesFull);
    }

    /// <summary>
    /// files as .bowerrc return whole
    /// </summary>
    /// <param name="so"></param>
    /// <param name="folders"></param>
    public static List<string> AllExtensionsInFolders(List<string> filesFull, GetExtensionArgs gea = null)
    {
        List<string> vr = new List<string>();


#if DEBUG

        //var dx = filesFull.IndexOf(".babelrc");
#endif
        var files = new List<string>(OnlyExtensionsToLower(filesFull, gea));

#if DEBUG
        //var dxs = CA.IndexesWithValue(files, Consts.se);

        //List<string> c = CA.GetIndexes(filesFull, dxs);

        //ClipboardHelper.SetLines(c);

        //var dx2 = files.IndexOf(".babelrc");
#endif
        foreach (var item in files)
        {
            if (!vr.Contains(item))
            {
                vr.Add(item);
            }
        }
        return vr;
    }

    public static string replaceIncorrectFor = string.Empty;

    public static string ExpandEnvironmentVariables(EnvironmentVariables environmentVariable)
    {
        return Environment.ExpandEnvironmentVariables(SH.WrapWith(environmentVariable.ToString(), AllStrings.percnt, false));
    }
    /// <summary>
    /// Pokud by byla cesta zakončená backslashem, vrátila by metoda FS.GetFileName prázdný řetězec.
    /// </summary>
    /// <param name="s"></param>
    public static string GetFileNameWithoutExtensionLower(string s)
    {
        return GetFileNameWithoutExtension(s).ToLower();
    }
    public static string AddUpfoldersToRelativePath(int i, string file, char delimiter)
    {
        var jumpUp = AllStrings.dd + delimiter;
        return SHJoin.JoinTimes(i, jumpUp) + file;
    }
    /// <summary>
    /// Keys returns with normalized ext
    /// In case zero files of ext wont be included in dict
    /// </summary>
    /// <param name="folderFrom"></param>
    /// <param name="extensions"></param>
    public static Dictionary<string, List<string>> FilesOfExtensions(string folderFrom, params string[] extensions)
    {
        var dict = new Dictionary<string, List<string>>();
        foreach (var item in extensions)
        {
            string ext = FS.NormalizeExtension(item);
            var files = FS.GetFiles(folderFrom, AllStrings.asterisk + ext, SearchOption.AllDirectories);
            if (files.Count != 0)
            {
                dict.Add(ext, files);
            }
        }
        return dict;
    }
    /// <summary>
    /// convert to lowercase and remove first dot - to už asi neplatí. Use NormalizeExtension2 for that
    /// </summary>
    /// <param name="item"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string NormalizeExtension(string item)
    {
        return AllStrings.dot + item.TrimStart(AllChars.dot);
    }



    public static string GetNormalizedExtension(string filename)
    {
        return NormalizeExtension(filename);
    }
    public static long ModifiedinUnix(string dsi)
    {
        return (long)(File.GetLastWriteTimeUtc(dsi).Subtract(DTConstants.UnixFsStart)).TotalSeconds;
    }
    public static void ReplaceDiacriticRecursive(string folder, bool dirs, bool files, DirectoryMoveCollisionOption fo, FileMoveCollisionOption co)
    {
        if (dirs)
        {
            List<TWithInt<string>> dires = FS.DirectoriesWithToken(folder, AscDesc.Desc);
            foreach (var item in dires)
            {
                var dirPath = FS.WithoutEndSlash(item.t);
                string dirName = FS.GetFileName(dirPath);
                if (SH.ContainsDiacritic(dirName))
                {
                    string dirNameWithoutDiac = SH.TextWithoutDiacritic(dirName);
                    FS.RenameDirectory(item.t, dirNameWithoutDiac, fo, co);
                }
            }
        }
        if (files)
        {
            var files2 = FS.GetFiles(folder, AllStrings.asterisk, SearchOption.AllDirectories);
            foreach (var item in files2)
            {
                string filePath = item;
                string fileName = FS.GetFileName(filePath);
                if (SH.ContainsDiacritic(fileName))
                {
                    string dirNameWithoutDiac = SH.TextWithoutDiacritic(fileName);
                    FS.RenameFile(item, dirNameWithoutDiac, co);
                }
            }
        }
    }
    /// <summary>
    /// A1,2 = with ext
    /// Physically rename file, this method is different from ChangeFilename in FileMoveCollisionOption A3 which can control advanced collision solution
    /// </summary>
    /// <param name="oldFn"></param>
    /// <param name="newFnWithoutPath"></param>
    /// <param name="co"></param>
    public static void RenameFile(string oldFn, string newFnWithoutPath, FileMoveCollisionOption co)
    {
        var to = FS.ChangeFilename(oldFn, newFnWithoutPath, false);
        FS.MoveFile(oldFn, to, co);
    }
    /// <summary>
    /// Může výhodit výjimku, proto je nutné používat v try-catch bloku
    /// Vrátí řetězec se zprávou kterou vypsat nebo null
    /// </summary>
    /// <param name="path"></param>
    /// <param name="newname"></param>
    public static string RenameDirectory(string path, string newname, DirectoryMoveCollisionOption co, FileMoveCollisionOption fo)
    {
        string vr = null;
        path = FS.WithoutEndSlash(path);
        string cesta = FS.GetDirectoryName(path);
        string nova = Path.Combine(cesta, newname);
        vr = MoveDirectoryNoRecursive(path, nova, co, fo);
        return vr;
    }
    public static List<string> FilesOfExtensionsArray(string folder, List<string> extension)
    {
        List<string> foundedFiles = new List<string>();
        FS.NormalizeExtensions(extension);
        var files = Directory.EnumerateFiles(folder, FS.MascFromExtension(), SearchOption.AllDirectories);
        foreach (var item in files)
        {
            string ext = FS.GetNormalizedExtension(item);
            if (extension.Contains(ext))
            {
                foundedFiles.Add(ext);
            }
        }
        return foundedFiles;
    }
    /// <summary>
    /// convert to lowercase and remove first dot
    /// </summary>
    /// <param name="extension"></param>
    private static void NormalizeExtensions(List<string> extension)
    {
        for (int i = 0; i < extension.Count; i++)
        {
            extension[i] = NormalizeExtension(extension[i]);
        }
    }
    /// <summary>
    /// A1 může obsahovat celou cestu, vrátí jen název sobuoru bez připony a příponu
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    /// <param name="ext"></param>
    public static void GetFileNameWithoutExtensionAndExtension(string fn, out string file, out string ext)
    {
        file = Path.GetFileNameWithoutExtension(fn);
        ext = FS.GetExtension(file);
    }

    public static void SaveStream(string path, Stream s)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
        {
            FS.CopyStream(s, fs);
            fs.Flush();
        }
    }
    public static List<string> OnlyNamesWithoutExtensionCopy(List<string> p2)
    {
        List<string> p = new List<string>(p2.Count);
        CASE.InitFillWith(p, p2.Count);
        for (int i = 0; i < p2.Count; i++)
        {
            p[i] = Path.GetFileNameWithoutExtension(p2[i]);
        }
        return p;
    }
    public static List<string> OnlyNamesWithoutExtension(string appendToStart, List<string> fullPaths)
    {
        List<string> ds = new List<string>(fullPaths.Count);
        CASE.InitFillWith(ds, fullPaths.Count);
        for (int i = 0; i < fullPaths.Count; i++)
        {
            ds[i] = appendToStart + Path.GetFileNameWithoutExtension(fullPaths[i]);
        }
        return ds;
    }

    public static string Postfix(string aPath, string s)
    {
        var result = aPath.TrimEnd(AllChars.bs) + s;
        FS.WithEndSlash(ref result);
        return result;
    }
}
