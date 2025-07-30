namespace SunamoFileSystem._sunamo.SunamoFileIO;

internal class TF
{
    internal static List<byte> ReadAllBytesSync(string arg)
    {
        return File.ReadAllBytes(arg).ToList();
    }
}