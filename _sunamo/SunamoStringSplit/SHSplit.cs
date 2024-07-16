namespace SunamoFileSystem._sunamo.SunamoStringSplit;

internal class SHSplit
{
    #region SplitToPartsFromEnd
    internal static List<string> SplitToPartsFromEnd(string what, int parts, params char[] deli)
    {
        List<char> chs = null;
        List<bool> bw = null;
        List<int> delimitersIndexes = null;
        SHSplit.SplitCustom(what, out chs, out bw, out delimitersIndexes, deli);

        List<string> vr = new List<string>(parts);
        StringBuilder sb = new StringBuilder();
        for (int i = chs.Count - 1; i >= 0; i--)
        {
            if (!bw[i])
            {
                while (i != 0 && !bw[i - 1])
                {
                    i--;
                }
                string d = sb.ToString();
                sb.Clear();
                if (d != "")
                {
                    vr.Add(d);
                }
            }
            else
            {
                sb.Insert(0, chs[i]);
                //sb.Append(chs[i]);
            }
        }
        string d2 = sb.ToString();
        sb.Clear();
        if (d2 != "")
        {
            vr.Add(d2);
        }
        List<string> v = new List<string>(parts);
        for (int i = 0; i < vr.Count; i++)
        {
            if (v.Count != parts)
            {
                v.Insert(0, vr[i]);
            }
            else
            {
                string ds = what[delimitersIndexes[i - 1]].ToString();
                v[0] = vr[i] + ds + v[0];
            }
        }
        return v;
    }

    internal static void SplitCustom(string what, out List<char> chs, out List<bool> bs, out List<int> delimitersIndexes, params char[] deli)
    {
        chs = new List<char>(what.Length);
        bs = new List<bool>(what.Length);
        delimitersIndexes = new List<int>(what.Length / 6);
        for (int i = 0; i < what.Length; i++)
        {
            bool isNotDeli = true;
            var ch = what[i];
            foreach (var item in deli)
            {
                if (item == ch)
                {
                    delimitersIndexes.Add(i);
                    isNotDeli = false;
                    break;
                }
            }
            chs.Add(ch);
            bs.Add(isNotDeli);
        }
        delimitersIndexes.Reverse();
    }
    #endregion

    internal static List<string> SplitMore(string item, params string[] space)
    {
        return item.Split(space, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    internal static List<string> SplitCharMore(string v1, params char[] v2)
    {
        return v1.Split(v2).ToList();
    }
}
