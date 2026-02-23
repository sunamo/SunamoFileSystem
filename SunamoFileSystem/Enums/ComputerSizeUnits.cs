namespace SunamoFileSystem.Enums;

/// <summary>
/// Units of computer storage size measurement.
/// </summary>
public enum ComputerSizeUnits : byte
{
    /// <summary>
    /// Automatically determine the most appropriate unit.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Bytes.
    /// </summary>
    B = 1,

    /// <summary>
    /// Kilobytes.
    /// </summary>
    KB = 2,

    /// <summary>
    /// Megabytes.
    /// </summary>
    MB = 3,

    /// <summary>
    /// Gigabytes.
    /// </summary>
    GB = 4,

    /// <summary>
    /// Terabytes.
    /// </summary>
    TB = 5
}