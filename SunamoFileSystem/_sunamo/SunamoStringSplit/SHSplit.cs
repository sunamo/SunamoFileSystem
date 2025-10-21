// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy

namespace SunamoFileSystem._sunamo.SunamoStringSplit;

internal class SHSplit
{
    internal static List<string> Split(string item, params string[] space)
    {
        return item.Split(space, StringSplitOptions.RemoveEmptyEntries).ToList();
    }


    #region SplitToPartsFromEnd

    internal static List<string> SplitToPartsFromEnd(string what, int parts, params char[] deli)
    {
        List<char> chs = null;
        List<bool> bw = null;
        List<int> delimitersIndexes = null;
        SplitCustom(what, out chs, out bw, out delimitersIndexes, deli);

        var vr = new List<string>(parts);
        var stringBuilder = new StringBuilder();
        for (var i = chs.Count - 1; i >= 0; i--)
            if (!bw[i])
            {
                while (i != 0 && !bw[i - 1]) i--;
                var data = stringBuilder.ToString();
                stringBuilder.Clear();
                if (data != "") vr.Add(data);
            }
            else
            {
                stringBuilder.Insert(0, chs[i]);
                //stringBuilder.Append(chs[i]);
            }

        var d2 = stringBuilder.ToString();
        stringBuilder.Clear();
        if (d2 != "") vr.Add(d2);
        var value = new List<string>(parts);
        for (var i = 0; i < vr.Count; i++)
            if (value.Count != parts)
            {
                value.Insert(0, vr[i]);
            }
            else
            {
                var ds = what[delimitersIndexes[i - 1]].ToString();
                value[0] = vr[i] + ds + value[0];
            }

        return value;
    }

    internal static void SplitCustom(string what, out List<char> chs, out List<bool> bs,
        out List<int> delimitersIndexes, params char[] deli)
    {
        chs = new List<char>(what.Length);
        bs = new List<bool>(what.Length);
        delimitersIndexes = new List<int>(what.Length / 6);
        for (var i = 0; i < what.Length; i++)
        {
            var isNotDeli = true;
            var ch = what[i];
            foreach (var item in deli)
                if (item == ch)
                {
                    delimitersIndexes.Add(i);
                    isNotDeli = false;
                    break;
                }

            chs.Add(ch);
            bs.Add(isNotDeli);
        }

        delimitersIndexes.Reverse();
    }

    #endregion
}