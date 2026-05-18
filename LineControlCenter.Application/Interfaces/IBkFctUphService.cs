using LineControlCenter.Domain;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Reads BK FCT UPH records from the MSSQL manufacturing database.</summary>
public interface IBkFctUphService
{
    /// <summary>Returns records matching the given filters.</summary>
    Task<IEnumerable<BkFctUph>> GetAllAsync(string? customer, string? family,
        string? testStatus, string? shift, string? shiftDate,
        CancellationToken ct = default);

    /// <summary>Returns records for a specific serial number.</summary>
    Task<IEnumerable<BkFctUph>> GetBySerialNumberAsync(
        string serialNumber, CancellationToken ct = default);
}