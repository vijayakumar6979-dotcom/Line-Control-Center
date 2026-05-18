namespace LineControlCenter.Domain.Primitives;

/// <summary>Represents a domain error with a code and human-readable description.</summary>
public sealed record Error(string Code, string Description)
{
    /// <summary>Represents the absence of an error.</summary>
    public static readonly Error None = new(string.Empty, string.Empty);
}
