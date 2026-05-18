using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Safety;

/// <summary>Domain errors for <see cref="LccSafetyTbl"/> operations.</summary>
public static class LccSafetyErrors
{
    /// <summary>The requested safety record was not found.</summary>
    public static readonly Error NotFound =
        new("Safety.NotFound", "The specified safety incident record was not found.");

    /// <summary>The from-date is after the to-date.</summary>
    public static readonly Error InvalidDateRange =
        new("Safety.InvalidDateRange", "The from-date must be before or equal to the to-date.");
}
