using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>JCAS main CAR record from the jcas_main_tbl table (PostgreSQL).</summary>
public sealed partial class JcasMainTbl : Entity<JcasRecordId>
{
    private JcasMainTbl() { }

    /// <summary>Creates a <see cref="JcasMainTbl"/> instance from persistence.</summary>
    public static JcasMainTbl From(string jcasRecordNumber)
    {
        return new JcasMainTbl
        {
            Id               = JcasRecordId.From(jcasRecordNumber),
            JcasRecordNumber = jcasRecordNumber
        };
    }

    /// <summary>JCAS record number (PK).</summary>
    public string JcasRecordNumber { get; private set; } = string.Empty;

    /// <summary>Initiator name.</summary>
    public string? JcasInitiator { get; private set; }

    /// <summary>FK to customer.</summary>
    public int CustNoId { get; private set; }

    /// <summary>FK to category.</summary>
    public int CategoryNoId { get; private set; }

    /// <summary>Initiating site.</summary>
    public string? InitiatingSite { get; private set; }

    /// <summary>Receiving site.</summary>
    public string? ReceivingSite { get; private set; }

    /// <summary>Sending site.</summary>
    public string? SendingSite { get; private set; }

    /// <summary>JCAS type.</summary>
    public string? JcasType { get; private set; }

    /// <summary>JCAS owner.</summary>
    public string? JcasOwner { get; private set; }

    /// <summary>Current phase.</summary>
    public string? Phase { get; private set; }

    /// <summary>Current status.</summary>
    public string? Status { get; private set; }

    /// <summary>Origination.</summary>
    public string? Origination { get; private set; }

    /// <summary>Date the JCAS record was created.</summary>
    public DateOnly? JcasCreatedDate { get; private set; }

    /// <summary>Business sector.</summary>
    public string? BusinessSector { get; private set; }

    /// <summary>Failure mode description.</summary>
    public string? FailureMode { get; private set; }

    /// <summary>Failure mode category.</summary>
    public string? FailureModeCategory { get; private set; }

    /// <summary>Title.</summary>
    public string? Title { get; private set; }

    /// <summary>Problem description.</summary>
    public string? ProblemDescription { get; private set; }

    /// <summary>Affected procedures.</summary>
    public string? ProcAffected { get; private set; }

    /// <summary>Severity level.</summary>
    public string? Severity { get; private set; }

    /// <summary>Investigation summary.</summary>
    public string? Investigation { get; private set; }

    /// <summary>Whether D7 is completed.</summary>
    public bool D7IsCompleted { get; private set; }

    /// <summary>Whether D7 is on time.</summary>
    public bool D7IsOntime { get; private set; }

    /// <summary>D7 due date.</summary>
    public DateOnly? D7DueDate { get; private set; }

    /// <summary>D7 completed date.</summary>
    public DateOnly? D7CompletedDate { get; private set; }

    /// <summary>JCAS URL.</summary>
    public string? JcasUrl { get; private set; }

    /// <summary>D4 completed date.</summary>
    public DateOnly? D4CompletedDate { get; private set; }

    /// <summary>D6 due date.</summary>
    public DateOnly? D6DueDate { get; private set; }

    /// <summary>D4 owner.</summary>
    public string? D4Owner { get; private set; }

    /// <summary>D8 owner.</summary>
    public string? D8Owner { get; private set; }

    /// <summary>Navigation: category.</summary>
    public JcasCategoryTbl? CategoryNo { get; private set; }

    /// <summary>Navigation: customer.</summary>
    public JcasCustomerTbl? CustNo { get; private set; }

    /// <summary>Navigation: associated actions.</summary>
    public ICollection<JcasActionTbl> JcasActionTbls { get; private set; } = new List<JcasActionTbl>();

    /// <summary>Navigation: associated root causes.</summary>
    public ICollection<JcasRootcauseTbl> JcasRootcauseTbls { get; private set; } = new List<JcasRootcauseTbl>();
}