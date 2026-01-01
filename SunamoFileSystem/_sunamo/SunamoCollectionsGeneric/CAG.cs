namespace SunamoFileSystem._sunamo.SunamoCollectionsGeneric;

/// <summary>
/// Collection Array Generic helper methods
/// </summary>
internal class CAG
{
    /// <summary>
    /// Returns elements that exist in both collections
    /// Modifies both collections - keeps only elements that are unique to each
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="collection1">First collection</param>
    /// <param name="collection2">Second collection</param>
    /// <returns>List of elements that exist in both collections</returns>
    internal static List<T> CompareList<T>(List<T> collection1, List<T> collection2) where T : IEquatable<T>
    {
        var existsInBoth = new List<T>();

        var index = -1;

        for (var i = collection2.Count - 1; i >= 0; i--)
        {
            var item = collection2[i];
            index = collection1.IndexOf(item);

            if (index != -1)
            {
                existsInBoth.Add(item);
                collection2.RemoveAt(i);
                collection1.RemoveAt(index);
            }
        }

        for (var i = collection1.Count - 1; i >= 0; i--)
        {
            var item = collection1[i];
            index = collection2.IndexOf(item);

            if (index != -1)
            {
                existsInBoth.Add(item);
                collection1.RemoveAt(i);
                collection2.RemoveAt(index);
            }
        }

        return existsInBoth;
    }
}