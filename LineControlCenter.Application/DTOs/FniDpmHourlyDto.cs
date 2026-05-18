namespace LineControlCenter.Application.DTOs;

/// <summary>DPM aggregated for a single hour slot within a day.</summary>
public sealed record FniDpmHourlyDto(
    int    Hour,
    int    Total,
    int    Fail,
    double DpmRate)
{
    public int    Pass        => Total - Fail;
    public string HourLabel   => $"{Hour:D2}:00";
}
