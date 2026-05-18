namespace LineControlCenter.Application.DTOs;

/// <summary>DPM aggregated per product family for a given period.</summary>
public sealed record FniDpmFamilyRankDto(
    string Family,
    int    Total,
    int    Fail,
    double DpmRate)
{
    public int Pass => Total - Fail;
}
