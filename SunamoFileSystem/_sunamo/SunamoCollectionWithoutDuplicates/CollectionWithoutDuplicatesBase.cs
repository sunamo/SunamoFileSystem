namespace SunamoFileSystem._sunamo.SunamoCollectionWithoutDuplicates;

/// <summary>
/// Base class for collections that do not allow duplicate values.
/// EN: Base class for collections that do not allow duplicate values.
/// CZ: Základní třída pro kolekce které nepovolují duplicitní hodnoty.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
internal abstract class CollectionWithoutDuplicatesBase<T>
{
    internal static bool BreakOnDebugger = false;
    private readonly int count = 10000;
    private readonly List<T> wasNotAdded = new();
    private bool? _allowNull = false;
    internal List<T> c;
    internal List<string> stringRepresentations = new();
    protected string? temporaryString = null;

    /// <summary>
    /// Initializes a new instance of the CollectionWithoutDuplicatesBase class.
    /// </summary>
    internal CollectionWithoutDuplicatesBase()
    {
        if (BreakOnDebugger) Debugger.Break();
        c = new List<T>();
    }

    /// <summary>
    /// Initializes a new instance with the specified capacity.
    /// </summary>
    /// <param name="count">The initial capacity.</param>
    internal CollectionWithoutDuplicatesBase(int count)
    {
        this.count = count;
        c = new List<T>(count);
    }

    /// <summary>
    /// Initializes a new instance with the specified list.
    /// </summary>
    /// <param name="list">The initial list of items.</param>
    internal CollectionWithoutDuplicatesBase(IList<T> list)
    {
        c = new List<T>(list.ToList());
    }

    /// <summary>
    /// Gets or sets whether null values are allowed.
    /// true = compareWithString
    /// false = !compareWithString
    /// null = allow null (can't compareWithString)
    /// </summary>
    internal bool? allowNull
    {
        get => _allowNull;
        set
        {
            _allowNull = value;
            if (value.HasValue && value.Value) stringRepresentations = new List<string>(count);
        }
    }

    /// <summary>
    /// Adds a value to the collection if it doesn't already exist.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>True if the value was added, false if it already exists.</returns>
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

    /// <summary>
    /// Determines whether comparison is done by string representation.
    /// </summary>
    /// <returns>True if comparing by string, false otherwise.</returns>
    protected abstract bool IsComparingByString();

    /// <summary>
    /// Checks if the collection contains the specified value.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the collection contains the value, false otherwise, null if undetermined.</returns>
    internal abstract bool? Contains(T value);


    /// <summary>
    /// Adds a range of values to the collection.
    /// If you want without checking, use c.AddRange directly.
    /// </summary>
    /// <param name="list">The list of values to add.</param>
    /// <returns>List of values that were not added (duplicates).</returns>
    internal List<T> AddRange(IList<T> list)
    {
        wasNotAdded.Clear();
        foreach (var item in list)
            if (!Add(item))
                wasNotAdded.Add(item);
        return wasNotAdded;
    }

}