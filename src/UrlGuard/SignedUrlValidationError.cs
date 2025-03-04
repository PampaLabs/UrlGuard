namespace UrlGuard;

/// <summary>
/// Represents an error that occurred during signed URL validation.
/// </summary>
public class SignedUrlValidationError
{
    /// <summary>
    /// Gets the error message describing the validation error.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignedUrlValidationError"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public SignedUrlValidationError(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Implicit conversion operator from string to <see cref="SignedUrlValidationError"/>.
    /// Allows creating a new instance of <see cref="SignedUrlValidationError"/> using a string message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new instance of <see cref="SignedUrlValidationError"/> with the specified message.</returns>
    public static implicit operator SignedUrlValidationError(string message) => new(message);
}
