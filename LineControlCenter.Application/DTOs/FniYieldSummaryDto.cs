namespace LineControlCenter.Application.DTOs;

public sealed record FniYieldSummaryDto(
    int PassCount,
    int FailCount,
    int TotalCount,
    double YieldRate,
    IReadOnlyList<FniYieldDetailDto> Details)
{
    /// <summary>Total defect count (= FailCount for DPM calculation).</summary>
    public int TotalDefects => FailCount;

    /// <summary>Defects Per Million: (Defects / TotalCount) * 1,000,000.</summary>
    public double DpmRate => TotalCount > 0
        ? Math.Round((double)TotalDefects / TotalCount * 1_000_000, 1)
        : 0;
}
