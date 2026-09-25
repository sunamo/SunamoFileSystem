namespace SunamoFileSystem._public.SunamoData.Data;

public class TWithInt<T>
{
    public int Count { get; set; } = 0;

    public T Value { get; set; } = default!;

    public override string? ToString()
    {
        return EqualityComparer<T>.Default.Equals(Value, default) ? "(nulled)" : Value!.ToString();
    }
}
