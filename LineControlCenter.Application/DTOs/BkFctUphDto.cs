namespace LineControlCenter.Application.DTOs;

/// <summary>Read-only projection of a <c>BkFctUph</c> entity.</summary>
public sealed record BkFctUphDto(
    string  SerialNumber,
    string? Number,
    string? Revision,
    string? Customer,
    string? Division,
    string? Family,
    string? TestFactory,
    string? TestRoute,
    string? TestRouteStep,
    string? TestEquipment,
    DateTime? TestStartDateTime,
    DateTime? TestEndDateTime,
    string? TestStatus,
    string? ProcessLoop,
    string? TestLoop,
    string? TestUser,
    string? Type,
    string? Shift,
    string? ShiftDate,
    string? TimeRange);
