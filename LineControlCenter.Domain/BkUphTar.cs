namespace LineControlCenter.Domain;

/// <summary>
/// Keyless entity mapping to the PostgreSQL public.bk_uph_tar table (jbk_te database).
/// Equivalent of the MSSQL BK_Test_Tar_RawData table.
/// </summary>
public sealed class BkUphTar
{
    public string  SerialNumber  { get; set; } = string.Empty;
    public string  Customer      { get; set; } = string.Empty;
    public string  Division      { get; set; } = string.Empty;
    public string  Family        { get; set; } = string.Empty;
    public string  Number        { get; set; } = string.Empty;
    public string  Process       { get; set; } = string.Empty;
    public string? TestStatus    { get; set; }
    public DateTimeOffset? StartDateTime { get; set; }
    public DateTimeOffset? EndDateTime   { get; set; }
    public string? Operator      { get; set; }
    public string? TestFailure   { get; set; }
    public string? RmaStatus     { get; set; }
    public string? TestLoopCount { get; set; }
    public string? TesterName    { get; set; }
    public string? Source        { get; set; }
    public string? Shift         { get; set; }
    public string? ShiftDate     { get; set; }
    public string? TimeRange     { get; set; }
}
