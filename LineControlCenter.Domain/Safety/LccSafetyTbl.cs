using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Safety;

/// <summary>Safety incident record from the lcc_safety_tbl table (PostgreSQL).</summary>
public sealed partial class LccSafetyTbl : Entity<SafetyNoId>
{
    private LccSafetyTbl() { }

    /// <summary>Creates a <see cref="LccSafetyTbl"/> instance from persistence.</summary>
    public static LccSafetyTbl From(
        string safetyNoId,
        string? site,
        string? segment,
        string? sector,
        string? region,
        string? status,
        string? typeOfIncident,
        string? typeOfInjuryOrIllness,
        string? incidentTitle,
        bool? highPotential,
        string? severity,
        int? lostTimeDays,
        int? restrictionOrTransferDays,
        string? injuryOrIllnessClassification,
        string? injuryOrIllnessCauseDirect,
        bool? recordableInjuryOrIllness,
        string? capaOrIpNo,
        DateTime? incidentDatetime,
        DateTime? createdDatetime)
    {
        return new LccSafetyTbl
        {
            Id                             = Primitives.SafetyNoId.From(safetyNoId),
            SafetyNoId                     = safetyNoId,
            Site                           = site,
            Segment                        = segment,
            Sector                         = sector,
            Region                         = region,
            Status                         = status,
            TypeOfIncident                 = typeOfIncident,
            TypeOfInjuryOrIllness          = typeOfInjuryOrIllness,
            IncidentTitle                  = incidentTitle,
            HighPotential                  = highPotential,
            Severity                       = severity,
            LostTimeDays                   = lostTimeDays,
            RestrictionOrTransferDays      = restrictionOrTransferDays,
            InjuryOrIllnessClassification  = injuryOrIllnessClassification,
            InjuryOrIllnessCauseDirect     = injuryOrIllnessCauseDirect,
            RecordableInjuryOrIllness      = recordableInjuryOrIllness,
            CapaOrIpNo                     = capaOrIpNo,
            IncidentDatetime               = incidentDatetime,
            CreatedDatetime                = createdDatetime
        };
    }

    /// <summary>Safety incident unique ID (PK).</summary>
    public string SafetyNoId { get; private set; } = string.Empty;

    /// <summary>Site name.</summary>
    public string? Site { get; private set; }

    /// <summary>Segment.</summary>
    public string? Segment { get; private set; }

    /// <summary>Sector.</summary>
    public string? Sector { get; private set; }

    /// <summary>Region.</summary>
    public string? Region { get; private set; }

    /// <summary>Incident status.</summary>
    public string? Status { get; private set; }

    /// <summary>Type of incident.</summary>
    public string? TypeOfIncident { get; private set; }

    /// <summary>Type of injury or illness.</summary>
    public string? TypeOfInjuryOrIllness { get; private set; }

    /// <summary>Title of the incident.</summary>
    public string? IncidentTitle { get; private set; }

    /// <summary>Whether the incident is high-potential.</summary>
    public bool? HighPotential { get; private set; }

    /// <summary>Severity classification.</summary>
    public string? Severity { get; private set; }

    /// <summary>Number of lost-time days.</summary>
    public int? LostTimeDays { get; private set; }

    /// <summary>Number of restriction or transfer days.</summary>
    public int? RestrictionOrTransferDays { get; private set; }

    /// <summary>Injury or illness classification.</summary>
    public string? InjuryOrIllnessClassification { get; private set; }

    /// <summary>Direct cause of injury or illness.</summary>
    public string? InjuryOrIllnessCauseDirect { get; private set; }

    /// <summary>Whether the incident is OSHA recordable.</summary>
    public bool? RecordableInjuryOrIllness { get; private set; }

    /// <summary>Associated CAPA or IP number.</summary>
    public string? CapaOrIpNo { get; private set; }

    /// <summary>Date and time of the incident.</summary>
    public DateTime? IncidentDatetime { get; private set; }

    /// <summary>Record creation timestamp.</summary>
    public DateTime? CreatedDatetime { get; private set; }
}