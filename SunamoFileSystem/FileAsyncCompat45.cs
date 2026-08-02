namespace SunamoFileSystem;

// net48 nema async File API (pridano az v .NET Core) - synchronni fallback pro .Net48.csproj wrapper.
internal static class FileAsyncCompat45
{
    public static Task<string> ReadAllTextAsync(string path)
    {
#if NET48
        return Task.FromResult(File.ReadAllText(path));
#else
        return File.ReadAllTextAsync(path);
#endif
    }

    public static Task<string> ReadAllTextAsync(string path, Encoding encoding)
    {
#if NET48
        return Task.FromResult(File.ReadAllText(path, encoding));
#else
        return File.ReadAllTextAsync(path, encoding);
#endif
    }

    public static Task WriteAllTextAsync(string path, string content)
    {
#if NET48
        File.WriteAllText(path, content);
        return Task.CompletedTask;
#else
        return File.WriteAllTextAsync(path, content);
#endif
    }

    public static Task WriteAllLinesAsync(string path, IEnumerable<string> lines)
    {
#if NET48
        File.WriteAllLines(path, lines);
        return Task.CompletedTask;
#else
        return File.WriteAllLinesAsync(path, lines);
#endif
    }
}
