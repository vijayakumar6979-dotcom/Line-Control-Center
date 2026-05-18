namespace LineControlCenter.Application.DTOs;

/// <summary>
/// Payload passed to IEmailReportService to build and send a FNI DPM report email.
/// </summary>
public sealed record DpmReportDto(
    /// <summary>Comma-separated recipient email addresses entered by the user.</summary>
    string ToAddresses,

    /// <summary>"WTD" or "Today" — used in subject and body heading.</summary>
    string PeriodLabel,

    /// <summary>Human-readable date range, e.g. "Jun 09 – Jun 15, 2025" or "Monday, June 16, 2025".</summary>
    string DateRangeLabel,

    string Customer,
    string? Family,

    // ── KPI values ──────────────────────────────────────────────────────────
    double CurrentDpm,
    double PreviousDpm,
    double RollingAvgDpm,

    // ── Detail data ─────────────────────────────────────────────────────────
    IReadOnlyList<FniDpmFamilyRankDto>  FamilyRanking,
    IReadOnlyList<FniYieldDetailDto>    RawDetails,

    /// <summary>Optional free-text note the user typed in the compose panel.</summary>
    string? Note = null
);
