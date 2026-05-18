namespace LineControlCenter.Application.DTOs;

/// <summary>Read-only projection of a <c>LccNcrcarsTbl</c> entity.</summary>
public sealed record LccNcrcarsTblDto(
    int      NcrcarNoId,
    string?  NcrcarNo,
    string?  NcrType,
    string?  Status,
    string?  CarOwner,
    DateTime? AcknowledgeDate,
    string?  Plant,
    string?  Customer,
    string?  Department,
    string?  ProblemStatementCategory,
    string?  ProblemStatement,
    string?  ProblemDescription,
    string?  Bay,
    string?  StationArea,
    string?  IssueBy,
    DateTime IssueDate,
    string?  ResponseStatus,
    string?  RespondedAging,
    DateTime? ClosureDate,
    string?  ClosureStatus,
    string?  ClosureAging);
