namespace LineControlCenter.Application.DTOs;

/// <summary>DPM aggregated for a full ISO calendar week.</summary>
public sealed record FniDpmWeeklyDto(
    DateOnly WeekStart,
    int      Total,
    int      Fail,
    double   DpmRate)
{
    public int    Pass      => Total - Fail;
    public string WeekLabel => $"W/C {WeekStart:MM/dd}";
}
