
namespace SunamoFileSystem;
using SunamoFileSystem._sunamo;


/// <summary>
///     FSXlf - postfixy jsou píčovina. volám v tom metody stejné třídy. Můžu nahradit FS. v SunExc ale musel bych to
///     zkopírovat zpět. to nese riziko že jsem přidal novou metodu kterou bych překopírováním ztratil. Krom toho to nedrží
///     konvenci. V názvu souboru to nechám ať vidím na první dobrou co je co.
/// </summary>
public partial class FS
{






    public static
#if ASYNC
    async Task<string>
#else
string
#endif
    ReadAllText(string path)
    {
        return
#if ASYNC
        await
#endif
        TFSE.ReadAllText(path);
    }







    /// <summary>
    ///     Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    ///     if have more extension, remove just one
    /// </summary>
    /// <param name="s"></param>
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile s,
    //AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        string ss = s.ToString();
    //        string vr = Path.GetFileNameWithoutExtension(ss.TrimEnd(SunamoValues.AllCharsSE.bs));
    //        string ext = Path.GetExtension(ss).TrimStart(SunamoValues.AllCharsSE.dot);

    //        if (!ext.All(d => AllChars.vsZnakyWithoutSpecial.Contains(d)) /*SH.ContainsOnly(ext, AllCharsSE.vsZnakyWithoutSpecial)*/)
    //        {
    //            if (ext != string.Empty)
    //            {
    //                return (dynamic)vr + AllStringsSE.dot + ext;
    //            }
    //        }

    //        return (dynamic)vr;
    //    }

    //    ThrowNotImplementedUwp();
    //    return s;
    //}





    public static void ThrowNotImplementedUwp()
    {
        throw new Exception("Not implemented in UWP");
    }

    public static string GetFileNameWithoutExtension(string s)
    {
        return Path.GetFileNameWithoutExtension(s);
        //return GetFileNameWithoutExtension<string, string>(s, null);
    }


    //public static bool IsFileHasKnownExtension(string relativeTo)
    //{
    //    var ext = Path.GetExtension(relativeTo);
    //    ext = FS.NormalizeExtension2(ext);


    //    return AllExtensionsHelperWithoutDot.allExtensionsWithoutDot.ContainsKey(ext);
    //}

    //public static string PathWithoutExtension(string path)
    //{
    //    string path2, file, ext;
    //    GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
    //    return Combine(path2, file);
    //}

    ///// <summary>
    ///// Vrátí cestu a název souboru bez ext a ext
    ///// All returned is normal case
    ///// </summary>
    ///// <param name="fn"></param>
    ///// <param name="path"></param>
    ///// <param name="file"></param>
    ///// <param name="ext"></param>
    //public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    //{
    //    path = Path.GetDirectoryName(fn) + AllCharsSE.bs;
    //    file = GetFileNameWithoutExtension(fn);
    //    ext = Path.GetExtension(fn);
    //}

    ///// <summary>
    ///// Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    ///// if have more extension, remove just one
    ///// </summary>
    ///// <param name="s"></param>
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile s, AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ss = s.ToString();
    //        var vr = Path.GetFileNameWithoutExtension(ss.TrimEnd(AllCharsSE.bs));
    //        var ext = Path.GetExtension(ss).TrimStart(AllCharsSE.dot);

    //        if (!SH.ContainsOnly(ext, RandomHelper.vsZnakyWithoutSpecial))
    //        {
    //            if (ext != string.Empty)
    //            {
    //                return (dynamic)vr + AllStringsSE.dot + ext;
    //            }
    //        }
    //        return (dynamic)vr;
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //        return s;
    //    }
    //}

    //public static string GetFileNameWithoutExtension(string s)
    //{
    //    return GetFileNameWithoutExtension<string, string>(s, null);
    //}

    //private static string Combine(params string[] path2)
    //{
    //    return Path.Combine(path2);
    //}

    #region MakeUncLongPath



    #endregion


    #region from FSShared64.cs


    //        /// <summary>
    //        /// Convert to UNC path
    //        /// </summary>
    //        /// <param name="item"></param>
    //        public static bool ExistsDirectoryWorker(string item, bool _falseIfContainsNoFile = false)
    //        {
    //            // Not working, flags from GeoCachingTool wasnt transfered to standard
    //#if NETFX_CORE
    //        ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exc.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
    //#endif
    //#if WINDOWS_UWP
    //        ThrowEx.IsNotAvailableInUwpWindowsStore(type, Exc.CallingMethod(), "  "+-sess.i18n(XlfKeys.UseMethodsInFSApps));
    //#endif

