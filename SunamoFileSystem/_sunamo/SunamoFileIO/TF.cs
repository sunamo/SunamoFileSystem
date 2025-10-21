// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoFileIO;

internal class TF
{
    internal static List<byte> ReadAllBytesSync(string arg)
    {
        return File.ReadAllBytes(arg).ToList();
    }
}