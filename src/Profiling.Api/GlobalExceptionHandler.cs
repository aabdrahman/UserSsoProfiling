using System;
using System.Data.Common;
using System.Text.Json;
using LoggerService;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Profiling.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILoggerManager _loggerManager;

    public GlobalExceptionHandler(ILoggerManager loggerManager)
    {
        _loggerManager = loggerManager;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        _loggerManager.LogError($"An Error Occurred. {exception.Message}");

        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

        var errorResponse = new ProblemDetails();

        errorResponse.Status = contextFeature?.Error switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            DbException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        errorResponse.Detail = contextFeature?.Error.Message!;
        errorResponse.Detail += exception.InnerException?.Message;

        //_loggerManager.LogError(errorResponse.ToString());
        _loggerManager.LogError(JsonSerializer.Serialize(errorResponse));

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse), cancellationToken);

        return true;
    }
}
