namespace SunamoFileSystem._sunamo.SunamoValues.Constants;


internal class DTConstants
{
    internal const long secondsInMinute = 60;
    internal const long secondsInHour = secondsInMinute * 60;
    internal const long secondsInDay = secondsInHour * 24;
    internal static readonly List<string> daysInWeekENShortcut = new List<string>(["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]);
    internal static readonly List<string> daysInWeekEN = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    internal static readonly List<string> monthsInYearEN = new List<string> { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    internal const int yearStartUnixDate = 1970;
    internal static readonly DateTime UnixFsStart = new DateTime(yearStartUnixDate, 1, 1);
    internal static readonly List<string> daysInWeekCS = new List<string> { Pondeli, Utery, Streda, Ctvrtek, Patek, Sobota, Nedele };
    internal static DateTime unixTimeStartEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
    internal static DateTime winTimeStartEpoch = new DateTime(1601, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    #region Dny v týdny CS
    internal const string Pondeli = "Pond\u011Bl\u00ED";
    internal const string Utery = "\u00DAter\u00FD";
    internal const string Streda = "St\u0159eda";
    internal const string Ctvrtek = "\u010Ctvrtek";
    internal const string Patek = "P\u00E1tek";
    internal const string Sobota = "Sobota";
    internal const string Nedele = "Ned\u011Ble";
    #endregion
    #region Měsíce v roce CS
    internal const string Leden = "Leden";
    internal const string Unor = "\u00DAnor";
    internal const string Brezen = "B\u0159ezen";
    internal const string Duben = "Duben";
    internal const string Kveten = "Kv\u011Bten";
    internal const string Cerven = "\u010Cerven";
    internal const string Cervenec = "\u010Cervenec";
    internal const string Srpen = "Srpen";
    internal const string Zari = "Z\u00E1\u0159\u00ED";
    internal const string Rijen = "\u0158\u00EDjen";
    internal const string Listopad = "Listopad";
    internal const string Prosinec = "Prosinec";
    #endregion
    internal static readonly List<string> monthsInYearCZ = new List<string> { Leden, Unor, Brezen, Duben, Kveten, Cerven, Cervenec, Srpen, Zari, Rijen, Listopad, Prosinec };
}