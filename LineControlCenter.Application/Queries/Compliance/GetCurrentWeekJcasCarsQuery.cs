using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.Compliance;

/// <summary>Returns JCAS CAR records for the current ISO week.</summary>
public sealed record GetCurrentWeekJcasCarsQuery
    : IRequest<Result<IReadOnlyList<JcasMainTblDto>>>;

/// <summary>Handles <see cref="GetCurrentWeekJcasCarsQuery"/>.</summary>
public sealed class GetCurrentWeekJcasCarsQueryHandler
    : IRequestHandler<GetCurrentWeekJcasCarsQuery, Result<IReadOnlyList<JcasMainTblDto>>>
{
    private readonly IPostgresqlDbContext _db;

    public GetCurrentWeekJcasCarsQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<JcasMainTblDto>>> Handle(
        GetCurrentWeekJcasCarsQuery request, CancellationToken ct)
    {
        var today     = DateOnly.FromDateTime(DateTime.Today);
        int dayOffset = ((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = today.AddDays(-dayOffset);
        var weekEnd   = weekStart.AddDays(6);

        var results = await _db.JcasMainTbls
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

        return Result.Success<IReadOnlyList<JcasMainTblDto>>(results);
    }
}
