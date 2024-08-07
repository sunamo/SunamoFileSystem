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

    internal override int AddWithIndex(T t2)
    {
        if (IsComparingByString())
        {
            if (Contains(t2).GetValueOrDefault())
            {
                // Will checkout below
            }
            else
            {
                Add(t2);
                return c.Count - 1;
            }
        }

        var vr = c.IndexOf(t2);
        if (vr == -1)
        {
            Add(t2);
            return c.Count - 1;
        }

        return vr;
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

    internal override int IndexOf(T path)
    {
        if (IsComparingByString()) return sr.IndexOf(path.ToString());
        var vr = c.IndexOf(path);
        if (vr == -1)
        {
            c.Add(path);
            return c.Count - 1;
        }

        return vr;
    }

    protected override bool IsComparingByString()
    {
        return allowNull.HasValue && allowNull.Value;
    }
}