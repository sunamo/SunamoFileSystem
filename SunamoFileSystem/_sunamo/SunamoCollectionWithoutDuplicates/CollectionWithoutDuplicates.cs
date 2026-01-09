namespace SunamoFileSystem._sunamo.SunamoCollectionWithoutDuplicates;

/// <summary>
/// Collection that does not allow duplicate values.
/// EN: Collection that does not allow duplicate values.
/// CZ: Kolekce která nepovoluje duplicitní hodnoty.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
internal class CollectionWithoutDuplicates<T> : CollectionWithoutDuplicatesBase<T>
{
    /// <summary>
    /// Initializes a new instance of the CollectionWithoutDuplicates class.
    /// </summary>
    internal CollectionWithoutDuplicates()
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified capacity.
    /// </summary>
    /// <param name="count">The initial capacity.</param>
    internal CollectionWithoutDuplicates(int count) : base(count)
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified list.
    /// </summary>
    /// <param name="list">The initial list of items.</param>
    internal CollectionWithoutDuplicates(IList<T> list) : base(list)
    {
    }

    /// <summary>
    /// Determines whether comparison is done by string representation.
    /// </summary>
    /// <returns>True if comparing by string, false otherwise.</returns>
    protected override bool IsComparingByString()
    {
        return allowNull.HasValue && allowNull.Value;
    }

    /// <summary>
    /// Checks if the collection contains the specified value.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the collection contains the value, false otherwise, null if undetermined.</returns>
    internal override bool? Contains(T value)
    {
        return base.c.Contains(value);
    }
}