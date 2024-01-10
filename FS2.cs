namespace SunamoFileSystem;


public partial class FS
{
    static FS()
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
        s_invalidFileNameCharsString = String.Join("", invalidFileNameChars);
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
}
