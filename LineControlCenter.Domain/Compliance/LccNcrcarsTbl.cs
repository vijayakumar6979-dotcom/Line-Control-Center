using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>NCR/CAR record from the lcc_ncrcars_tbl table (PostgreSQL).</summary>
public sealed partial class LccNcrcarsTbl : Entity<NcrcarNoId>
{
    private LccNcrcarsTbl() { }

    /// <summary>Creates a <see cref="LccNcrcarsTbl"/> instance from persistence.</summary>
    public static LccNcrcarsTbl From(
        int ncrcarNoId,
        string? ncrcarNo,
        string? ncrType,
        string? status,
        string? carOwner,
        DateTime? acknowledgeDate,
        string? plant,
        string? customer,
        string? department,
        string? problemStatementCategory,
        string? problemStatement,
        string? problemDescription,
        string? bay,
        string? stationArea,
        string? issueBy,
        DateTime issueDate,
        string? responseStatus,
        string? respondedAging,
        DateTime? closureDate,
        string? closureStatus,
        string? closureAging)
    {
        return new LccNcrcarsTbl
        {
            Id                       = Primitives.NcrcarNoId.From(ncrcarNoId),
            NcrcarNoId               = ncrcarNoId,
            NcrcarNo                 = ncrcarNo,
            NcrType                  = ncrType,
            Status                   = status,
            CarOwner                 = carOwner,
            AcknowledgeDate          = acknowledgeDate,
            Plant                    = plant,
            Customer                 = customer,
            Department               = department,
            ProblemStatementCategory = problemStatementCategory,
            ProblemStatement         = problemStatement,
            ProblemDescription       = problemDescription,
            Bay                      = bay,
            StationArea              = stationArea,
            IssueBy                  = issueBy,
            IssueDate                = issueDate,
            ResponseStatus           = responseStatus,
            RespondedAging           = respondedAging,
            ClosureDate              = closureDate,
            ClosureStatus            = closureStatus,
            ClosureAging             = closureAging
        };
    }

    /// <summary>NCR/CAR record surrogate key.</summary>
    public int NcrcarNoId { get; private set; }

    /// <summary>NCR/CAR document number.</summary>
    public string? NcrcarNo { get; private set; }

    /// <summary>NCR type classification.</summary>
    public string? NcrType { get; private set; }

    /// <summary>Current status.</summary>
    public string? Status { get; private set; }

    /// <summary>CAR owner.</summary>
    public string? CarOwner { get; private set; }

    /// <summary>Date the record was acknowledged.</summary>
    public DateTime? AcknowledgeDate { get; private set; }

    /// <summary>Plant code.</summary>
    public string? Plant { get; private set; }

    /// <summary>Customer name.</summary>
    public string? Customer { get; private set; }

    /// <summary>Responsible department.</summary>
    public string? Department { get; private set; }

    /// <summary>Problem statement category.</summary>
    public string? ProblemStatementCategory { get; private set; }

    /// <summary>Problem statement summary.</summary>
    public string? ProblemStatement { get; private set; }

    /// <summary>Detailed problem description.</summary>
    public string? ProblemDescription { get; private set; }

    /// <summary>Bay where the issue was raised.</summary>
    public string? Bay { get; private set; }

    /// <summary>Station or area.</summary>
    public string? StationArea { get; private set; }

    /// <summary>Person who issued the record.</summary>
    public string? IssueBy { get; private set; }

    /// <summary>Date the record was issued.</summary>
    public DateTime IssueDate { get; private set; }

    /// <summary>Response status.</summary>
    public string? ResponseStatus { get; private set; }

    /// <summary>Responded aging metric.</summary>
    public string? RespondedAging { get; private set; }

    /// <summary>Date the record was closed.</summary>
    public DateTime? ClosureDate { get; private set; }

    /// <summary>Closure status.</summary>
    public string? ClosureStatus { get; private set; }

    /// <summary>Closure aging metric.</summary>
    public string? ClosureAging { get; private set; }
}