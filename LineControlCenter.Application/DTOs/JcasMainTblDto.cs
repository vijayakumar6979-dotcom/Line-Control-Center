namespace LineControlCenter.Application.DTOs;

/// <summary>Read-only projection of a <c>JcasMainTbl</c> entity (with eager-loaded navigation).</summary>
public sealed record JcasMainTblDto(
    string   JcasRecordNumber,
    string?  JcasInitiator,
    string?  CustomerName,
    string?  CategoryName,
    string?  InitiatingSite,
    string?  ReceivingSite,
    string?  SendingSite,
    string?  JcasType,
    string?  JcasOwner,
    string?  Phase,
    string?  Status,
    string?  Origination,
    DateOnly? JcasCreatedDate,
    string?  BusinessSector,
    string?  FailureMode,
    string?  FailureModeCategory,
    string?  Title,
    string?  ProblemDescription,
    string?  Severity,
    bool     D7IsCompleted,
    bool     D7IsOntime,
    DateOnly? D7DueDate,
    DateOnly? D7CompletedDate,
    string?  JcasUrl);
