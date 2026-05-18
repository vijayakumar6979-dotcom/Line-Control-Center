namespace LineControlCenter.Application.DTOs;

public sealed record FniYieldDetailDto(
    string? SerialNumber,
    DateTime? StartTime,
    DateTime? EndTime,
    string Status,
    string? CustomerName,
    string? Family,
    string? StepInstance);
