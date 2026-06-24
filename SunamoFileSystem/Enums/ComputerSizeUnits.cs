namespace SunamoFileSystem.Enums;

public enum ComputerSizeUnits : byte
{
    // Automatically determine the most appropriate unit.
    Auto = 0,

    // Bytes.
    B = 1,

    // Kilobytes.
    KB = 2,

    // Megabytes.
    MB = 3,

    // Gigabytes.
    GB = 4,

    // Terabytes.
    TB = 5
}
