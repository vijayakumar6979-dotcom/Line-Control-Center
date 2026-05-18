using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>JCAS root cause record from the jcas_rootcause_tbl table (PostgreSQL).</summary>
public sealed partial class JcasRootcauseTbl : Entity<JcasRootcauseId>
{
    private JcasRootcauseTbl() { }

    /// <summary>Creates a <see cref="JcasRootcauseTbl"/> instance from persistence.</summary>
    public static JcasRootcauseTbl From(
        int rcNoId,
        string? jcasRecordNumber,
        string? type,
        string? rootCause,
        string? category,
        DateTime updatedDatetime,
        bool status)
    {
        return new JcasRootcauseTbl
        {
            Id               = JcasRootcauseId.From(rcNoId),
            RcNoId           = rcNoId,
            JcasRecordNumber = jcasRecordNumber,
            Type             = type,
            RootCause        = rootCause,
            Category         = category,
            UpdatedDatetime  = updatedDatetime,
            Status           = status
        };
    }

    /// <summary>Root cause surrogate key.</summary>
    public int RcNoId { get; private set; }

    /// <summary>FK to the parent JCAS record.</summary>
    public string? JcasRecordNumber { get; private set; }

    /// <summary>Root cause type.</summary>
    public string? Type { get; private set; }

    /// <summary>Root cause description.</summary>
    public string? RootCause { get; private set; }

    /// <summary>Root cause category.</summary>
    public string? Category { get; private set; }

    /// <summary>Last updated timestamp.</summary>
    public DateTime UpdatedDatetime { get; private set; }

    /// <summary>true = active; false = canceled.</summary>
    public bool Status { get; private set; }

    /// <summary>Navigation: parent JCAS record.</summary>
    public JcasMainTbl? JcasRecordNumberNavigation { get; private set; }
}