
using SunamoFileSystem._sunamo;
using SunamoTextOutputGenerator;

namespace SunamoFileSystem;
public partial class FS
{
    #region Making problem in translate
    /// <summary>
    /// Delete whole folder A1. If fail, only "1" subdir
    /// Use in RepairBlogContent but sample data is NA
    /// Deleting old folder hiearchy and create new
    /// </summary>
    /// <param name="repairedBlogPostsFolder"></param>
    public static int DeleteSerieDirectoryOrCreateNew(string repairedBlogPostsFolder)
    {
        int resultSerie = 1;
        var folders = Directory.GetDirectories(repairedBlogPostsFolder);

        bool deleted = true;
        // 0 or 1
        if (folders.Length < 2)
        {
            try
            {
                Directory.Delete(repairedBlogPostsFolder, true);
            }
            catch (Exception ex)
            {
                ThrowEx.FolderCannotBeDeleted(repairedBlogPostsFolder, ex);
                deleted = false;
            }
        }

        string withEndFlash = FSND.WithEndSlash(repairedBlogPostsFolder);

        if (!deleted)
        {
            // confuse me, dir can exists
            // Here seems to be OK on 8-7-19 (unit test)
            Directory.CreateDirectory(withEndFlash + @"1" + "\\");
        }
        else
        {
            // When deleting will be successful, create new dir
            TextOutputGenerator generator = new TextOutputGenerator();
            generator.sb.Append(withEndFlash);
            generator.sb.CanUndo = true;
            for (; resultSerie < int.MaxValue; resultSerie++)
            {
                generator.sb.Append(resultSerie);
                string newDirectory = generator.ToString();
                if (!Directory.Exists(newDirectory))
                {
                    Directory.CreateDirectory(newDirectory);
                    break;
                }
                generator.Undo();
            }
        }

        return resultSerie;
    }

    public static SearchOption ToSearchOption(bool? recursive)
    {
        return recursive.GetValueOrDefault() ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
    }



    public static void WriteAllText(string path, string content)
    {
        File.WriteAllTextAsync(path, content);
    }

    public static bool IsAllInSameFolder(List<string> c)
    {
        if (c.Count > 0)
        {
            var f = Path.GetDirectoryName(c[0]);

            for (int i = 1; i < c.Count; i++)
            {
                if (Path.GetDirectoryName(c[i]) != f)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static void CreateFileWithTemplateContent(string folder, string files, string ext, string templateFromContent)
    {
        var lines = SHGetLines.GetLines(files);

        foreach (var item in lines)
        {
            var path = Path.Combine(folder, item + ext);
            if (!File.Exists(path))
            {
                File.WriteAllTextAsync(path, templateFromContent);
            }
        }
    }

    public static bool ContainsInvalidFileNameChars(string arg)
    {
        foreach (var item in invalidFileNameStrings)
        {
            if (arg.Contains(item))
            {
                return true;
            }
        }

        return false;
    }

    public static void NumberByDateModified(string folder, string masc, SearchOption so)
    {
        var files = GetFiles(folder, masc, so, new GetFilesArgs { byDateOfLastModifiedAsc = true });
        int i = 1;
        foreach (var item in files)
        {
            FS.RenameFile(item, i + Path.GetExtension(item), FileMoveCollisionOption.DontManipulate);
            i++;
        }
    }




    #endregion
}