    //            if (item == Consts.UncLongPath || item == string.Empty)
    //            {
    //                return false;
    //            }

    //            var item2 = MakeUncLongPath(item);

    //            // Directory.Exists if pass SE or only start of Unc return false
    //            var result = Directory.Exists(item2);
    //            if (_falseIfContainsNoFile)
    //            {
    //                if (result)
    //                {
    //                    var f = GetFilesSimple(item, "*", SearchOption.AllDirectories).Count;
    //                    result = f > 0;
    //                }
    //            }
    //            return result;
    //        }

    //public static bool IsCountOfFilesMoreThan(string bpMb, int v)
    //{
    //    return false;
    //}

    #region FirstCharUpper
    public static string FirstCharUpper(ref string result)
    {
        if (IsWindowsPathFormat(result))
        {
            result = SH.FirstCharUpper(result);
        }

        return result;
    }

    public static string FirstCharUpper(string nazevPP, bool only = false)
    {
        if (nazevPP != null)
        {
            string sb = nazevPP.Substring(1);
            if (only)
            {
                sb = sb.ToLower();
            }

            return nazevPP[0].ToString().ToUpper() + sb;
        }

        return null;
    }
    #endregion



    /// <summary>
    ///     Usage: Exceptions.FileWasntFoundInDirectory
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>
    public static void GetPathAndFileName(string fn, out string path, out string file)
    {
        path = WithEndSlash(GetDirectoryName(fn));
        file = GetFileName(fn);
    }

    public static string GetFileName(string fn)
    {
        return Path.GetFileName(fn);
    }










    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeExtension2(string item)
    {
        return item.ToLower().TrimStart(SunamoValues.AllCharsSE.dot);
    }

    public static List<string> GetFiles(string v1, string v2, SearchOption topDirectoryOnly)
    {
        return Directory.GetFiles(v1, v2, topDirectoryOnly).ToList();
    }

    //private static void ThrowNotImplementedUwp()
    //{
    //    throw new Exception("Not implemented in UWP");
    //}

    //        private static Type type = typeof(FS);

    //
    //        #region

    //        #endregion
    //
    //
    //

    //
    //        private static long ConvertToSmallerComputerUnitSize(long value, ComputerSizeUnits b, ComputerSizeUnits to)
    //        {
    //            return ConvertToSmallerComputerUnitSize(value, b, to);
    //        }
    //
    //

    //
    //        public static string GetSizeInAutoString(long value, ComputerSizeUnits b)
    //        {
    //            if (b != ComputerSizeUnits.B)
    //            {
    //                // Získám hodnotu v bytech
    //                value = ConvertToSmallerComputerUnitSize(value, b, ComputerSizeUnits.B);
    //            }
    //
    //
    //            if (value < 1024)
    //            {
    //                return value + " B";
    //            }
    //
    //            double previous = value;
    //            value /= 1024;
    //
    //            if (value < 1)
    //            {
    //                return previous + " B";
    //            }
    //
    //            previous = value;
    //            value /= 1024;
    //
    //            if (value < 1)
    //            {
    //                return previous + " KB";
    //            }
    //
    //            previous = value;
    //            value /= 1024;
    //            if (value < 1)
    //            {
    //                return previous + " MB";
    //            }
    //
    //            previous = value;
    //            value /= 1024;
    //
    //            if (value < 1)
    //            {
    //                return previous + " GB";
    //            }
    //
    //            return value + " TB";
    //        }
    //
    //
    //        public static List<string> GetFiles(string path, string v, SearchOption topDirectoryOnly)
    //        {
    //            return GetFilesMoreMasc(path, v, topDirectoryOnly).ToList();
    //        }
    //
    //        public static List<string> GetFilesMoreMasc(string path, string v, SearchOption topDirectoryOnly)
    //        {
    //            var c = AllStringsSE.comma;
    //            var sc = AllStringsSE.sc;
    //            List<string> result = new List<string>();
    //            List<string> masks = new List<string>();
    //
    //            if (v.Contains(c))
    //            {
    //                masks.AddRange(SHSplit.Split(v, c));
    //            }
    //            else if (v.Contains(sc))
    //            {
    //                masks.AddRange(SHSplit.Split(v, sc));
    //            }
    //            else
    //            {
    //                masks.Add(v);
    //            }
    //
    //            foreach (var item in masks)
    //            {
    //                result.AddRange(GetFiles(path, item, topDirectoryOnly));
    //            }
    //
    //            return result;
    //        }
    //

