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
        { MondayCs, TuesdayCs, WednesdayCs, ThursdayCs, FridayCs, SaturdayCs, SundayCs };

    internal static DateTime UnixTimeStartEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    internal static DateTime WinTimeStartEpoch = new(1601, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    internal static readonly List<string> MonthsInYearCZ = new()
        { JanuaryCs, FebruaryCs, MarchCs, AprilCs, MayCs, JuneCs, JulyCs, AugustCs, SeptemberCs, OctoberCs, NovemberCs, DecemberCs };

    #region Days in week (Czech names)

    internal const string MondayCs = "Pondělí";
    internal const string TuesdayCs = "Úterý";
    internal const string WednesdayCs = "Středa";
    internal const string ThursdayCs = "Čtvrtek";
    internal const string FridayCs = "Pátek";
    internal const string SaturdayCs = "Sobota";
    internal const string SundayCs = "Neděle";

    #endregion

    #region Months in year (Czech names)

    internal const string JanuaryCs = "Leden";
    internal const string FebruaryCs = "Únor";
    internal const string MarchCs = "Březen";
    internal const string AprilCs = "Duben";
    internal const string MayCs = "Květen";
    internal const string JuneCs = "Červen";
    internal const string JulyCs = "Červenec";
    internal const string AugustCs = "Srpen";
    internal const string SeptemberCs = "Září";
    internal const string OctoberCs = "Říjen";
    internal const string NovemberCs = "Listopad";
    internal const string DecemberCs = "Prosinec";

    #endregion
}
