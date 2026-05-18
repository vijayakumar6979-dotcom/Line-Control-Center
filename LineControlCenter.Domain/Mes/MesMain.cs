namespace LineControlCenter.Domain.Mes;

public sealed class MesMain
{
    public long MesId { get; set; }
    public long? RouteStepId { get; set; }
    public string? SerialNumber { get; set; }
    public int? CustId { get; set; }
    public string? AssyId { get; set; }
    public long? FamilyId { get; set; }
    public int? TestLoop { get; set; }
    public int? ProcessLoop { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Status { get; set; }
}