    //
    //        public static void CreateUpfoldersPsysicallyUnlessThere(string nad)
    //        {
    //            CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(nad));
    //        }
    //
    //        /// <summary>
    //        /// Create all upfolders of A1 with, if they dont exist
    //        /// </summary>
    //        /// <param name="nad"></param>
    //        public static void CreateFoldersPsysicallyUnlessThere(string nad)
    //        {
    //            ThrowEx.IsNullOrEmpty("nad", nad);
    //            ThrowEx.IsNotWindowsPathFormat("nad", nad);
    //
    //            FS.MakeUncLongPath(ref nad);
    //            if (Directory.Exists(nad))
    //            {
    //                return;
    //            }
    //            else
    //            {
    //                List<string> slozkyKVytvoreni = new List<string>();
    //                slozkyKVytvoreni.Add(nad);
    //
    //                while (true)
    //                {
    //                    nad = Path.GetDirectoryName(nad);
    //
    //                    // TODO: Tady to nefunguje pro UWP/UAP apps protoze nemaji pristup k celemu disku. Zjistit co to je UWP/UAP/... a jak v nem ziskat/overit jakoukoliv slozku na disku
    //                    if (Directory.Exists(nad))
    //                    {
    //                        break;
    //                    }
    //
    //                    string kopia = nad;
    //                    slozkyKVytvoreni.Add(kopia);
    //                }
    //
    //                slozkyKVytvoreni.Reverse();
    //                foreach (string item in slozkyKVytvoreni)
    //                {
    //                    string folder = FS.MakeUncLongPath(item);
    //                    if (!Directory.Exists(folder))
    //                    {
    //                        Directory.CreateDirectory(folder);
    //                    }
    //                }
    //            }
    //        }
    //
    //        public static string ReadAllText(string filename)
    //        {
    //            return File.ReadAllTextAsync(filename);
    //        }
    //
    //        #region MyRegion
    //
    //
    //        private static void MakeUncLongPath(ref string nad)
    //        {
    //
    //        }
    //

    //        #endregion
    //
    //        #region Just in SunamoExceptions
    //        public static void CreateFileIfDoesntExists(string path)
    //        {
    //            if (!File.Exists(path))
    //            {
    //                File.WriteAllTextAsync(path, string.Empty);
    //            }
    //        }
    //        #endregion


    //        /// <summary>
    //        /// Dont check for size
    //        /// Into A2 is good put true - when storage was fulled, all new files will be written with zero size. But then failing because HtmlNode as null - empty string as input
    //        /// But when file is big, like backup of DB, its better false.. Then will be avoid reading whole file to determining their size and totally blocking HW resources on VPS
    //        ///
    //        /// A2 must be false otherwise read file twice
    //        ///
    //        /// Change falseIfSizeZeroOrEmpty = false. Its extremely resource intensive
    //        /// </summary>
    //        /// <param name="selectedFile"></param>
    //        public static bool ExistsFile(string selectedFile, bool falseIfSizeZeroOrEmpty = false)
    //        {
    //            SH.FirstCharUpper(ref selectedFile);
    //            //ThrowEx.FirstLetterIsNotUpper(selectedFile);

    //            if (filesWhichSurelyExists.Contains(selectedFile))
    //            {
    //                return true;
    //            }

    //#if DEBUG

    //#endif

    //            if (selectedFile == Consts.UncLongPath || selectedFile == string.Empty)
    //            {
    //                return false;
    //            }

    //            FS.MakeUncLongPath(ref selectedFile);

    //            var exists = File.Exists(selectedFile);

    //            if (falseIfSizeZeroOrEmpty)
    //            {
    //                if (!exists)
    //                {

    //                    return false;
    //                }
    //                else
    //                {
    //                    var ext = Path.GetExtension(selectedFile).ToLower();
    //                    // Musím to kontrolovat jen když je to tmp, logicky
    //                    if (ext == AllExtensions.tmp)
    //                    {
    //                        return false;
    //                    }
    //                    else
    //                    {
    //                        var c = string.Empty;
    //                        try
    //                        {
    //                            c = File.ReadAllTextAsync(selectedFile);
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            if (ex.Message.StartsWith("The process cannot access the file"))
    //                            {
    //                                return true;
    //                            }

    //                        }

