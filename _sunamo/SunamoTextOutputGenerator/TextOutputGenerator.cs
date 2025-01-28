namespace SunamoFileSystem._sunamo.SunamoTextOutputGenerator;

/// <summary>
///     In Comparing
/// </summary>
internal class TextOutputGenerator //: ITextOutputGenerator
{
    private static readonly string s_znakNadpisu = "*";

    // při převádění na nugety jsem to změnil na ITextBuilder sb = TextBuilder.Create();
    // ale asi to byla blbost, teď mám v _sunamo Create() která je ale null místo abych použil ctor
    // takže vracím nazpět.
    //internal TextBuilder sb = new TextBuilder();
    internal StringBuilder sb = new();

    //internal string prependEveryNoWhite
    //{
    //    get => sb.prependEveryNoWhite;
    //    set => sb.prependEveryNoWhite = value;
    //}

    public override string ToString()
    {
        var ts = sb.ToString();
        return ts;
    }

    internal void Undo()
    {
        ThrowEx.NotImplementedMethod();
        //sb.Undo();
    }

    #region Static texts



    #endregion

    #region Templates



    #endregion

    #region AppendLine


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



    #endregion

    #region Other adding methods

    internal void Header(string v)
    {
        sb.AppendLine();
        AppendLine(v);
        sb.AppendLine();
    }


    #endregion

    #region List



    /// <summary>
    ///     If you have StringBuilder, use Paragraph()
    /// </summary>
    /// <param name="files1"></param>
    internal void List(IList<string> files1)
    {
        List<string>(files1);
    }

    internal void List<Value>(IList<Value> files1, string deli = "\r\n", string whenNoEntries = "")
    {
        if (files1.Count() == 0)
            sb.AppendLine(whenNoEntries);
        else
            foreach (var item in files1)
                Append(item + deli);
        //sb.AppendLine();
    }

    /// <summary>
    ///     must be where Header : IEnumerable<char> (like is string)
    /// </summary>
    /// <typeparam name="Header"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="files1"></param>
    /// <param name="header"></param>
    internal void List<Header, Value>(IList<Value> files1, Header header) where Header : IEnumerable<char>
    {
        List(files1, header, new TextOutputGeneratorArgs { headerWrappedEmptyLines = true, insertCount = false });
    }

    internal void List(IList<string> files1, string header)
    {
        List(files1, header, new TextOutputGeneratorArgs { headerWrappedEmptyLines = true, insertCount = false });
    }


    /// <summary>
    ///     Use DictionaryHelper.CategoryParser
    /// </summary>
    /// <typeparam name="Header"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="files1"></param>
    /// <param name="header"></param>
    /// <param name="a"></param>
    internal void List<Header, Value>(IList<Value> files1, Header header, TextOutputGeneratorArgs a)
        where Header : IEnumerable<char>
    {
        if (a.insertCount)
        {
            //throw new Exception("later");
            //header = (Header)((IList<char>)CA.JoinIList<char>(header, " (" + files1.Count() + ")"));
        }

        if (a.headerWrappedEmptyLines) sb.AppendLine();
        sb.AppendLine(header + ":");
        if (a.headerWrappedEmptyLines) sb.AppendLine();
        List(files1, a.delimiter, a.whenNoEntries);
    }

    #endregion

    #region Paragraph


    /// <summary>
    ///     For ordinary text use Append*
    /// </summary>
    /// <param name="text"></param>
    /// <param name="header"></param>
    internal void Paragraph(string text, string header)
    {
        if (text != string.Empty)
        {
            sb.AppendLine(header + ":");
            sb.AppendLine(text);
            sb.AppendLine();
        }
    }

    #endregion

    #region Dictionary




    private Dictionary<string, List<string>> IGroupingToDictionary(IEnumerable<IGrouping<string, string>> g)
    {
        var l = new Dictionary<string, List<string>>();
        foreach (var item in g) l.Add(item.Key, item.ToList());
        return l;
    }

    internal void Dictionary(Dictionary<string, List<string>> ls)
    {
        foreach (var item in ls) List(item.Value, item.Key);
    }


    #endregion
}