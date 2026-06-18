namespace VerticalSliceTest.Orders.Api.Common.Results;

public class Result
{
    public Result(bool isSuccess, Failure error)
    {
        if (isSuccess && error != Abstractions.Failure.None)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == Abstractions.Failure.None)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess
    {
        get;
    }

    public bool IsFailure => !IsSuccess;

    public Failure Error
    {
        get;
    }

    public static Result Success() => new(true, Abstractions.Failure.None);

    public static Result Failure(Failure error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Abstractions.Failure.None);

    public static Result<TValue> Failure<TValue>(Failure error) => new(default, false, error);

    public static Result<TValue> Create<TValue>(TValue? value)
        => value is not null ? Success(value) : Failure<TValue>(Abstractions.Failure.NullValue);
}