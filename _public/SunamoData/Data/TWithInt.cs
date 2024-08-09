namespace SunamoFileSystem._public.SunamoData.Data;

public class TWithInt<T>
{
    public int count = 0;
    public T t = default;

    public override string ToString()
    {
        return EqualityComparer<T>.Default.Equals(t, default) ? "(nulled)" : t!.ToString();
    }
}