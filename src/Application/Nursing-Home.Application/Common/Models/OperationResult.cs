namespace Nursing_Home.Application.Common.Models;
public class OperationResult<TResult>
{
    public TResult Result { get; private set; }

    public bool IsSuccess { get; private set; }
    public bool IsException { get; set; }
    public bool IsNotFound { get; private set; }
    public static OperationResult<TResult> SuccessResult(TResult result)
    {
        return new OperationResult<TResult> { Result = result, IsSuccess = true };
    }

    public static OperationResult<TResult> FailureResult(TResult result = default)
    {
        return new OperationResult<TResult> { Result = result, IsSuccess = false };
    }

    public static OperationResult<TResult> NotFoundResult()
    {
        return new OperationResult<TResult> { IsSuccess = false, IsNotFound = true };
    }
}