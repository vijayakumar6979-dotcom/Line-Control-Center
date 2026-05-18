using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain;

/// <summary>Raw test data record from the BK_Test_Tar_RawData table (MSSQL).</summary>
public sealed partial class BkTestTarRawDatum : Entity<BkTestSerialId>
{
    private BkTestTarRawDatum() { }

    /// <summary>Creates a new <see cref="BkTestTarRawDatum"/> instance from persistence.</summary>
    public static BkTestTarRawDatum From(
        string serialNumber,
        string? customer,
        string? division,
        string? family,
        string? number,
        string? process,
        string? testStatus,
        DateTime? startDateTime,
        DateTime? endDateTime,
        string? operatorName,
        string? testFailure,
        string? rmaStatus,
        byte? testLoopCount,
        string? testerName,
        string? source,
        string? shift,
        string? shiftDate,
        string? timeRange)
    {
        return new BkTestTarRawDatum
        {
            Id            = BkTestSerialId.From(serialNumber),
            SerialNumber  = serialNumber,
            Customer      = customer,
            Division      = division,
            Family        = family,
            Number        = number,
            Process       = process,
            TestStatus    = testStatus,
            StartDateTime = startDateTime,
            EndDateTime   = endDateTime,
            Operator      = operatorName,
            TestFailure   = testFailure,
            Rmastatus     = rmaStatus,
            TestLoopCount = testLoopCount,
            TesterName    = testerName,
            Source        = source,
            Shift         = shift,
            ShiftDate     = shiftDate,
            TimeRange     = timeRange
        };
    }

    /// <summary>Serial number (primary identifier).</summary>
    public string SerialNumber { get; private set; } = string.Empty;

    /// <summary>Customer name.</summary>
    public string? Customer { get; private set; }

    /// <summary>Division.</summary>
    public string? Division { get; private set; }

    /// <summary>Product family.</summary>
    public string? Family { get; private set; }

    /// <summary>Part number.</summary>
    public string? Number { get; private set; }

    /// <summary>Test process.</summary>
    public string? Process { get; private set; }

    /// <summary>Test status (P/F/A).</summary>
    public string? TestStatus { get; private set; }

    /// <summary>Test start timestamp.</summary>
    public DateTime? StartDateTime { get; private set; }

    /// <summary>Test end timestamp.</summary>
    public DateTime? EndDateTime { get; private set; }

    /// <summary>Operator ID.</summary>
    public string? Operator { get; private set; }

    /// <summary>Failure description.</summary>
    public string? TestFailure { get; private set; }

    /// <summary>RMA status.</summary>
    public string? Rmastatus { get; private set; }

    /// <summary>Number of test loops.</summary>
    public byte? TestLoopCount { get; private set; }

    /// <summary>Tester equipment name.</summary>
    public string? TesterName { get; private set; }

    /// <summary>Data source.</summary>
    public string? Source { get; private set; }

    /// <summary>Shift (Morning / Night).</summary>
    public string? Shift { get; private set; }

    /// <summary>Shift date string (MM/dd/yyyy).</summary>
    public string? ShiftDate { get; private set; }

    /// <summary>Time range label.</summary>
    public string? TimeRange { get; private set; }
}