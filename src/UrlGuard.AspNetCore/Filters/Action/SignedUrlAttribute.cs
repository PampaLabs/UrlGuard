using Microsoft.AspNetCore.Mvc;

namespace UrlGuard;

/// <summary>
/// Attribute for applying signed URL validation to controller classes or methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SignedUrlAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignedUrlAttribute"/> class.
    /// </summary>
    public SignedUrlAttribute() : base(typeof(SignedUrlActionFilter))
    {
    }
}
