using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Services;

/// <summary>Reads BK test raw data records from MSSQL.</summary>
public sealed class BkTestTarRawDataService : IBkTestTarRawDataService
{
    private readonly IManufacturingDbContext _context;

    public BkTestTarRawDataService(IManufacturingDbContext context) => _context = context;

    public async Task<IEnumerable<BkTestTarRawDatum>> GetAllAsync(
        string? customer, string? testStatus,
        string? shift, string? shiftDate, string? process,
        CancellationToken ct = default)
    {
        var query = _context.BkTestTarRawData.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(customer))
            query = query.Where(x => x.Customer == customer);

        if (!string.IsNullOrEmpty(testStatus))
            query = query.Where(x => x.TestStatus == testStatus);

        if (!string.IsNullOrEmpty(shift))
            query = query.Where(x => x.Shift == shift);

        if (!string.IsNullOrEmpty(shiftDate))
            query = query.Where(x => x.ShiftDate == shiftDate);

        if (!string.IsNullOrEmpty(process))
            query = query.Where(x => x.Process == process);

        return await query.ToListAsync(ct);
    }

    public async Task<IEnumerable<BkTestTarRawDatum>> GetBySerialNumberAsync(
        string serialNumber, CancellationToken ct = default)
    {
        return await _context.BkTestTarRawData
            .AsNoTracking()
            .Where(x => x.SerialNumber == serialNumber)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<BkTestTarRawDatum>> GetFailedTestsAsync(
        CancellationToken ct = default)
    {
        return await _context.BkTestTarRawData
            .AsNoTracking()
            .Where(x => x.TestStatus == "F")
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<BkTestTarRawDatum>> GetByFilterAsync(
        string? customer,
        string? division,
        string? family,
        string? testStatus,
        string? shift,
        DateOnly? shiftDateFrom,
        DateOnly? shiftDateTo,
        CancellationToken ct = default)
    {
        var query = _context.BkTestTarRawData.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(shift))
            query = query.Where(x => x.Shift == shift.Trim());

        if (!string.IsNullOrEmpty(testStatus))
            query = query.Where(x => x.TestStatus == testStatus.Trim().ToUpper());
        else
            query = query.Where(x => x.TestStatus != "A");

        if (!string.IsNullOrEmpty(customer))
            query = query.Where(x => x.Customer == customer);

        if (!string.IsNullOrEmpty(division))
            query = query.Where(x => x.Division == division);

        if (!string.IsNullOrEmpty(family))
            query = query.Where(x => x.Family == family);

        if (shiftDateFrom.HasValue)
        {
            if (shiftDateTo.HasValue && shiftDateFrom == shiftDateTo)
            {
                var dateStr = shiftDateFrom.Value.ToString("MM/dd/yyyy");
                query = query.Where(x => x.ShiftDate == dateStr);
                return await query.ToListAsync(ct);
            }

            var results = await query.ToListAsync(ct);
            return results.Where(x =>
            {
                if (DateOnly.TryParseExact(x.ShiftDate, "MM/dd/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var parsed))
                {
                    return shiftDateTo.HasValue
                        ? parsed >= shiftDateFrom.Value && parsed <= shiftDateTo.Value
                        : parsed == shiftDateFrom.Value;
                }
                return false;
            });
        }

        return await query.ToListAsync(ct);
    }

    public async Task<IEnumerable<BkTestTarRawDatum>> GetDistinctGroupsAsync(
        CancellationToken ct = default)
    {
        var groups = await _context.BkTestTarRawData
            .AsNoTracking()
            .Where(x => x.Customer != null && x.Family != null)
            .Select(x => new { x.Customer, x.Division, x.Family })
            .Distinct()
            .OrderBy(x => x.Customer)
            .ThenBy(x => x.Division)
            .ThenBy(x => x.Family)
            .ToListAsync(ct);

        return groups.Select(g => BkTestTarRawDatum.From(
            serialNumber:  string.Empty,
            customer:      g.Customer,
            division:      g.Division,
            family:        g.Family,
            number:        null,
            process:       null,
            testStatus:    null,
            startDateTime: null,
            endDateTime:   null,
            operatorName:  null,
            testFailure:   null,
            rmaStatus:     null,
            testLoopCount: null,
            testerName:    null,
            source:        null,
            shift:         null,
            shiftDate:     null,
            timeRange:     null));
    }
}
