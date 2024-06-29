namespace SunamoFileSystem;
internal class TF
{
    internal static List<byte> ReadAllBytesSync(string arg)
    {
        return File.ReadAllBytes(arg).ToList();
    }
}
