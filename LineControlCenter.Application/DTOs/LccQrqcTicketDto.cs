namespace LineControlCenter.Application.DTOs;

/// <summary>Read-only projection of a <c>LccQrqcTicket</c> entity.</summary>
public sealed record LccQrqcTicketDto(
    int       Id,
    int       BayNoId,
    string?   QrapId,
    DateTime  CreationDate,
    int       TicketId,
    string?   Batch,
    string?   Bay,
    string?   Status,
    string?   Customer,
    string?   Step,
    DateTime  UpdatedDatetime,
    int       AgingDays,
    string?   Process,
    string?   Symptom);
