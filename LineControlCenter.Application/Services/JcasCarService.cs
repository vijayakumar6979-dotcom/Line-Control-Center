using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Services;

/// <summary>Reads JCAS CAR records from PostgreSQL.</summary>
public sealed class JcasCarService : IJcasCarService
{
    private readonly IPostgresqlDbContext _db;

    public JcasCarService(IPostgresqlDbContext db) => _db = db;

    public async Task<IReadOnlyList<JcasMainTblDto>> GetCurrentWeekCarsAsync(
        CancellationToken ct = default)
    {
        var today     = DateOnly.FromDateTime(DateTime.Today);
        int dayOffset = ((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = today.AddDays(-dayOffset);
        var weekEnd   = weekStart.AddDays(6);

        return await _db.JcasMainTbls
            .AsNoTracking()
            .Include(x => x.CustNo)
            .Include(x => x.CategoryNo)
            .Where(x => x.JcasCreatedDate >= weekStart && x.JcasCreatedDate <= weekEnd)
            .OrderByDescending(x => x.JcasCreatedDate)
            .Select(x => new JcasMainTblDto(
                x.JcasRecordNumber,
                x.JcasInitiator,
                x.CustNo != null ? x.CustNo.CustomerName : null,
                x.CategoryNo != null ? x.CategoryNo.CategoryName : null,
                x.InitiatingSite, x.ReceivingSite, x.SendingSite,
                x.JcasType, x.JcasOwner, x.Phase, x.Status, x.Origination,
                x.JcasCreatedDate, x.BusinessSector,
                x.FailureMode, x.FailureModeCategory,
                x.Title, x.ProblemDescription, x.Severity,
                x.D7IsCompleted, x.D7IsOntime, x.D7DueDate, x.D7CompletedDate,
                x.JcasUrl))
            .ToListAsync(ct);
    }
}
