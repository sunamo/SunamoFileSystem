namespace SunamoFileSystem._sunamo.SunamoCollectionsGeneric;

internal class CAG
{
    /// <summary>
    ///     Return what exists in both
    ///     Modify both A1 and A2 - keep only which is only in one
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    internal static List<T> CompareList<T>(List<T> c1, List<T> c2) where T : IEquatable<T>
    {
        var existsInBoth = new List<T>();

        var dex = -1;

        for (var i = c2.Count - 1; i >= 0; i--)
        {
            var item = c2[i];
            dex = c1.IndexOf(item);

            if (dex != -1)
            {
                existsInBoth.Add(item);
                c2.RemoveAt(i);
                c1.RemoveAt(dex);
            }
        }

        for (var i = c1.Count - 1; i >= 0; i--)
        {
            var item = c1[i];
            dex = c2.IndexOf(item);

            if (dex != -1)
            {
                existsInBoth.Add(item);
                c1.RemoveAt(i);
                c2.RemoveAt(dex);
            }
        }

        return existsInBoth;
    }
}