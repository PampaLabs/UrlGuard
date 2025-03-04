namespace UrlGuard;

/// <summary>
/// Represents the result of validating a signed URL.
/// </summary>
public class SignedUrlValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsSuccess => !IsFailure;

    /// <summary>
    /// Gets a value indicating whether the validation failed.
    /// </summary>
    public bool IsFailure { get; }

    /// <summary>
    /// Gets the validation error when the validation fails, or null if validation is successful.
    /// </summary>
    public SignedUrlValidationError? Error { get; }

    /// <summary>
    /// Private constructor to initialize a successful validation result.
    /// </summary>
    private SignedUrlValidationResult()
    {
        IsFailure = false;
        Error = default;
    }

    /// <summary>
    /// Private constructor to initialize a failed validation result with an error.
    /// </summary>
    /// <param name="error">The validation error.</param>
    private SignedUrlValidationResult(SignedUrlValidationError error)
    {
        IsFailure = true;
        Error = error;
    }

    /// <summary>
    /// Creates a new instance of successful validation result.
    /// </summary>
    /// <returns>A new instance of <see cref="SignedUrlValidationResult"/> representing successful validation.</returns>
    public static SignedUrlValidationResult Success() => new();

    /// <summary>
    /// Creates a new instance of failed validation result with the specified error.
    /// </summary>
    /// <param name="error">The validation error.</param>
    /// <returns>A new instance of <see cref="SignedUrlValidationResult"/> representing failed validation.</returns>
    public static SignedUrlValidationResult Fail(SignedUrlValidationError error) => new(error);
}
