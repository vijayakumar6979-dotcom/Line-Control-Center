namespace LineControlCenter.Application.DTOs;

/// <summary>Read-only projection of a <c>LccSafetyTbl</c> entity.</summary>
public sealed record LccSafetyTblDto(
    string   SafetyNoId,
    string?  Site,
    string?  Segment,
    string?  Sector,
    string?  Region,
    string?  Status,
    string?  TypeOfIncident,
    string?  TypeOfInjuryOrIllness,
    string?  IncidentTitle,
    bool?    HighPotential,
    string?  Severity,
    int?     LostTimeDays,
    int?     RestrictionOrTransferDays,
    string?  InjuryOrIllnessClassification,
    string?  InjuryOrIllnessCauseDirect,
    bool?    RecordableInjuryOrIllness,
    string?  CapaOrIpNo,
    DateTime? IncidentDatetime,
    DateTime? CreatedDatetime);
