using Microsoft.AspNetCore.Diagnostics;
using Accrual.Application.Exceptions;

namespace Accrual.Api.ExceptionHandlers;

public class ApplicationExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int? status = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            ServiceNotAvailableException => StatusCodes.Status503ServiceUnavailable,
            _ => null
        };

        if (status is null)
            return false;

        httpContext.Response.StatusCode = status.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = { Status = status, Detail = exception.Message }
        });
    }
}