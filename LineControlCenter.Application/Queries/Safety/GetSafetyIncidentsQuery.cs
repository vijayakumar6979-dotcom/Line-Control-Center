using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.Safety;

/// <summary>Returns safety incidents filtered by date range.</summary>
public sealed record GetSafetyIncidentsQuery(
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<Result<IReadOnlyList<LccSafetyTblDto>>>;

/// <summary>Handles <see cref="GetSafetyIncidentsQuery"/>.</summary>
public sealed class GetSafetyIncidentsQueryHandler
    : IRequestHandler<GetSafetyIncidentsQuery, Result<IReadOnlyList<LccSafetyTblDto>>>
{
    private readonly IPostgresqlDbContext _db;

    public GetSafetyIncidentsQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<LccSafetyTblDto>>> Handle(
        GetSafetyIncidentsQuery request, CancellationToken ct)
    {
        var query = _db.LccSafetyTbls.AsNoTracking().AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(x => x.IncidentDatetime >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(x => x.IncidentDatetime <= request.ToDate.Value);

        var results = await query
            .Select(x => new LccSafetyTblDto(
                x.SafetyNoId, x.Site, x.Segment, x.Sector, x.Region,
                x.Status, x.TypeOfIncident, x.TypeOfInjuryOrIllness,
                x.IncidentTitle, x.HighPotential, x.Severity,
                x.LostTimeDays, x.RestrictionOrTransferDays,
                x.InjuryOrIllnessClassification, x.InjuryOrIllnessCauseDirect,
                x.RecordableInjuryOrIllness, x.CapaOrIpNo,
                x.IncidentDatetime, x.CreatedDatetime))
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<LccSafetyTblDto>>(results);
    }
}
