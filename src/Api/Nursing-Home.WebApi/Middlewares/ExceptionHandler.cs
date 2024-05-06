using System.Net;
using System.Text.Json;

namespace Nursing_Home.WebApi.Middlewares;

public class ExceptionHandler : IMiddleware
{
    private readonly ILogger<ExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandler(ILogger<ExceptionHandler> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError("Something Went Wrong");
            await HanderException(context, e);
        }
    }

    private Task HanderException(HttpContext context, Exception e)
    {
        int statusCode = (int)HttpStatusCode.InternalServerError;
        if (e is HttpRequestException httpException)
        {
            if (httpException.StatusCode != null)
            {
                statusCode = (int)httpException.StatusCode;
            }
        }

        var errorResponse = new ErrorException
        {
            StatusCode = statusCode,
            Message = e.Message,
            IsSuccess = false,
            ErrorType = e.GetType().Name
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}

public class ErrorException
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorType { get; set; }

}