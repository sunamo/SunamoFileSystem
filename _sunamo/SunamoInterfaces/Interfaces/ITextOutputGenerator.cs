namespace SunamoFileSystem._sunamo.SunamoInterfaces.Interfaces;


internal interface ITextOutputGenerator
{
    void PairBullet(string key, string v);
    string prependEveryNoWhite { get; set; }
    void Append(string text);
    void AppendFormat(string text, params string[] p);
    void AppendLine();
    void AppendLine(string text);
    void AppendLine(StringBuilder text);
    void AppendLineFormat(string text, params string[] p);
    void CountEvery<T>(IList<KeyValuePair<T, int>> eq);
    void Dictionary(Dictionary<string, int> charEntity, string delimiter);
    void Dictionary(Dictionary<string, List<string>> ls);
    void Dictionary(Dictionary<string, string> v);
    void Dictionary<Header, Value>(Dictionary<Header, List<Value>> ls, bool onlyCountInValue = false) where Header : IEnumerable<char>;
    void Dictionary<T1, T2>(Dictionary<T1, T2> d, string deli = "|");
    string DictionaryBothToStringToSingleLine<Key, Value>(Dictionary<Key, Value> sorted, bool putValueAsFirst, string delimiter = " ");
    void DictionaryKeyValuePair<T1, T2>(string header, IOrderedEnumerable<KeyValuePair<T1, T2>> ordered);
    void EndRunTime();
    void Header(string v);
    void List(IList<string> files1);
    void List(IList<string> files1, string header);
    void List<Header, Value>(IList<Value> files1, Header header) where Header : IEnumerable<char>;
    void List<Header, Value>(IList<Value> files1, Header header, object textOutputGeneratorArgs) where Header : IEnumerable<char>;
    void List<Value>(IList<Value> files1, string deli = "\r\n", string whenNoEntries = "");
    void ListObject(IList files1);
    void ListSB(StringBuilder onlyStart, string v);
    void ListString(string list, string header);
    void NoData();
    void Paragraph(string text, string header);
    void Paragraph(StringBuilder wrongNumberOfParts, string header);
    void SingleCharLine(char paddingChar, int v);
    void StartRunTime(string text);
    string ToString();
    void Undo();
}