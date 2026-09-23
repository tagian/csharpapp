using CSharpApp.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace CSharpApp.Api.Middleware;

public sealed class UpstreamExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not UpstreamApiException upstreamException)
            return false;

        httpContext.Response.StatusCode = upstreamException.StatusCode;
        httpContext.Response.ContentType = upstreamException.ContentType ?? "application/json";

        if (!string.IsNullOrEmpty(upstreamException.ResponseBody))
        {
            await httpContext.Response.WriteAsync(upstreamException.ResponseBody);
        }
        return true;
    }
}