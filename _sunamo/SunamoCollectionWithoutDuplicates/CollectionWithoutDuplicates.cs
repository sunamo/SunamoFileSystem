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


    internal override bool? Contains(T t2)
    {
        if (IsComparingByString())
        {
            ts = t2.ToString();
            return sr.Contains(ts);
        }

        if (!c.Contains(t2))
        {
            if (EqualityComparer<T>.Default.Equals(t2, default)) return null;
            return false;
        }

        return true;
    }


    protected override bool IsComparingByString()
    {
        return allowNull.HasValue && allowNull.Value;
    }
}