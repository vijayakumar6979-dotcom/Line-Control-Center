using LineControlCenter.Application.DTOs;

namespace LineControlCenter.UI.Models;

/// <summary>One summary card representing a group of NCR or CAR records.</summary>
public sealed class NcrCarGroupSummary
{
    public string                          GroupTitle  { get; init; } = "";
    public string                          SectionType { get; init; } = ""; // "NCR" | "CAR"
    public IReadOnlyList<LccNcrcarsTblDto> Items       { get; init; } = [];

    /// <summary>
    /// A record is considered closed when Status is "Closed Completed" or "Closed-Cancellation".
    /// All other statuses (In Progress, Pending Approval, Pending Verification,
    /// Pending Confirmation) are treated as open.
    /// </summary>
    private static bool IsClosed(LccNcrcarsTblDto x) =>
        x.Status?.Equals("Closed Completed",    StringComparison.OrdinalIgnoreCase) == true ||
        x.Status?.Equals("Closed-Cancellation", StringComparison.OrdinalIgnoreCase) == true;

    public int    Count        => Items.Count;
    public int    ClosedCount  => Items.Count(IsClosed);
    public int    OpenCount    => Count - ClosedCount;
    public int    OverdueCount => Items.Count(x =>
                                      !IsClosed(x) &&
                                      !string.IsNullOrWhiteSpace(x.ClosureAging));
    public double ClosureRate  => Count == 0 ? 0 : Math.Round((double)ClosedCount / Count * 100, 1);

    public string StatusColor  => Count == 0 ? "#00ff88" : Count <= 3 ? "#ffaa00" : "#ff4444";
    public string CountColor   => OpenCount == 0 ? "#00ff88" : OpenCount <= 3 ? "#ffaa00" : "#ff4444";
    public string OverdueColor => OverdueCount == 0 ? "#556677" : OverdueCount <= 2 ? "#ffaa00" : "#ff4444";

    /// <summary>0=Critical · 1=At Risk · 2=Fair · 3=Acceptable · 4=Excellent — based on open count</summary>
    public int    MoodTier  => OpenCount == 0 ? 4 : OpenCount <= 3 ? 3 : OpenCount <= 6 ? 2 : OpenCount <= 10 ? 1 : 0;
    public string MoodLabel => MoodTier switch { 4 => "EXCELLENT", 3 => "ACCEPTABLE", 2 => "FAIR", 1 => "AT RISK", _ => "CRITICAL" };
    public string MoodColor => MoodTier switch { 4 => "#00ff88", 3 => "#88dd44", 2 => "#ffaa00", 1 => "#ff7722", _ => "#ff4444" };
}
