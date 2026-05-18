using LineControlCenter.Application.DTOs;

namespace LineControlCenter.UI.Models;

public class CardModel
{
    public string? Family { get; set; }
    public string? Customer { get; set; }
    public string? Division { get; set; }
    public string Shift { get; set; } = "";
    public string ShiftDate { get; set; } = "";
    
    // First-pass only: TestLoopCount == 1 (initial attempt). NULL included as fallback.
    public int PassCount => Records.Count(x =>
        x.TestStatus == "P" && (x.TestLoopCount == null || x.TestLoopCount == 1));

    public int FailCount => TestedCount - PassCount;
    public int PlannedQty { get; set; }

    public double PassRate => PlannedQty > 0
        ? Math.Round(PassCount / (double)PlannedQty * 100, 1)
        : 0;

    // Total first-pass attempts: TestLoopCount == 1 (or NULL as fallback)
    public int TestedCount => Records.Count(x =>
        x.TestLoopCount == null || x.TestLoopCount == 1);
    
    public double YieldRate => TestedCount > 0
                                 ? Math.Round(PassCount / (double)TestedCount * 100, 1)
                                 : 0;
    
    public List<BkTestTarRawDatumDto> Records { get; set; } = new();
}