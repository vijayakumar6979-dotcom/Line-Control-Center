using LineControlCenter.Domain.Compliance;
using LineControlCenter.Domain.Safety;
using LineControlCenter.Domain.Mes;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Read-only view of the PostgreSQL database exposed to the Application layer.</summary>
public interface IPostgresqlDbContext
{
    /// <summary>Safety incident records.</summary>
    IQueryable<LccSafetyTbl> LccSafetyTbls { get; }

    /// <summary>NCR/CAR records.</summary>
    IQueryable<LccNcrcarsTbl> LccNcrCarsTbls { get; }

    /// <summary>JCAS main CAR records.</summary>
    IQueryable<JcasMainTbl> JcasMainTbls { get; }

    /// <summary>QRQC ticket records.</summary>
    IQueryable<LccQrqcTicket> LccQrqcTickets { get; }

    /// <summary>JCAS category lookup records.</summary>
    IQueryable<JcasCategoryTbl> JcasCategoryTbls { get; }

    /// <summary>JCAS customer lookup records.</summary>
    IQueryable<JcasCustomerTbl> JcasCustomerTbls { get; }

    /// <summary>MES main records.</summary>
    IQueryable<MesMain> MesMains { get; }

    /// <summary>MES route steps.</summary>
    IQueryable<MesRouteStep> MesRouteSteps { get; }

    /// <summary>MES customers.</summary>
    IQueryable<MesCustomer> MesCustomers { get; }

    /// <summary>MES family lookup.</summary>
    IQueryable<MesFamily> MesFamilies { get; }

    /// <summary>
    /// Returns distinct family_id values from mes_main for the given customer using a
    /// recursive loose-index-scan CTE — O(distinct values) seeks instead of a full table scan.
    /// </summary>
    Task<List<long>> GetDistinctFamilyIdsForCustomerAsync(int customerId, CancellationToken ct = default);
}
