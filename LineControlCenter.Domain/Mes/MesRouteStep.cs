namespace LineControlCenter.Domain.Mes;

public sealed class MesRouteStep
{
    public long RouteStepNoId { get; set; }
    public long RouteStepId { get; set; }
    public string? StepInstance { get; set; }
    public int? CustId { get; set; }
}
