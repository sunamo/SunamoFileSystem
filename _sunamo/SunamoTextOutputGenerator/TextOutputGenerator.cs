namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;


/// <summary>
/// In Comparing
/// </summary>
internal class TextOutputGenerator //: ITextOutputGenerator
{
    private readonly static string s_znakNadpisu = AllStrings.asterisk;
    // při převádění na nugety jsem to změnil na ITextBuilder sb = TextBuilder.Create();
    // ale asi to byla blbost, teď mám v _sunamo Create() která je ale null místo abych použil ctor
    // takže vracím nazpět.
    //internal TextBuilder sb = new TextBuilder();
    internal StringBuilder sb = new StringBuilder();
    //internal string prependEveryNoWhite
    //{
    //    get => sb.prependEveryNoWhite;
    //    set => sb.prependEveryNoWhite = value;
    //}
    internal static TextOutputGenerator Create()
    {
        return new TextOutputGenerator();
    }
    #region Static texts
    internal void EndRunTime()
    {
        sb.AppendLine("AppWillBeTerminated");
    }
    /// <summary>
    /// Pouze vypíše "Az budete mit vstupní data, spusťte program znovu."
    /// </summary>
    internal void NoData()
    {
        sb.AppendLine("NoData");
    }
    #endregion
    #region Templates
    /// <summary>
    /// Napíše nadpis A1 do konzole
    /// </summary>
    /// <param name="text"></param>
    internal void StartRunTime(string text)
    {
        int delkaTextu = text.Length;
        string hvezdicky = "";
        hvezdicky = new string(s_znakNadpisu[0], delkaTextu);
        //hvezdicky.PadLeft(delkaTextu, znakNadpisu[0]);
        sb.AppendLine(hvezdicky);
        sb.AppendLine(text);
        sb.AppendLine(hvezdicky);
    }
    internal void CountEvery<T>(IList<KeyValuePair<T, int>> eq)
    {
        foreach (var item in eq)
        {
            AppendLine(item.Key + AllStrings.cs + item.Value + "x");
        }
    }
    #endregion
    #region AppendLine
    internal void AppendLine()
    {
        AppendLine(string.Empty);
    }
    internal void AppendLine(StringBuilder text)
    {
        sb.AppendLine(text.ToString());
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Append(string text)
    {
        sb.Append(text);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AppendLine(string text)
    {
        sb.AppendLine(text);
    }
    internal void AppendLineFormat(string text, params string[] p)
    {
        sb.AppendLine();
        AppendLine(string.Format(text, p));
    }
    internal void AppendFormat(string text, params string[] p)
    {
        AppendLine(string.Format(text, p));
    }
    #endregion
    #region Other adding methods
    internal void Header(string v)
    {
        sb.AppendLine();
        AppendLine(v);
        sb.AppendLine();
    }
    internal void SingleCharLine(char paddingChar, int v)
    {
        sb.AppendLine(string.Empty.PadLeft(v, paddingChar));
    }
    #endregion
    public override string ToString()
    {
        var ts = sb.ToString();
        return ts;
    }
    #region List
    internal void ListObject(IList files1)
    {
        List<string> l = new List<string>();
        foreach (var item in files1)
        {
            l.Add(item.ToString());
        }
        List(l);
    }
    internal void ListSB(StringBuilder onlyStart, string v)
    {
        Header(v);
        AppendLine(onlyStart);
    }
    /// <summary>
    /// If you have StringBuilder, use Paragraph()
    /// </summary>
    /// <param name="files1"></param>
    internal void List(IList<string> files1)
    {
        List<string>(files1);
    }
    internal void List<Value>(IList<Value> files1, string deli = "\r\n", string whenNoEntries = Consts.stringEmpty)
    {
        if (files1.Count() == 0)
        {
            sb.AppendLine(whenNoEntries);
        }
        else
        {
            foreach (var item in files1)
            {
                Append(item.ToString() + deli);
            }
            //sb.AppendLine();
        }
    }
    /// <summary>
    ///  must be where Header : IEnumerable<char> (like is string)
    /// </summary>
    /// <typeparam name="Header"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="files1"></param>
    /// <param name="header"></param>
    internal void List<Header, Value>(IList<Value> files1, Header header) where Header : IEnumerable<char>
    {
        List<Header, Value>(files1, header, new TextOutputGeneratorArgs { headerWrappedEmptyLines = true, insertCount = false });
    }
    internal void List(IList<string> files1, string header)
    {
        List<string, string>(files1, header, new TextOutputGeneratorArgs { headerWrappedEmptyLines = true, insertCount = false });
    }
    internal void ListString(string list, string header)
    {
        Header(header);
        AppendLine(list);
        sb.AppendLine();
    }
    /// <summary>
    /// Use DictionaryHelper.CategoryParser
    /// </summary>
    /// <typeparam name="Header"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="files1"></param>
    /// <param name="header"></param>
    /// <param name="a"></param>
    internal void List<Header, Value>(IList<Value> files1, Header header, TextOutputGeneratorArgs a) where Header : IEnumerable<char>
    {
        if (a.insertCount)
        {
            //throw new Exception("later");
            //header = (Header)((IList<char>)CA.JoinIList<char>(header, " (" + files1.Count() + AllStrings.rb));
        }
        if (a.headerWrappedEmptyLines)
        {
            sb.AppendLine();
        }
        sb.AppendLine(header + AllStrings.colon);
        if (a.headerWrappedEmptyLines)
        {
            sb.AppendLine();
        }
        List(files1, a.delimiter, a.whenNoEntries);
    }
    #endregion
    #region Paragraph
    internal void Paragraph(StringBuilder wrongNumberOfParts, string header)
    {
        string text = wrongNumberOfParts.ToString().Trim();
        Paragraph(text, header);
    }
    /// <summary>
    /// For ordinary text use Append*
    /// </summary>
    /// <param name="text"></param>
    /// <param name="header"></param>
    internal void Paragraph(string text, string header)
    {
        if (text != string.Empty)
        {
            sb.AppendLine(header + AllStrings.colon);
            sb.AppendLine(text);
            sb.AppendLine();
        }
    }
    #endregion
    internal void Undo()
    {
        ThrowEx.NotImplementedMethod();
        //sb.Undo();
    }
    #region Dictionary
    internal void Dictionary(Dictionary<string, int> charEntity, string delimiter)
    {
        foreach (var item in charEntity)
        {
            sb.AppendLine(item.Key + delimiter + item.Value);
        }
    }
    internal void DictionaryKeyValuePair<T1, T2>(string header, IOrderedEnumerable<KeyValuePair<T1, T2>> ordered)
    {
        Header(header);
        foreach (var item in ordered)
        {
            sb.AppendLine(item.Key + AllStrings.space + item.Value);
        }
    }
    internal void IGrouping(IEnumerable<IGrouping<string, string>> g)
    {
        var d = IGroupingToDictionary(g);
        Dictionary(d);
    }
    Dictionary<string, List<string>> IGroupingToDictionary(IEnumerable<IGrouping<string, string>> g)
    {
        Dictionary<string, List<string>> l = new Dictionary<string, List<string>>();
        foreach (var item in g)
        {
            l.Add(item.Key, item.ToList());
        }
        return l;
    }
    internal void Dictionary(Dictionary<string, List<string>> ls)
    {
        foreach (var item in ls)
        {
            List(item.Value, item.Key);
        }
    }
    internal void Dictionary<Header, Value>(Dictionary<Header, List<Value>> ls, bool onlyCountInValue = false) where Header : IEnumerable<char>
    {
        if (onlyCountInValue)
        {
            List<string> d = new List<string>(ls.Count);
            foreach (var item in ls)
            {
                d.Add(item.Key + AllStrings.space + item.Value.Count());
            }
            List(d);
        }
        else
        {
            foreach (var item in ls)
            {
                List<Header, Value>(item.Value, item.Key);
            }
        }
    }
    /// <summary>
    /// vše na 1 řádku, oddělí |
    /// </summary>
    /// <param name="v"></param>
    internal void Dictionary(Dictionary<string, string> v)
    {
        foreach (var item in v)
        {
            sb.AppendLine(string.Join("|", item.Key, item.Value));
        }
    }
    /// <summary>
    /// vše na 1 řádku, oddělí |
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="d"></param>
    /// <param name="deli"></param>
    internal void Dictionary<T1, T2>(Dictionary<T1, T2> d, string deli = AllStrings.verbar)
    {
        //StringBuilder sb = new StringBuilder();
        foreach (var item in d)
        {
            if (deli != AllStrings.verbar)
            {
                Header(item.Key.ToString());
                // vrací mi to na jednom řádku jak key tak všechny value oddělené |.
                sb.AppendLine(string.Join(deli, new string[] { item.Value.ToString() }));
                sb.AppendLine();
            }
            else
            {
                // vrací mi to na jednom řádku jak key tak všechny value oddělené |.
                sb.AppendLine(string.Join(deli, new string[] { item.Key.ToString(), item.Value.ToString() }));// SF.PrepareToSerializationExplicitString(new List<string>(), deli));
            }
        }
    }
    internal void PairBullet(string key, string v)
    {
        sb.AppendLine(key + ": " + v);
    }
    internal string DictionaryBothToStringToSingleLine<Key, Value>(Dictionary<Key, Value> sorted, bool putValueAsFirst, string delimiter = AllStrings.space)
    {
        foreach (var item in sorted)
        {
            string first, second = null;
            if (putValueAsFirst)
            {
                first = item.Value.ToString();
                second = item.Key.ToString();
            }
            else
            {
                first = item.Key.ToString();
                second = item.Value.ToString();
            }
            sb.AppendLine(first + delimiter + second);
        }
        return sb.ToString();
    }
    #endregion
}
