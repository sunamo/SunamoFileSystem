namespace SunamoFileSystem._sunamo.SunamoValues.Constants;

internal class DTConstants
{
    internal const long SecondsInMinute = 60;

    internal const long SecondsInHour = SecondsInMinute * 60;

    internal const long SecondsInDay = SecondsInHour * 24;

    internal const int YearStartUnixDate = 1970;

    internal static readonly List<string> DaysInWeekENShortcut =
        new List<string>(["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);

    internal static readonly List<string> DaysInWeekEN = new()
        { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

    internal static readonly List<string> MonthsInYearEN = new()
    {
        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November",
        "December"
    };

    internal static readonly DateTime UnixFsStart = new(YearStartUnixDate, 1, 1);

    internal static readonly List<string> DaysInWeekCS = new()
        { Pondeli, Utery, Streda, Ctvrtek, Patek, Sobota, Nedele };

    internal static DateTime UnixTimeStartEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    internal static DateTime WinTimeStartEpoch = new(1601, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly List<string> MonthsInYearCZ = new()
        { Leden, Unor, Brezen, Duben, Kveten, Cerven, Cervenec, Srpen, Zari, Rijen, Listopad, Prosinec };

    #region Days in week (Czech names)

    internal const string Pondeli = "Pondělí";
    internal const string Utery = "Úterý";
    internal const string Streda = "Středa";
    internal const string Ctvrtek = "Čtvrtek";
    internal const string Patek = "Pátek";
    internal const string Sobota = "Sobota";
    internal const string Nedele = "Neděle";

    #endregion

    #region Months in year (Czech names)

    internal const string Leden = "Leden";
    internal const string Unor = "Únor";
    internal const string Brezen = "Březen";
    internal const string Duben = "Duben";
    internal const string Kveten = "Květen";
    internal const string Cerven = "Červen";
    internal const string Cervenec = "Červenec";
    internal const string Srpen = "Srpen";
    internal const string Zari = "Září";
    internal const string Rijen = "Říjen";
    internal const string Listopad = "Listopad";
    internal const string Prosinec = "Prosinec";

    #endregion
}
