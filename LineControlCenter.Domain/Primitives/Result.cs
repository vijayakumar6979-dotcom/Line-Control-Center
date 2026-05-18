namespace LineControlCenter.Domain.Primitives;

/// <summary>Represents the outcome of an operation that can succeed or fail.</summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("A successful result cannot carry an error.");
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("A failed result must carry an error.");

        IsSuccess = isSuccess;
        Error     = error;
    }

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool  IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool  IsFailure => !IsSuccess;

    /// <summary>The error associated with a failed result, or <see cref="Error.None"/>.</summary>
    public Error Error { get; }

    /// <summary>Creates a successful result.</summary>
    public static Result Success()                         => new(true,  Error.None);

    /// <summary>Creates a failed result carrying the given error.</summary>
    public static Result Failure(Error error)              => new(false, error);

    /// <summary>Creates a successful result carrying a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true,  Error.None);

    /// <summary>Creates a failed result of the given value type.</summary>
    public static Result<TValue> Failure<TValue>(Error error)  => new(default!, false, error);
}

/// <summary>Represents the outcome of an operation that returns a value on success.</summary>
public sealed class Result<TValue> : Result
{
    private readonly TValue _value;

    internal Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
        => _value = value;

    /// <summary>Gets the result value; throws if the result is a failure.</summary>
    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>Implicitly converts a value to a successful result.</summary>
    public static implicit operator Result<TValue>(TValue value) => Result.Success(value);

    /// <summary>Implicitly converts an error to a failed result.</summary>
    public static implicit operator Result<TValue>(Error error)  => Result.Failure<TValue>(error);
}
