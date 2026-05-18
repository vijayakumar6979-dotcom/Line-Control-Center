using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain;

/// <summary>FCT UPH test record from the BK_FCT_UPH table (MSSQL).</summary>
public sealed partial class BkFctUph : Entity<BkFctUphSerialId>
{
    private BkFctUph() { }

    /// <summary>Creates a new <see cref="BkFctUph"/> instance from persistence.</summary>
    public static BkFctUph From(
        string serialNumber,
        string? number,
        string? revision,
        string? customer,
        string? division,
        string? family,
        string? testFactory,
        string? testRoute,
        string? testRouteStep,
        string? testEquipment,
        DateTime? testStartDateTime,
        DateTime? testEndDateTime,
        string? testStatus,
        string? processLoop,
        string? testLoop,
        string? testUserIdId,
        string? testUser,
        string? type,
        string? shift,
        string? shiftDate,
        string? timeRange)
    {
        return new BkFctUph
        {
            Id                = BkFctUphSerialId.From(serialNumber),
            SerialNumber      = serialNumber,
            Number            = number,
            Revision          = revision,
            Customer          = customer,
            Division          = division,
            Family            = family,
            TestFactory       = testFactory,
            TestRoute         = testRoute,
            TestRouteStep     = testRouteStep,
            TestEquipment     = testEquipment,
            TestStartDateTime = testStartDateTime,
            TestEndDateTime   = testEndDateTime,
            TestStatus        = testStatus,
            ProcessLoop       = processLoop,
            TestLoop          = testLoop,
            TestUserIdId      = testUserIdId,
            TestUser          = testUser,
            Type              = type,
            Shift             = shift,
            ShiftDate         = shiftDate,
            TimeRange         = timeRange
        };
    }

    /// <summary>Serial number (primary identifier).</summary>
    public string SerialNumber { get; private set; } = string.Empty;

    /// <summary>Part number.</summary>
    public string? Number { get; private set; }

    /// <summary>Revision.</summary>
    public string? Revision { get; private set; }

    /// <summary>Customer name.</summary>
    public string? Customer { get; private set; }

    /// <summary>Division.</summary>
    public string? Division { get; private set; }

    /// <summary>Product family.</summary>
    public string? Family { get; private set; }

    /// <summary>Test factory name.</summary>
    public string? TestFactory { get; private set; }

    /// <summary>Test route.</summary>
    public string? TestRoute { get; private set; }

    /// <summary>Test route step.</summary>
    public string? TestRouteStep { get; private set; }

    /// <summary>Test equipment identifier.</summary>
    public string? TestEquipment { get; private set; }

    /// <summary>Test start timestamp.</summary>
    public DateTime? TestStartDateTime { get; private set; }

    /// <summary>Test end timestamp.</summary>
    public DateTime? TestEndDateTime { get; private set; }

    /// <summary>Test status (P/F/A).</summary>
    public string? TestStatus { get; private set; }

    /// <summary>Process loop.</summary>
    public string? ProcessLoop { get; private set; }

    /// <summary>Test loop.</summary>
    public string? TestLoop { get; private set; }

    /// <summary>Test user ID (raw).</summary>
    public string? TestUserIdId { get; private set; }

    /// <summary>Test user display name.</summary>
    public string? TestUser { get; private set; }

    /// <summary>Record type.</summary>
    public string? Type { get; private set; }

    /// <summary>Shift (Morning / Night).</summary>
    public string? Shift { get; private set; }

    /// <summary>Shift date string (MM/dd/yyyy).</summary>
    public string? ShiftDate { get; private set; }

    /// <summary>Time range label.</summary>
    public string? TimeRange { get; private set; }
}