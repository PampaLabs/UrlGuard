using System.Net;

using Microsoft.Extensions.Time.Testing;

namespace UrlGuard.Test;

public class SignedUrlGuardTests
{
    [Fact]
    public void Should_ValidateSignedUrl_WithinExpiryTime()
    {
        var timeProvider = new FakeTimeProvider();

        var guard = new SignedUrlGuard("MySecret");
        guard.UseTimeProvider(timeProvider);

        // var url = new Uri("https://example.test/static/sensitive-info.pdf");
        var url = new Uri("/static/sensitive-info.pdf", UriKind.Relative);
        var signedUrl = guard.GenerateSignedUrl(url, TimeSpan.FromMinutes(5), IPAddress.Any);

        timeProvider.Advance(TimeSpan.FromMinutes(4));
        guard.ValidateSignedUrl(signedUrl, IPAddress.Any).IsSuccess.Should().BeTrue();

        timeProvider.Advance(TimeSpan.FromMinutes(2));
        guard.ValidateSignedUrl(signedUrl, IPAddress.Any).IsSuccess.Should().BeFalse();
    }
}