using System.Security.Claims;

namespace MauiAuth.Core.Models;

/// <summary>
/// Represents the current authentication state for the application.
/// </summary>
/// <param name="User">The current user principal.</param>
/// <param name="AccessToken">The persisted access token, when available.</param>
/// <param name="ExpiresAtUtc">The access token expiration time in UTC, when known.</param>
public sealed record AuthenticationState(
    ClaimsPrincipal User,
    string? AccessToken = null,
    DateTimeOffset? ExpiresAtUtc = null)
{
    /// <summary>
    /// Gets an unauthenticated state.
    /// </summary>
    public static AuthenticationState Anonymous { get; } = new(new ClaimsPrincipal(new ClaimsIdentity()));

    /// <summary>
    /// Gets a value indicating whether the user has an authenticated identity.
    /// </summary>
    public bool IsAuthenticated => User.Identity?.IsAuthenticated == true;

    /// <summary>
    /// Gets a value indicating whether the state has an expired token.
    /// </summary>
    public bool IsExpired => ExpiresAtUtc.HasValue && ExpiresAtUtc.Value <= DateTimeOffset.UtcNow;
}
