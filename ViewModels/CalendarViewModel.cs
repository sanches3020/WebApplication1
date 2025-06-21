public class CalendarViewModel
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int DaysInMonth { get; set; }
    public int FirstDayOfWeek { get; set; } 
    public Dictionary<int, string> SavedEmotions { get; set; } = new();
    public int TodayDay { get; set; }
}
