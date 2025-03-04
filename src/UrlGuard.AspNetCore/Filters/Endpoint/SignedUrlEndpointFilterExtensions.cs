#if NET7_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace UrlGuard;

/// <summary>
/// Extension methods for adding the signed URL endpoint filter to endpoint convention builders.
/// </summary>
public static class SignedUrlEndpointFilterExtensions
{
    /// <summary>
    /// Adds the signed URL endpoint filter to the specified endpoint convention builder.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the endpoint convention builder.</typeparam>
    /// <param name="builder">The endpoint convention builder to add the filter to.</param>
    /// <returns>The same endpoint convention builder instance with the filter added.</returns>
    public static TBuilder AddSignedUrlEndpointFilter<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        return builder.AddEndpointFilter<TBuilder, SignedUrlEndpointFilter>();
    }
}
#endif
