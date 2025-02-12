namespace SunamoFileSystem._sunamo.SunamoCollectionWithoutDuplicates;

internal class CollectionWithoutDuplicates<T> : CollectionWithoutDuplicatesBase<T>
{
    internal CollectionWithoutDuplicates()
    {
    }

    internal CollectionWithoutDuplicates(int count) : base(count)
    {
    }

    internal CollectionWithoutDuplicates(IList<T> l) : base(l)
    {
    }

    protected override bool IsComparingByString()
    {
        return allowNull.HasValue && allowNull.Value;
    }

    internal override bool? Contains(T t2)
    {
        return base.c.Contains(t2);
    }
}