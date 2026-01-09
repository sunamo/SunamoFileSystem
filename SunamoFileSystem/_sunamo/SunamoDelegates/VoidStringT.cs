namespace SunamoFileSystem._sunamo.SunamoDelegates;

/// <summary>
/// Delegate for actions that take a string and a value of type T.
/// EN: Delegate for actions that take a string and a value of type T.
/// CZ: Delegát pro akce které přijímají string a hodnotu typu T.
/// </summary>
/// <typeparam name="T">The type of the second parameter.</typeparam>
/// <param name="text">The string parameter.</param>
/// <param name="value">The value of type T.</param>
internal delegate void VoidStringT<T>(string text, T value);