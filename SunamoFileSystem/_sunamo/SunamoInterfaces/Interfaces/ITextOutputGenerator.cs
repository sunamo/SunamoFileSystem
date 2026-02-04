namespace SunamoFileSystem._sunamo.SunamoInterfaces.Interfaces;

/// <summary>
/// Interface for text output generator.
/// </summary>
internal interface ITextOutputGenerator
{
    string PrependEveryNoWhite { get; set; }
    void PairBullet(string key, string value);
    void Append(string text);
    void AppendFormat(string text, params string[] args);
    void AppendLine();
    void AppendLine(string text);
    void AppendLine(StringBuilder text);
    void AppendLineFormat(string text, params string[] args);
    void CountEvery<T>(IList<KeyValuePair<T, int>> pairs);
    void Dictionary(Dictionary<string, int> charEntity, string delimiter);
    void Dictionary(Dictionary<string, List<string>> dictionary);
    void Dictionary(Dictionary<string, string> dictionary);

    void Dictionary<Header, Value>(Dictionary<Header, List<Value>> ls, bool onlyCountInValue = false)
        where Header : IEnumerable<char>;

    void Dictionary<T1, T2>(Dictionary<T1, T2> dictionary, string delimiter = "|") where T1 : notnull;

    string DictionaryBothToStringToSingleLine<Key, Value>(Dictionary<Key, Value> sorted, bool putValueAsFirst,
        string delimiter = " ") where Key : notnull;

    void DictionaryKeyValuePair<T1, T2>(string header, IOrderedEnumerable<KeyValuePair<T1, T2>> ordered);
    void EndRunTime();
    void Header(string headerText);
    void List(IList<string> items);
    void List(IList<string> items, string header);
    void List<Header, Value>(IList<Value> items, Header header) where Header : IEnumerable<char>;

    void List<Header, Value>(IList<Value> items, Header header, object textOutputGeneratorArgs)
        where Header : IEnumerable<char>;

    void List<Value>(IList<Value> items, string delimiter = "\r\n", string whenNoEntries = "");
    void ListObject(IList items);
    void ListSB(StringBuilder onlyStart, string text);
    void ListString(string list, string header);
    void NoData();
    void Paragraph(string text, string header);
    void Paragraph(StringBuilder wrongNumberOfParts, string header);
    void SingleCharLine(char paddingChar, int length);
    void StartRunTime(string text);
    string ToString();
    void Undo();
}