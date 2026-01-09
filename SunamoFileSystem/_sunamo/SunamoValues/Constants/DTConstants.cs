namespace SunamoFileSystem._sunamo.SunamoValues.Constants;

/// <summary>
/// Constants for date and time operations.
/// EN: Constants for date and time operations.
/// CZ: Konstanty pro operace s datem a časem.
/// </summary>
internal class DTConstants
{
    /// <summary>
    /// Number of seconds in a minute.
    /// </summary>
    internal const long SecondsInMinute = 60;

    /// <summary>
    /// Number of seconds in an hour.
    /// </summary>
    internal const long SecondsInHour = SecondsInMinute * 60;

    /// <summary>
    /// Number of seconds in a day.
    /// </summary>
    internal const long SecondsInDay = SecondsInHour * 24;

    /// <summary>
    /// Year when Unix date starts (1970).
    /// </summary>
    internal const int YearStartUnixDate = 1970;

    /// <summary>
    /// Shortcut names of days in week (English).
    /// </summary>
    internal static readonly List<string> DaysInWeekENShortcut =
        new List<string>(["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);

    /// <summary>
    /// Full names of days in week (English).
    /// </summary>
    internal static readonly List<string> DaysInWeekEN = new()
        { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    /// <summary>
    /// Names of months in year (English).
    /// </summary>
    internal static readonly List<string> MonthsInYearEN = new()
    {
        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November",
        "December"
    };

    /// <summary>
    /// Unix filesystem start date.
    /// </summary>
    internal static readonly DateTime UnixFsStart = new(YearStartUnixDate, 1, 1);

    /// <summary>
    /// Names of days in week (Czech).
    /// </summary>
    internal static readonly List<string> DaysInWeekCS = new()
        { Pondeli, Utery, Streda, Ctvrtek, Patek, Sobota, Nedele };

    /// <summary>
    /// Unix time start epoch.
    /// </summary>
    internal static DateTime UnixTimeStartEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Windows time start epoch.
    /// </summary>
    internal static DateTime WinTimeStartEpoch = new(1601, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Names of months in year (Czech).
    /// </summary>
    internal static readonly List<string> MonthsInYearCZ = new()
        { Leden, Unor, Brezen, Duben, Kveten, Cerven, Cervenec, Srpen, Zari, Rijen, Listopad, Prosinec };

    #region Days in week (Czech names)

    /// <summary>Monday in Czech.</summary>
    internal const string Pondeli = "Pond\u011Bl\u00ED";
    /// <summary>Tuesday in Czech.</summary>
    internal const string Utery = "\u00DAter\u00FD";
    /// <summary>Wednesday in Czech.</summary>
    internal const string Streda = "St\u0159eda";
    /// <summary>Thursday in Czech.</summary>
    internal const string Ctvrtek = "\u010Ctvrtek";
    /// <summary>Friday in Czech.</summary>
    internal const string Patek = "P\u00E1tek";
    /// <summary>Saturday in Czech.</summary>
    internal const string Sobota = "Sobota";
    /// <summary>Sunday in Czech.</summary>
    internal const string Nedele = "Ned\u011Ble";

    #endregion

    #region Months in year (Czech names)

    /// <summary>January in Czech.</summary>
    internal const string Leden = "Leden";
    /// <summary>February in Czech.</summary>
    internal const string Unor = "\u00DAnor";
    /// <summary>March in Czech.</summary>
    internal const string Brezen = "B\u0159ezen";
    /// <summary>April in Czech.</summary>
    internal const string Duben = "Duben";
    /// <summary>May in Czech.</summary>
    internal const string Kveten = "Kv\u011Bten";
    /// <summary>June in Czech.</summary>
    internal const string Cerven = "\u010Cerven";
    /// <summary>July in Czech.</summary>
    internal const string Cervenec = "\u010Cervenec";
    /// <summary>August in Czech.</summary>
    internal const string Srpen = "Srpen";
    /// <summary>September in Czech.</summary>
    internal const string Zari = "Z\u00E1\u0159\u00ED";
    /// <summary>October in Czech.</summary>
    internal const string Rijen = "\u0158\u00EDjen";
    /// <summary>November in Czech.</summary>
    internal const string Listopad = "Listopad";
    /// <summary>December in Czech.</summary>
    internal const string Prosinec = "Prosinec";

    #endregion
}