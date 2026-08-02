namespace SunamoFileSystem._sunamo.SunamoRegex;

internal class Wildcard : Regex
{
    internal Wildcard(string pattern)
        : base(WildcardToRegex(pattern))
    {
    }

    internal Wildcard(string pattern, RegexOptions options)
        : base(WildcardToRegex(pattern), options)
    {
    }

    internal static string WildcardToRegex(string pattern)
        => "^" + Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
}
