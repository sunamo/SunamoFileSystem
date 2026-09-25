namespace SunamoFileSystem._sunamo.SunamoCollectionWithoutDuplicates;

// EN: Collection that does not allow duplicate values.
// CZ: Kolekce která nepovoluje duplicitní hodnoty.
internal class CollectionWithoutDuplicates<T> : CollectionWithoutDuplicatesBase<T>
{
    internal CollectionWithoutDuplicates()
    {
    }

    internal CollectionWithoutDuplicates(int count) : base(count)
    {
    }

    internal CollectionWithoutDuplicates(IList<T> list) : base(list)
    {
    }

    protected override bool IsComparingByString() => allowNull.HasValue && allowNull.Value;

    internal override bool? Contains(T value) => base.c.Contains(value);
}
