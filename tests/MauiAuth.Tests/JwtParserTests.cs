using System.Text;
using System.Text.Json;
using MauiAuth.Jwt.Services;

namespace MauiAuth.Tests;

public sealed class JwtParserTests
{
    [Fact]
    public void CreatePrincipal_maps_standard_claims_and_roles()
    {
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1);
        var token = JwtTokenBuilder.Create(new
        {
            sub = "user-123",
            name = "Ada",
            email = "ada@example.com",
            roles = new[] { "Admin", "User" },
            exp = expiresAt.ToUnixTimeSeconds()
        });

        var principal = new JwtParser().CreatePrincipal(token);

        Assert.True(principal.Identity?.IsAuthenticated);
        Assert.Equal("Ada", principal.Identity?.Name);
        Assert.True(principal.IsInRole("Admin"));
        Assert.Equal("ada@example.com", principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
    }

    [Fact]
    public void GetExpiresAtUtc_reads_exp_claim()
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds();
        var token = JwtTokenBuilder.Create(new { exp = expiresAt });

        var parsed = new JwtParser().GetExpiresAtUtc(token);

        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(expiresAt), parsed);
    }

    private static class JwtTokenBuilder
    {
        public static string Create(object payload)
        {
            var header = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new { alg = "none", typ = "JWT" }));
            var body = Base64Url(JsonSerializer.SerializeToUtf8Bytes(payload));
            return $"{header}.{body}.";
        }

        private static string Base64Url(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
