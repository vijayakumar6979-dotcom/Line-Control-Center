using LineControlCenter.Application.DTOs;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Reads JCAS CAR records from the PostgreSQL database.</summary>
public interface IJcasCarService
{
    /// <summary>
    /// Returns all JCAS CAR records whose JcasCreatedDate falls within the current ISO week,
    /// with Customer and Category navigation properties projected to a DTO.
    /// </summary>
    Task<IReadOnlyList<JcasMainTblDto>> GetCurrentWeekCarsAsync(CancellationToken ct = default);
}
