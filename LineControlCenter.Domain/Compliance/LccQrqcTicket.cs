using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>QRQC ticket record from the lcc_qrqc_tickets table (PostgreSQL).</summary>
public sealed partial class LccQrqcTicket : Entity<int>
{
    private LccQrqcTicket() { }

    public int        BayNoId          { get; private set; }
    public string?    QrapId           { get; private set; }
    public DateTime   CreationDate     { get; private set; }
    public int        TicketId         { get; private set; }
    public string?    Batch            { get; private set; }
    public string?    Bay              { get; private set; }
    public string?    Status           { get; private set; }
    public string?    Customer         { get; private set; }
    public string?    Step             { get; private set; }
    public DateTime   UpdatedDatetime  { get; private set; }
    public int        AgingDays        { get; private set; }
    public string?    Process          { get; private set; }
    public string?    Symptom          { get; private set; }
}
