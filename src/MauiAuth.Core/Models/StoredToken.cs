namespace MauiAuth.Core.Models;

/// <summary>
/// Represents token material persisted by a token store.
/// </summary>
/// <param name="AccessToken">The bearer access token.</param>
/// <param name="ExpiresAtUtc">The token expiration time in UTC, when known.</param>
public sealed record StoredToken(string AccessToken, DateTimeOffset? ExpiresAtUtc = null);
