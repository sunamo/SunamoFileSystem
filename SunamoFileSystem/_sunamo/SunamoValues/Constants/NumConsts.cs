namespace SunamoFileSystem._sunamo.SunamoValues.Constants;

/// <summary>
/// Numeric constants.
/// EN: Numeric constants.
/// CZ: Numerické konstanty.
/// </summary>
internal class NumConsts
{
    #region For easy copy

    /// <summary>
    /// Minus one (-1).
    /// </summary>
    internal const int MOne = -1;

    #endregion

    /// <summary>
    /// Default port if cannot be parsed (587).
    /// </summary>
    internal const int DefaultPortIfCannotBeParsed = 587;

    /// <summary>
    /// Minimum age is 18 due to GDPR - below 18 is needed parent agreement of child.
    /// </summary>
    internal const int MinAge = 18;

    /// <summary>
    /// One kilobyte in bytes (1024).
    /// </summary>
    internal const long KB = 1024;

    /// <summary>
    /// Zero as double.
    /// </summary>
    internal const double ZeroDouble = 0;

    /// <summary>
    /// Zero as float.
    /// </summary>
    internal const float ZeroFloat = 0;

    /// <summary>
    /// One (at int should be no postfix).
    /// </summary>
    internal const int One = 1;

    /// <summary>
    /// Zero as int.
    /// </summary>
    internal const int ZeroInt = 0;

    /// <summary>
    /// DateTime minimum value.
    /// </summary>
    internal static short NDtMinVal = 10101;

    /// <summary>
    /// DateTime maximum value.
    /// </summary>
    internal static short NDtMaxVal = 32271;

    /// <summary>
    /// One thousand (1000).
    /// </summary>
    internal static int ThousandValue = 1000;
}