    //                        if (c == string.Empty)
    //                        {
    //                            // Měl jsem tu chybu že ač exists bylo true, TF.ReadAllText selhalo protože soubor neexistoval.
    //                            // Vyřešil jsem to kontrolou přípony, snad
    //                            return false;
    //                        }
    //                    }
    //                }
    //            }
    //            return exists;
    //        }


    ///// <summary>
    ///// Cant return with end slash becuase is working also with files
    ///// Use this than Path.Combine which if argument starts with backslash ignore all arguments before this
    ///// </summary>
    ///// <param name="upFolderName"></param>
    ///// <param name="dirNameDecoded"></param>
    //public static string Combine(params string[] s)
    //{
    //    return CombineWorker(true, s);
    //}

    ///// <summary>
    ///// Cant return with end slash becuase is working also with files
    ///// </summary>
    ///// <param name="FirstCharUpper"></param>
    ///// <param name="s"></param>
    //private static string CombineWorker(bool FirstCharUpper, params string[] s)
    //{
    //    s = CA.TrimStart(AllCharsSE.bs, s).ToArray();
    //    var result = Path.Combine(s);
    //    if (FirstCharUpper)
    //    {
    //        result = SH.FirstCharUpper(ref result);
    //    }
    //    else
    //    {
    //        result = SH.FirstCharUpper(ref result);
    //    }
    //    // Cant return with end slash becuase is working also with files
    //    //FSND.WithEndSlash(ref result);
    //    return result;
    //}

    //public static string GetFileNameWithoutExtension(string s)
    //{
    //    return GetFileNameWithoutExtension<string, string>(s, null);
    //}

    /// <summary>
    /// Pokud by byla cesta zakončená backslashem, vrátila by metoda Path.GetFileName prázdný řetězec.
    /// if have more extension, remove just one
    /// </summary>
    /// <param name = "s" ></ param >
    //public static StorageFile GetFileNameWithoutExtension<StorageFolder, StorageFile>(StorageFile s, AbstractCatalogBase<StorageFolder, StorageFile> ac)
    //{
    //    if (ac == null)
    //    {
    //        var ss = s.ToString();
    //        var vr = Path.GetFileNameWithoutExtension(ss.TrimEnd(AllCharsSE.bs));
    //        var ext = Path.GetExtension(ss).TrimStart(AllCharsSE.dot);

    //        if (!SH.ContainsOnly(ext, RandomHelper.vsZnakyWithoutSpecial))
    //        {
    //            if (ext != string.Empty)
    //            {
    //                return (dynamic)vr + AllStringsSE.dot + ext;
    //            }
    //        }
    //        return (dynamic)vr;
    //    }
    //    else
    //    {
    //        ThrowNotImplementedUwp();
    //        return s;
    //    }
    //}


    //        #region  from FSShared.cs
    //        public static void DeleteFile(string item)
    //        {
    //            File.Delete(item);
    //        }
    //
    //
    /// <summary>
    /// Usage: FileWasntFoundInDirectory
    /// Vrátí cestu a název souboru s ext
    /// </summary>
    /// <param name="fn"></param>
    /// <param name="path"></param>
    /// <param name="file"></param>

    //
    //        /// <summary>
    //        /// Vrátí cestu a název souboru bez ext a ext
    //        /// All returned is normal case
    //        /// </summary>
    //        /// <param name="fn"></param>
    //        /// <param name="path"></param>
    //        /// <param name="file"></param>
    //        /// <param name="ext"></param>
    //        public static void GetPathAndFileNameWithoutExtension(string fn, out string path, out string file, out string ext)
    //        {
    //            path = Path.GetDirectoryName(fn) + AllCharsSE.bs;
    //            file = Path.GetFileNameWithoutExtension(fn);
    //            ext = Path.GetExtension(fn);
    //        }
    //
    //        public static string PathWithoutExtension(string path)
    //        {
    //            string path2, file, ext;
    //            FS.GetPathAndFileNameWithoutExtension(path, out path2, out file, out ext);
    //            return Path.Combine(path2, file);
    //        }
    //
    //        public static string GetFullPath(string vr)
    //        {
    //            var result = Path.GetFullPath(vr);
    //            SH.FirstCharUpper(ref result);
    //            return result;
    //        }
    //
    //        public static void FileToDirectory(ref string dir)
    //        {
    //            if (!dir.EndsWith(AllStringsSE.bs))
    //            {
    //                dir = Path.GetDirectoryName(dir);
    //            }
    //        }
    //
    //        #endregion

    #endregion
}
