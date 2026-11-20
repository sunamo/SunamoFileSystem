namespace SunamoFileSystem._public.SunamoData.Data;

/// <summary>
/// Represents the t with int t type.
/// </summary>
public class TWithInt<T>
{
    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    public int Count { get; set; } = 0;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public T Value { get; set; } = default!;

    /// <summary>
    /// To string.
    /// </summary>
    /// <returns>The to string result.</returns>
    public override string? ToString()
    {
        return EqualityComparer<T>.Default.Equals(Value, default) ? "(nulled)" : Value!.ToString();
    }
}
