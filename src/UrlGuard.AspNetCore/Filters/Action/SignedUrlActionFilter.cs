using System.Net;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace UrlGuard;

/// <summary>
/// Action filter for validating signed URLs based on configured options.
/// </summary>
public class SignedUrlActionFilter : IAsyncActionFilter
{
    private readonly IOptions<SignedUrlOptions> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignedUrlActionFilter"/> class.
    /// </summary>
    /// <param name="options">The signed URL options configured for the application.</param>
    public SignedUrlActionFilter(IOptions<SignedUrlOptions> options)
    {
        _options = options;
    }

    /// <summary>
    /// Validates the incoming request's URL against the configured signed URL options.
    /// </summary>
    /// <param name="context">The context for the action being executed.</param>
    /// <param name="next">The delegate to execute the next action filter or the action itself.</param>
    /// <returns>A task that represents the asynchronous action filter operation.</returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var remoteAddress = context.HttpContext.Connection.RemoteIpAddress;

        var uri = new Uri(request.GetEncodedUrl());

        var guard = new SignedUrlGuard(_options.Value.Secret);
        var result = guard.ValidateSignedUrl(uri, remoteAddress!);

        if (result.IsFailure)
        {
            context.Result = new ContentResult
            {
                Content = result.Error!.Message,
                ContentType = "text/plain",
                StatusCode = (int)HttpStatusCode.Forbidden
            };

            return;
        }

        await next();
    }
}
