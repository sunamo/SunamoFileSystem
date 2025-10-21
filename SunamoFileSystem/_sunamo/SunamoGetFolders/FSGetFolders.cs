// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoGetFolders;

internal class FSGetFolders
{




    /// <summary>
    ///     A3 must be GetFilesArgs, not GetFoldersEveryFolder because is calling from GetFiles
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="list"></param>
    /// <param name="e"></param>
    private static void GetFoldersEveryFolder(string folder, List<string> list, GetFilesArgsFS e = null)
    {
        List<string> folders = null;

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
            ThrowEx.Custom(ex);
            // Not throw exception, it's probably Access denied  on Documents and Settings etc
            //throw new Exception("GetFoldersEveryFolder with path: " + folder, ex);
        }

        if (folders != null)
        {
            CA.RemoveWhichContainsList(folders, e.excludeFromLocationsCOntains, e.wildcard);
            list.AddRange(folders);
            for (var i = 0; i < folders.Count; i++) GetFoldersEveryFolder(folders[i], list, e);
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
            foreach (var item in folders) GetFoldersEveryFolder(item, mask, list);
        }
        catch (Exception ex)
        {
            ThrowEx.Custom(ex);
            // Not throw exception, it's probably Access denied  on Documents and Settings etc
            //throw new Exception("GetFoldersEveryFolder with path: " + folder, ex);
        }
    }

    /// <summary>
    ///     It's always recursive
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="mask"></param>
    internal static List<string> GetFoldersEveryFolder(string folder, GetFilesArgsFS e = null)
    {
        if (e == null) e = new GetFilesArgsFS();
        var list = new List<string>();
        // zde progress bar nedává smysl. načítám to rekurzivně, tedy nevím na začátku kolik těch složek bude
        //IProgressBarHelper pbh = null;
        //if (a.progressBarHelper != null)
        //{
        //    pbh = a.progressBarHelper.CreateInstance(a.pb, files.Count, this);
        //}
        GetFoldersEveryFolder(folder, list, e);
        if (e._trimA1AndLeadingBs)
            //list = CAChangeContent.ChangeContent0(null, list, d => d = d.Replace(folder, "").TrimStart('\\'));
            for (var i = 0; i < list.Count; i++)
                list[i] = list[i].Replace(folder, "").TrimStart('\\');
        if (e.excludeFromLocationsCOntains != null)
            // I want to find files recursively
            foreach (var item in e.excludeFromLocationsCOntains)
                CA.RemoveWhichContains(list, item, e.wildcard, Regex.IsMatch);
        return list;
    }

}