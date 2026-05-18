namespace LineControlCenter.UI.Models;

public class DashboardFilter
{
    public string? Customer { get; set; }
    public string? Division { get; set; }
    public string? Family { get; set; }
    public string? Process { get; set; } = "Backend";
    public DateOnly ShiftDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public int FniPlannedQty { get; set; } = 10000;
    public int DpmTarget { get; set; } = 800;
}