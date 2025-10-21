// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoCollectionsGeneric;

internal class CAG
{
    /// <summary>
    ///     Return what exists in both
    ///     Modify both A1 and A2 - keep only which is only in one
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
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