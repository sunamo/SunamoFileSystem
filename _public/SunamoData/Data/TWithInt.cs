namespace SunamoFileSystem._public.SunamoData.Data;


public class TWithInt<T>
{
    public T t = default;
    public int count = 0;

    public override string ToString()
    {
        return EqualityComparer<T>.Default.Equals(t, default) ? "(nulled)" : t!.ToString();
    }
}