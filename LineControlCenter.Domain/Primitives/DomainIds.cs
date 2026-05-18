namespace LineControlCenter.Domain.Primitives;

/// <summary>Strongly-typed identifier for BkTestTarRawDatum (no natural key — uses serial number string).</summary>
public sealed record BkTestSerialId(string Value)
{
    /// <summary>Creates a new identifier from a serial-number string.</summary>
    public static BkTestSerialId From(string value) => new(value);
}

/// <summary>Strongly-typed identifier for BkFctUph (no natural key — uses serial number string).</summary>
public sealed record BkFctUphSerialId(string Value)
{
    /// <summary>Creates a new identifier from a serial-number string.</summary>
    public static BkFctUphSerialId From(string value) => new(value);
}

/// <summary>Strongly-typed identifier for LccSafetyTbl.</summary>
public sealed record SafetyNoId(string Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static SafetyNoId From(string value) => new(value);
}

/// <summary>Strongly-typed identifier for LccNcrcarsTbl.</summary>
public sealed record NcrcarNoId(int Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static NcrcarNoId From(int value) => new(value);
}

/// <summary>Strongly-typed identifier for JcasMainTbl.</summary>
public sealed record JcasRecordId(string Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static JcasRecordId From(string value) => new(value);
}

/// <summary>Strongly-typed identifier for JcasActionTbl.</summary>
public sealed record JcasActionId(int Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static JcasActionId From(int value) => new(value);
}

/// <summary>Strongly-typed identifier for JcasCategoryTbl.</summary>
public sealed record JcasCategoryId(int Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static JcasCategoryId From(int value) => new(value);
}

/// <summary>Strongly-typed identifier for JcasCustomerTbl.</summary>
public sealed record JcasCustomerId(int Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static JcasCustomerId From(int value) => new(value);
}

/// <summary>Strongly-typed identifier for JcasRootcauseTbl.</summary>
public sealed record JcasRootcauseId(int Value)
{
    /// <summary>Creates a new identifier.</summary>
    public static JcasRootcauseId From(int value) => new(value);
}
