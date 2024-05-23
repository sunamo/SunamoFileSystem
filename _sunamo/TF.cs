namespace SunamoFileSystem;
public class TF
{
    public static List<byte> ReadAllBytesSync(string arg)
    {
        return File.ReadAllBytes(arg).ToList();
    }
}
