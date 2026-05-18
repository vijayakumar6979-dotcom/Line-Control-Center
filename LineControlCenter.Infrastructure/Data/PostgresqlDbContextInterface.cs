using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Compliance;
using LineControlCenter.Domain.Safety;
using LineControlCenter.Domain.Mes;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Infrastructure.Data;

public sealed partial class PostgresqlDbContext : IPostgresqlDbContext, IUnitOfWork
{
    IQueryable<LccSafetyTbl>    IPostgresqlDbContext.LccSafetyTbls    => LccSafetyTbls;
    IQueryable<LccNcrcarsTbl>   IPostgresqlDbContext.LccNcrCarsTbls   => LccNcrCarsTbls;
    IQueryable<LccQrqcTicket>   IPostgresqlDbContext.LccQrqcTickets   => LccQrqcTickets;
    IQueryable<JcasMainTbl>     IPostgresqlDbContext.JcasMainTbls     => JcasMainTbls;
    IQueryable<JcasCategoryTbl> IPostgresqlDbContext.JcasCategoryTbls => JcasCategoryTbls;
    IQueryable<JcasCustomerTbl> IPostgresqlDbContext.JcasCustomerTbls => JcasCustomerTbls;
    IQueryable<MesMain>         IPostgresqlDbContext.MesMains         => MesMains;
    IQueryable<MesRouteStep>    IPostgresqlDbContext.MesRouteSteps    => MesRouteSteps;
    IQueryable<MesCustomer>     IPostgresqlDbContext.MesCustomers     => MesCustomers;
    IQueryable<MesFamily>       IPostgresqlDbContext.MesFamilies      => MesFamilies;

    /// <inheritdoc/>
    async Task<List<long>> IPostgresqlDbContext.GetDistinctFamilyIdsForCustomerAsync(
        int customerId, CancellationToken ct)
    {
        // Recursive loose-index-scan CTE: jumps through the (cust_id, family_id) composite
        // index in O(distinct family_id values) seeks instead of scanning all 29M+ rows.
        return await Database
            .SqlQuery<long>($"""
                WITH RECURSIVE loose_scan AS (
                    SELECT MIN(family_id) AS "Value"
                    FROM   mes.mes_main
                    WHERE  cust_id    = {customerId}
                      AND  family_id IS NOT NULL
                    UNION ALL
                    SELECT (
                        SELECT MIN(m.family_id)
                        FROM   mes.mes_main m
                        WHERE  m.cust_id   = {customerId}
                          AND  m.family_id > loose_scan."Value"
                    )
                    FROM loose_scan
                    WHERE loose_scan."Value" IS NOT NULL
                )
                SELECT "Value"
                FROM   loose_scan
                WHERE  "Value" IS NOT NULL
                """)
            .ToListAsync(ct);
    }
}
