using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunamoFileSystem._sunamo.SunamoCollectionsGeneric;
internal class CAG
{
    /// <summary>
    /// Return what exists in both
    /// Modify both A1 and A2 - keep only which is only in one
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    internal static List<T> CompareList<T>(List<T> c1, List<T> c2) where T : IEquatable<T>
    {
        List<T> existsInBoth = new List<T>();

        int dex = -1;

        for (int i = c2.Count - 1; i >= 0; i--)
        {
            T item = c2[i];
            dex = c1.IndexOf(item);

            if (dex != -1)
            {
                existsInBoth.Add(item);
                c2.RemoveAt(i);
                c1.RemoveAt(dex);
            }
        }

        for (int i = c1.Count - 1; i >= 0; i--)
        {
            T item = c1[i];
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
