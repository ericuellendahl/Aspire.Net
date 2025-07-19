namespace Aspire.Net.ApiService.Domain.DTOs;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }
    public T? Value { get; private set; }

    protected Result(bool isSuccess, string? error, T? value)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException();
        if (!isSuccess && value != null)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, null, value);
    public static Result<T> Failure(string error) => new(false, error, default);
}
