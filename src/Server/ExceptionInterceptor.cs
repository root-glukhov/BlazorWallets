using BlazorWallets.Shared;
using Newtonsoft.Json;
using System.Net;

namespace BlazorWallets.Server;

public class ExceptionInterceptor
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(RequestDelegate next, ILogger<ExceptionInterceptor> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentNullException ex)
        when (ex.ParamName == "result")
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.TooManyRequests,
                "Слишком много запросов к RPC ноде. Повторите попытку позже");
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.InternalServerError,
                ex.Message);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, string exMessage, HttpStatusCode statusCode, string message)
    {
        _logger.LogError(exMessage);
        var response = context.Response;

        response.ContentType = "application/json";
        response.StatusCode = (int)statusCode;

        var error = new Error
        {
            StatusCode = (int)statusCode,
            Message = message
        };

        var result = JsonConvert.SerializeObject(error);
        await response.WriteAsync(result);
    }
}
