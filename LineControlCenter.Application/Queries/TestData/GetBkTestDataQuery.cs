using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns filtered test records from PostgreSQL jbk_te public.bk_uph_tar.</summary>
public sealed record GetBkTestDataQuery(
    string?   Customer,
    string?   Division,
    string?   Family,
    string?   TestStatus,
    string?   Shift,
    DateOnly? ShiftDateFrom,
    DateOnly? ShiftDateTo) : IRequest<Result<IReadOnlyList<BkTestTarRawDatumDto>>>;

/// <summary>Handles <see cref="GetBkTestDataQuery"/>.</summary>
public sealed class GetBkTestDataQueryHandler
    : IRequestHandler<GetBkTestDataQuery, Result<IReadOnlyList<BkTestTarRawDatumDto>>>
{
    private readonly IJbkTeDbContext _db;
    private readonly ILogger<GetBkTestDataQueryHandler> _logger;

    public GetBkTestDataQueryHandler(
        IJbkTeDbContext db,
        ILogger<GetBkTestDataQueryHandler> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<BkTestTarRawDatumDto>>> Handle(
        GetBkTestDataQuery request, CancellationToken ct)
    {
        var query = _db.BkUphTars.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.Customer))
            query = query.Where(x => x.Customer == request.Customer);

        if (!string.IsNullOrEmpty(request.Division))
            query = query.Where(x => x.Division == request.Division);

        if (!string.IsNullOrEmpty(request.Family))
            query = query.Where(x => x.Family == request.Family);

        if (!string.IsNullOrEmpty(request.TestStatus))
            query = query.Where(x => x.TestStatus == request.TestStatus.ToUpper());

        if (request.ShiftDateFrom.HasValue && request.ShiftDateTo.HasValue
            && request.ShiftDateFrom == request.ShiftDateTo)
        {
            var shiftDate = request.ShiftDateFrom.Value.ToString("MM/dd/yyyy");
            query = query.Where(x => x.ShiftDate == shiftDate);
        }

        if (!string.IsNullOrEmpty(request.Shift))
            query = query.Where(x => x.Shift == request.Shift);

        var raw = await query
            .Select(x => new
            {
                x.SerialNumber,
                x.Customer,
                x.Division,
                x.Family,
                x.Number,
                x.Process,
                x.TestStatus,
                x.StartDateTime,
                x.EndDateTime,
                x.Operator,
                x.TestFailure,
                x.RmaStatus,
                x.TestLoopCount,
                x.TesterName,
                x.Source,
                x.Shift,
                x.ShiftDate,
                x.TimeRange
            })
            .ToListAsync(ct);

        var results = raw
            .Select(x => new BkTestTarRawDatumDto(
                SerialNumber:  x.SerialNumber  ?? string.Empty,
                Customer:      x.Customer,
                Division:      x.Division,
                Family:        x.Family,
                Number:        x.Number,
                Process:       x.Process,
                TestStatus:    x.TestStatus,
                StartDateTime: x.StartDateTime?.UtcDateTime,
                EndDateTime:   x.EndDateTime?.UtcDateTime,
                Operator:      x.Operator,
                TestFailure:   x.TestFailure,
                Rmastatus:     x.RmaStatus,
                TestLoopCount: byte.TryParse(x.TestLoopCount, out var b) ? b : null,
                TesterName:    x.TesterName,
                Source:        x.Source,
                Shift:         x.Shift,
                ShiftDate:     x.ShiftDate,
                TimeRange:     x.TimeRange))
            .ToList();

        _logger.LogInformation(
            "GetBkTestData (jbk_te): Customer={Customer}, Division={Division}, Family={Family}, " +
            "Shift={Shift}, Date={Date}, RecordCount={Count}",
            request.Customer, request.Division, request.Family,
            request.Shift, request.ShiftDateFrom?.ToString("MM/dd/yyyy"),
            results.Count);

        return Result.Success<IReadOnlyList<BkTestTarRawDatumDto>>(results);
    }
}
