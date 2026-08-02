namespace SunamoFileSystem._sunamo.SunamoCollectionWithoutDuplicates;

// EN: Base class for collections that do not allow duplicate values.
// CZ: Základní třída pro kolekce které nepovolují duplicitní hodnoty.
internal abstract class CollectionWithoutDuplicatesBase<T>
{
    internal static bool BreakOnDebugger = false;
    private readonly int count = 10000;
    private readonly List<T> wasNotAdded = new();
    private bool? _allowNull = false;
    internal List<T> c;
    internal List<string> stringRepresentations = new();
    protected string? temporaryString = null;

    internal CollectionWithoutDuplicatesBase()
    {
        if (BreakOnDebugger) Debugger.Break();
        c = new List<T>();
    }

    internal CollectionWithoutDuplicatesBase(int count)
    {
        this.count = count;
        c = new List<T>(count);
    }

    internal CollectionWithoutDuplicatesBase(IList<T> list)
    {
        c = new List<T>(list.ToList());
    }

    // true = compareWithString
    // false = !compareWithString
    // null = allow null (can't compareWithString)
    internal bool? allowNull
    {
        get => _allowNull;
        set
        {
            _allowNull = value;
            if (value.HasValue && value.Value) stringRepresentations = new List<string>(count);
        }
    }

    internal bool Add(T value)
    {
        var result = false;
        var contains = Contains(value);
        if (contains.HasValue)
        {
            if (!contains.Value)
            {
                c.Add(value);
                result = true;
            }
        }
        else
        {
            if (!allowNull.HasValue)
            {
                c.Add(value);
                result = true;
            }
        }

        if (result)
            if (IsComparingByString())
                stringRepresentations.Add(temporaryString!);
        return result;
    }

    protected abstract bool IsComparingByString();

    internal abstract bool? Contains(T value);

    // If you want without checking, use c.AddRange directly.
    internal List<T> AddRange(IList<T> list)
    {
        wasNotAdded.Clear();
        foreach (var item in list)
            if (!Add(item))
                wasNotAdded.Add(item);
        return wasNotAdded;
    }
}
