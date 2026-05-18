using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>Domain errors for <see cref="LccNcrcarsTbl"/> operations.</summary>
public static class LccNcrcarErrors
{
    /// <summary>The requested NCR/CAR record was not found.</summary>
    public static readonly Error NotFound =
        new("NcrCar.NotFound", "The specified NCR/CAR record was not found.");

    /// <summary>The Bay value is required.</summary>
    public static readonly Error BayRequired =
        new("NcrCar.BayRequired", "A Bay value is required to retrieve NCR/CAR records.");
}

/// <summary>Domain errors for <see cref="JcasMainTbl"/> operations.</summary>
public static class JcasCarErrors
{
    /// <summary>The requested JCAS record was not found.</summary>
    public static readonly Error NotFound =
        new("JcasCar.NotFound", "The specified JCAS CAR record was not found.");
}

/// <summary>Domain errors for test-data operations.</summary>
public static class TestDataErrors
{
    /// <summary>The shift value is invalid.</summary>
    public static readonly Error InvalidShift =
        new("TestData.InvalidShift", "Shift must be either 'Morning' or 'Night'.");

    /// <summary>The test status value is invalid.</summary>
    public static readonly Error InvalidTestStatus =
        new("TestData.InvalidTestStatus", "TestStatus must be 'P' or 'F'.");
}
