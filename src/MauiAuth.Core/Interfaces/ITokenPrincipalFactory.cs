using System.Security.Claims;

namespace MauiAuth.Core.Interfaces;

/// <summary>
/// Creates a principal and expiration metadata from an access token.
/// </summary>
public interface ITokenPrincipalFactory
{
    /// <summary>
    /// Creates a claims principal from an access token.
    /// </summary>
    ClaimsPrincipal CreatePrincipal(string accessToken);

    /// <summary>
    /// Gets the token expiration time, when available.
    /// </summary>
    DateTimeOffset? GetExpiresAtUtc(string accessToken);
}
