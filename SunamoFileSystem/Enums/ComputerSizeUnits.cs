namespace SunamoFileSystem.Enums;

/// <summary>
/// Represents the computer size units type.
/// </summary>
public enum ComputerSizeUnits : byte
{
    // Automatically determine the most appropriate unit.
    /// <summary>
    /// The auto value.
    /// </summary>
    Auto = 0,

    // Bytes.
    /// <summary>
    /// The b value.
    /// </summary>
    B = 1,

    // Kilobytes.
    /// <summary>
    /// The KB value.
    /// </summary>
    KB = 2,

    // Megabytes.
    /// <summary>
    /// The MB value.
    /// </summary>
    MB = 3,

    // Gigabytes.
    /// <summary>
    /// The GB value.
    /// </summary>
    GB = 4,

    // Terabytes.
    /// <summary>
    /// The TB value.
    /// </summary>
    TB = 5
}
