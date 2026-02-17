using System.Text.Json.Serialization;

namespace Domain.Shared;

public class Result
{
    [JsonIgnore]
    public bool IsSuccess { get; }
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

}

public class Result<T>(T? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
    public T Value
    {
        get => IsSuccess
            ? field!
            : throw new InvalidOperationException("Cannot access value of failed result");
    } = value;

    private static Result<T> Success(T value) => new(value, true, Error.None);
    public static new Result<T> Failure(Error error) => new(default, false, error);

    public static implicit operator Result<T>(T value) => Success(value);
}