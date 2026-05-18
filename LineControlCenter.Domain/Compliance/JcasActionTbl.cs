using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>JCAS action record from the jcas_action_tbl table (PostgreSQL).</summary>
public sealed partial class JcasActionTbl : Entity<JcasActionId>
{
    private JcasActionTbl() { }

    /// <summary>Creates a <see cref="JcasActionTbl"/> instance from persistence.</summary>
    public static JcasActionTbl From(
        int actionNoId,
        string? jcasRecordNumber,
        string? type,
        string? action,
        string? owner,
        DateTime updatedDatetime,
        bool status)
    {
        return new JcasActionTbl
        {
            Id               = JcasActionId.From(actionNoId),
            ActionNoId       = actionNoId,
            JcasRecordNumber = jcasRecordNumber,
            Type             = type,
            Action           = action,
            Owner            = owner,
            UpdatedDatetime  = updatedDatetime,
            Status           = status
        };
    }

    /// <summary>Action surrogate key.</summary>
    public int ActionNoId { get; private set; }

    /// <summary>FK to the parent JCAS record.</summary>
    public string? JcasRecordNumber { get; private set; }

    /// <summary>Action type.</summary>
    public string? Type { get; private set; }

    /// <summary>Action description.</summary>
    public string? Action { get; private set; }

    /// <summary>Action owner.</summary>
    public string? Owner { get; private set; }

    /// <summary>Last updated timestamp.</summary>
    public DateTime UpdatedDatetime { get; private set; }

    /// <summary>true = active; false = canceled.</summary>
    public bool Status { get; private set; }

    /// <summary>Navigation: parent JCAS record.</summary>
    public JcasMainTbl? JcasRecordNumberNavigation { get; private set; }
}