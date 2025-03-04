namespace UrlGuard;

/// <summary>
/// Options for configuring signed URL generation and validation.
/// </summary>
public class SignedUrlOptions
{
    /// <summary>
    /// Gets or sets the secret key used for generating and validating signed URLs.
    /// </summary>
    public string Secret { get; set; } = default!;
}
