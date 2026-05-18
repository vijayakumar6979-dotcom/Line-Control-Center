namespace LineControlCenter.Application.DTOs;

/// <summary>DPM aggregated for a single calendar day.</summary>
public sealed record FniDpmDailyDto(
    DateOnly   Day,
    int        Total,
    int        Fail,
    double     DpmRate)
{
    public int Pass => Total - Fail;
}
