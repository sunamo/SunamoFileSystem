namespace SunamoFileSystem._sunamo.SunamoFileIO;

internal class TF
{
    internal static List<byte> ReadAllBytesSync(string filePath)
    {
        return File.ReadAllBytes(filePath).ToList();
    }
}