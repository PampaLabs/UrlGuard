using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace UrlGuard;

/// <summary>
/// Generates and validates signed URLs using a secret key.
/// </summary>
public class SignedUrlGuard
{
    private readonly string _secret;

    private TimeProvider _timeProvider = TimeProvider.System;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignedUrlGuard"/> class with the specified secret key.
    /// </summary>
    /// <param name="secret">The secret key used for generating and validating signed URLs.</param>
    public SignedUrlGuard(string secret)
    {
        _secret = secret;
    }

    /// <summary>
    /// Sets the time provider for obtaining the current time in UTC.
    /// </summary>
    /// <param name="timeProvider">The time provider to use.</param>
    public void UseTimeProvider(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Computes the signature for the signed URL based on the URI, expiration time, and remote address.
    /// </summary>
    /// <param name="uri">The URI of the resource.</param>
    /// <param name="expiration">The expiration date and time of the signed URL.</param>
    /// <param name="remoteAddress">The remote IP address of the client.</param>
    /// <returns>The computed signature as a hexadecimal string.</returns>
    private string ComputeSignature(Uri uri, DateTimeOffset expiration, IPAddress remoteAddress)
    {
        const string expression = "{path}:{expiration}:{remote_addr}";

        var content = expression
            .Replace("{path}", uri.AbsolutePath)
            .Replace("{expiration}", expiration.ToUnixTimeSeconds().ToString())
            .Replace("{remote_addr}", remoteAddress.ToString());

        var key = Encoding.UTF8.GetBytes(_secret);
        var buffer = Encoding.UTF8.GetBytes(content);

        using var hmac = new HMACMD5(key);
        var hash = hmac.ComputeHash(buffer);

        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    /// <summary>
    /// Generates a signed URL with signature and expiration timestamp.
    /// </summary>
    /// <param name="uri">The URI of the resource to be signed.</param>
    /// <param name="expiresOn">The time span until the URL expires.</param>
    /// <param name="remoteAddress">The remote IP address of the client.</param>
    /// <returns>The signed URL as a <see cref="Uri"/> object.</returns>
    public Uri GenerateSignedUrl(Uri uri, TimeSpan expiresOn, IPAddress remoteAddress)
    {
        var absoluteUri = CreateAbsoluteUri(uri);

        var expiration = GetUtcNow() + expiresOn;
        var epoch = expiration.ToUnixTimeSeconds();

        var signature = ComputeSignature(absoluteUri, expiration, remoteAddress);

        var uriBuilder = new UriBuilder(absoluteUri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query.Set(SignedUrlQueryParams.Sig, signature);
        query.Set(SignedUrlQueryParams.Exp, epoch.ToString());

        uriBuilder.Query = query.ToString();

        return uri.IsAbsoluteUri ? uriBuilder.Uri : new Uri(uriBuilder.Uri.PathAndQuery, UriKind.Relative);
    }

    /// <summary>
    /// Validates the integrity and expiration of a signed URL.
    /// </summary>
    /// <param name="uri">The signed URL to validate.</param>
    /// <param name="remoteAddress">The remote IP address of the client.</param>
    /// <returns>A <see cref="SignedUrlValidationResult"/> indicating whether the validation was successful or failed.</returns>
    public SignedUrlValidationResult ValidateSignedUrl(Uri uri, IPAddress remoteAddress)
    {
        var absoluteUri = CreateAbsoluteUri(uri);

        var query = HttpUtility.ParseQueryString(absoluteUri.Query);

        var sig = query[SignedUrlQueryParams.Sig];
        var expStr = query[SignedUrlQueryParams.Exp];

        if (string.IsNullOrEmpty(sig))
        {
            return SignedUrlValidationResult.Fail("Bad URL hash");
        }

        if (!long.TryParse(expStr, out long exp))
        {
            return SignedUrlValidationResult.Fail("Bad URL timestamp");
        }

        var expiresOn = DateTimeOffset.FromUnixTimeSeconds(exp);

        if (GetUtcNow() > expiresOn)
        {
            return SignedUrlValidationResult.Fail("URL signature expired");
        }

        var computed = ComputeSignature(absoluteUri, expiresOn, remoteAddress);

        if (!string.Equals(computed, sig))
        {
            return SignedUrlValidationResult.Fail("URL signature mismatch");
        }

        return SignedUrlValidationResult.Success();
    }

    /// <summary>
    /// Creates an absolute URI from the given URI, ensuring it is absolute.
    /// </summary>
    /// <param name="uri">The URI to convert to an absolute URI.</param>
    /// <returns>The absolute URI as a <see cref="Uri"/> object.</returns>
    private Uri CreateAbsoluteUri(Uri uri)
    {
        if (uri.IsAbsoluteUri)
        {
            return new UriBuilder(uri).Uri;
        }
        else
        {
            var baseUri = new Uri("https://domain.invalid");
            return new Uri(baseUri, uri);
        }
    }

    /// <summary>
    /// Retrieves the current UTC time, using a custom time provider if set.
    /// </summary>
    /// <returns>The current UTC time as a <see cref="DateTimeOffset"/>.</returns>
    private DateTimeOffset GetUtcNow()
    {
        return _timeProvider.GetUtcNow();
    }
}
