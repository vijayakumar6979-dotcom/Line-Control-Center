namespace LineControlCenter.Application.Settings;

/// <summary>
/// Configurable DPM calculation settings — loaded from appsettings.json "DpmSettings" section.
/// No restart required: handlers use IOptionsMonitor which reloads on file change.
/// </summary>
public sealed class DpmSettings
{
    public const string SectionName = "DpmSettings";

    /// <summary>
    /// Step instance names that count as the Final Native Inspection (FNI) step.
    /// Defaults to ["HFNI"] which matches the current production filter.
    /// Override via appsettings.json "DpmSettings:FniStepNames".
    /// </summary>
    public List<string> FniStepNames { get; set; } = ["HFNI"];

    /// <summary>
    /// If true, include only records with ProcessLoop == 1 in DPM calculation.
    /// If false, do not filter by ProcessLoop. Defaults to true (production behavior).
    /// </summary>
    public bool FilterProcessLoop { get; set; } = true;

    /// <summary>
    /// If true, include only records with TestLoop == 1 in DPM calculation.
    /// If false, do not filter by TestLoop.
    /// </summary>
    public bool FilterTestLoop { get; set; } = true;
}
