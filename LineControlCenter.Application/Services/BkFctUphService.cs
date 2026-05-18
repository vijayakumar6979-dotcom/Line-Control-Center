using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Services;

/// <summary>Reads BK FCT UPH records from MSSQL.</summary>
public sealed class BkFctUphService : IBkFctUphService
{
    private readonly IManufacturingDbContext _context;

    public BkFctUphService(IManufacturingDbContext context) => _context = context;

    public async Task<IEnumerable<BkFctUph>> GetAllAsync(
        string? customer, string? family,
        string? testStatus, string? shift, string? shiftDate,
        CancellationToken ct = default)
    {
        var query = _context.BkFctUphs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(customer))
            query = query.Where(x => x.Customer == customer);

        if (!string.IsNullOrEmpty(family))
            query = query.Where(x => x.Family == family);

        if (!string.IsNullOrEmpty(testStatus))
            query = query.Where(x => x.TestStatus == testStatus);

        if (!string.IsNullOrEmpty(shift))
            query = query.Where(x => x.Shift == shift);

        if (!string.IsNullOrEmpty(shiftDate))
            query = query.Where(x => x.ShiftDate == shiftDate);

        return await query.ToListAsync(ct);
    }

    public async Task<IEnumerable<BkFctUph>> GetBySerialNumberAsync(
        string serialNumber, CancellationToken ct = default)
    {
        return await _context.BkFctUphs
            .AsNoTracking()
            .Where(x => x.SerialNumber == serialNumber)
            .ToListAsync(ct);
    }
}