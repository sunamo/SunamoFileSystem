// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoFileSystem._sunamo.SunamoCollectionWithoutDuplicates;

internal abstract class CollectionWithoutDuplicatesBase<T> //: IDumpAsString
{
    internal static bool br = false;
    private readonly int count = 10000;
    private readonly List<T> wasNotAdded = new();
    private bool? _allowNull = false;
    internal List<T> c;
    internal List<string> sr;
    protected string ts = null;

    internal CollectionWithoutDuplicatesBase()
    {
        if (br) Debugger.Break();
        c = new List<T>();
    }

    internal CollectionWithoutDuplicatesBase(int count)
    {
        this.count = count;
        c = new List<T>(count);
    }

    internal CollectionWithoutDuplicatesBase(IList<T> l)
    {
        c = new List<T>(l.ToList());
    }

    /// <summary>
    ///     true = compareWithString
    ///     false = !compareWithString
    ///     null = allow null (can't compareWithString)
    /// </summary>
    internal bool? allowNull
    {
        get => _allowNull;
        set
        {
            _allowNull = value;
            if (value.HasValue && value.Value) sr = new List<string>(count);
        }
    }

    internal bool Add(T t2)
    {
        var result = false;
        var con = Contains(t2);
        if (con.HasValue)
        {
            if (!con.Value)
            {
                c.Add(t2);
                result = true;
            }
        }
        else
        {
            if (!allowNull.HasValue)
            {
                c.Add(t2);
                result = true;
            }
        }

        if (result)
            if (IsComparingByString())
                sr.Add(ts);
        return result;
    }

    protected abstract bool IsComparingByString();
    internal abstract bool? Contains(T t2);


    /// <summary>
    ///     If I want without checkink, use c.AddRange
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="withoutChecking"></param>
    internal List<T> AddRange(IList<T> list)
    {
        wasNotAdded.Clear();
        foreach (var item in list)
            if (!Add(item))
                wasNotAdded.Add(item);
        return wasNotAdded;
    }

}