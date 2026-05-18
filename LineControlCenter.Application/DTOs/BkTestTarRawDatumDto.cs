namespace LineControlCenter.Application.DTOs;

/// <summary>Read-only projection of a <c>BkTestTarRawDatum</c> entity.</summary>
public sealed record BkTestTarRawDatumDto(
    string  SerialNumber,
    string? Customer,
    string? Division,
    string? Family,
    string? Number,
    string? Process,
    string? TestStatus,
    DateTime? StartDateTime,
    DateTime? EndDateTime,
    string? Operator,
    string? TestFailure,
    string? Rmastatus,
    byte?   TestLoopCount,
    string? TesterName,
    string? Source,
    string? Shift,
    string? ShiftDate,
    string? TimeRange);
