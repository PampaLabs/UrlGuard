#if NET7_0_OR_GREATER
using System.Net;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace UrlGuard;

/// <summary>
/// Endpoint filter for validating signed URLs based on configured options.
/// </summary>
public class SignedUrlEndpointFilter : IEndpointFilter
{
    private readonly IOptions<SignedUrlOptions> _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignedUrlEndpointFilter"/> class.
    /// </summary>
    /// <param name="options">The signed URL options configured for the application.</param>
    public SignedUrlEndpointFilter(IOptions<SignedUrlOptions> options)
    {
        _options = options;
    }

    /// <summary>
    /// Validates the incoming request's URL against the configured signed URL options.
    /// </summary>
    /// <param name="context">The context for the endpoint filter invocation.</param>
    /// <param name="next">The delegate to invoke the next endpoint filter or endpoint handler.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous endpoint filter operation.</returns>
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;
        var remoteAddress = context.HttpContext.Connection.RemoteIpAddress;

        var uri = new Uri(request.GetEncodedUrl());

        var guard = new SignedUrlGuard(_options.Value.Secret);
        var result = guard.ValidateSignedUrl(uri, remoteAddress!);

        if (result.IsFailure)
        {
            return Results.Text(result.Error!.Message, statusCode: (int)HttpStatusCode.Forbidden);
        }

        return await next(context);
    }
}
#endif
