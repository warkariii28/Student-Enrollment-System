// For every HTTP request, pass the request through this middleware.
// If any unhandled exception occurs, catch it and return a standardized error response.

using BrightPath.Exceptions;
using BrightPath.Helpers;
using System.Net;

namespace BrightPath.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            // If ASP.NET has already started sending the response, do not try to replace it.
            if (context.Response.HasStarted)
                throw;

            await HandleException(context, ex);
        }
    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        int statusCode = ex switch
        {
            BadRequestException => (int)HttpStatusCode.BadRequest,    //400
            NotFoundException => (int)HttpStatusCode.NotFound,        //404
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,//401
            _ => (int)HttpStatusCode.InternalServerError              //500
        };

        context.Response.StatusCode = statusCode;

        var message = statusCode == 500
            ? "An unexpected error occurred"
            : ex.Message;

        var response = ResponseHelper.Fail<object>(message);

        return context.Response.WriteAsJsonAsync(response);
    }
}
