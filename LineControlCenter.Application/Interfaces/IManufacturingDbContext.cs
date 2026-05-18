using LineControlCenter.Domain;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Read-only view of the MSSQL manufacturing database exposed to the Application layer.</summary>
public interface IManufacturingDbContext
{
    /// <summary>BK FCT UPH test records.</summary>
    IQueryable<BkFctUph> BkFctUphs { get; }

    /// <summary>BK raw test data records.</summary>
    IQueryable<BkTestTarRawDatum> BkTestTarRawData { get; }
}
