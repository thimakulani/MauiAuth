using System.Security.Claims;

namespace MauiAuth.Core.Services;

/// <summary>
/// Minimal fallback token principal factory for non-JWT scenarios.
/// </summary>
public sealed class SystemTextTokenPrincipalFactory : MauiAuth.Core.Interfaces.ITokenPrincipalFactory
{
    /// <inheritdoc />
    public ClaimsPrincipal CreatePrincipal(string accessToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        return new ClaimsPrincipal(new ClaimsIdentity(authenticationType: "Bearer"));
    }

    /// <inheritdoc />
    public DateTimeOffset? GetExpiresAtUtc(string accessToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        return null;
    }
}
