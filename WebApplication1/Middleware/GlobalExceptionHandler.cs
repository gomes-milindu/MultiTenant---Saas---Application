using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // 1. සම්පූර්ණ Error එක Server Logs වල සටහන් කිරීම (Debugging සඳහා)
        _logger.LogError(
            exception,
            "Unhandled exception occurred on {Method} {Path}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.TraceIdentifier);

        // 2. Exception වර්ගය අනුව HTTP Status Code සහ Title තීරණය කිරීම
        var (statusCode, title, detail) = exception switch
        {
            KeyNotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                ex.Message
            ),
            UnauthorizedAccessException ex => (
                StatusCodes.Status403Forbidden,
                "Access Denied",
                ex.Message
            ),
            Microsoft.EntityFrameworkCore.Storage.RetryLimitExceededException or TimeoutException => (
                StatusCodes.Status503ServiceUnavailable, "Service Temporarily Unavailable",
                    "Network connection is slow or unstable. Please try again in a few seconds."),
                    InvalidOperationException ex => (
                        StatusCodes.Status400BadRequest,
                        "Bad Request",
                        ex.Message
            ),

            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid Argument",
                ex.Message
            ),
            // බලාපොරොත්තු නොවූ System / Database දෝෂ (500)
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                _env.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred. Please try again later."
            )
        };

        // 3. RFC 7807 සම්මත ProblemDetails Response එක සැකසීම
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        // Frontend එකට "message" property එකත් පහසුවෙන් කියවීමට එක් කිරීම
        problemDetails.Extensions["message"] = detail;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Exception එක සාර්ථකව handle කළ බව ASP.NET Core වෙත දන්වයි
    }
}