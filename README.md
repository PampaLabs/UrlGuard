# UrlGuard

[![CI](https://github.com/PampaLabs/UrlGuard/actions/workflows/ci.yml/badge.svg)](https://github.com/PampaLabs/UrlGuard/actions/workflows/ci.yml)
[![Downloads](https://img.shields.io/nuget/dt/UrlGuard)](https://www.nuget.org/stats/packages/UrlGuard?groupby=Version)
[![NuGet](https://img.shields.io/nuget/v/UrlGuard)](https://www.nuget.org/packages/UrlGuard/)

The `UrlGuard` library provides functionality to generate and validate signed URLs. It ensures that URLs are protected and can only be accessed by authorized remote address within a specified timeframe.

## Get Started

`UrlGuard` and `UrlGuard.AspNet` can be installed using the Nuget package manager or the dotnet CLI.

```bash
dotnet add package UrlGuard
```

```bash
dotnet add package UrlGuard.AspNet
```

## Usage

### Generating a Signed URL

```csharp
using UrlGuard;

var secretKey = "your_secret_key_here";
var guard = new SignedUrlGuard(secretKey);

var resourceUri = new Uri("https://example.com/resource");
var expirationTime = TimeSpan.FromMinutes(6);
var remoteIpAddress = IPAddress.Parse("192.168.1.1");

var signedUrl = guard.GenerateSignedUrl(resourceUri, expirationTime, remoteIpAddress);
Console.WriteLine($"Signed URL: {signedUrl}");
```

### Validating a Signed URL

```csharp
using UrlGuard;

var secretKey = "your_secret_key_here";
var guard = new SignedUrlGuard(secretKey);

var signedUrl = new Uri("https://example.com/resource?sig=8EA0CFAA3E13D0CAC05D32EA07C1B4D4&exp=1719184714");
var validationResult = guard.ValidateSignedUrl(signedUrl, remoteIpAddress);

if (validationResult.IsSuccess)
{
    Console.WriteLine("URL is valid.");
}
else
{
    Console.WriteLine($"Validation failed: {validationResult.Error.Message}");
}
```

## ASP.NET

Secure your ASP.NET application using out-of-the-box valiadtion filters for MVC/API controllers and minimal API.

### Configuration

Configure `SignedUrlOptions` in your application startup (`Startup.cs`):

```csharp
builder.Services.Configure<SignedUrlOptions>(options => options.Secret = "your_secret_key_here");
```

### Using controllers

Apply `SignedUrl` _attribute_ to controllers or methods.

```csharp
[ApiController]
[Route("api/[controller]")]
public class GreetingController : ControllerBase
{
    [SignedUrl]
    [HttpGet]
    public IActionResult Get() => Ok("Hello World!");
}
```

### Using minimal API

Apply `AddSignedUrlEndpointFilter` to endpoint convention builders.

```csharp
app.MapGet("/", () => "Hello World!").AddSignedUrlEndpointFilter();
```
