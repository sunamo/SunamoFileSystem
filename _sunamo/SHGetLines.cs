namespace SunamoFileSystem;
internal class SHGetLines
{
    internal static List<string> GetLines(string v)
    {
        return vv.Split(new string[] { v.Contains("\r\n") ? "\r\n" : "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
