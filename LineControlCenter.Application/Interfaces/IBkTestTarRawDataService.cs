using LineControlCenter.Domain;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Reads BK test raw data records from the MSSQL manufacturing database.</summary>
public interface IBkTestTarRawDataService
{
    /// <summary>Returns records matching the given simple filters.</summary>
    Task<IEnumerable<BkTestTarRawDatum>> GetAllAsync(string? customer,
        string? testStatus, string? shift, string? shiftDate, string? process,
        CancellationToken ct = default);

    /// <summary>Returns records for a specific serial number.</summary>
    Task<IEnumerable<BkTestTarRawDatum>> GetBySerialNumberAsync(
        string serialNumber, CancellationToken ct = default);

    /// <summary>Returns all records with a failing test status.</summary>
    Task<IEnumerable<BkTestTarRawDatum>> GetFailedTestsAsync(CancellationToken ct = default);

    /// <summary>Returns records matching the given advanced filters.</summary>
    Task<IEnumerable<BkTestTarRawDatum>> GetByFilterAsync(
        string? customer,
        string? division,
        string? family,
        string? testStatus,
        string? shift,
        DateOnly? shiftDateFrom,
        DateOnly? shiftDateTo,
        CancellationToken ct = default);

    /// <summary>Returns one representative record per distinct Customer/Division/Family group.</summary>
    Task<IEnumerable<BkTestTarRawDatum>> GetDistinctGroupsAsync(CancellationToken ct = default);
}