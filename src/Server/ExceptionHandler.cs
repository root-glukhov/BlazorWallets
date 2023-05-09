using BlazorWallets.Shared;
using Newtonsoft.Json;
using System.Net;

namespace BlazorWallets.Server;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
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
        catch (Exception ex)
        {
            if (ex is TooManyRequestsException)
            {
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.TooManyRequests,
                    "Слишком много запросов к RPC ноде. Повторите попытку позже");
            }
            else
            {
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.InternalServerError,
                    "Ошибка выполнения запроса");
            }
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
