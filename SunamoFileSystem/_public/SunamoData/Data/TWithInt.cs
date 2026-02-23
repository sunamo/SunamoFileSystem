namespace SunamoFileSystem._public.SunamoData.Data;

/// <summary>
/// Holds a value of type T with an integer count.
/// EN: Holds a value of type T with an integer count.
/// CZ: Uchovává hodnotu typu T s celočíselným počtem.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
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
    /// Returns a string representation of the value.
    /// </summary>
    /// <returns>String representation of the value, or "(nulled)" if value is default.</returns>
    public override string? ToString()
    {
        return EqualityComparer<T>.Default.Equals(Value, default) ? "(nulled)" : Value!.ToString();
    }
}