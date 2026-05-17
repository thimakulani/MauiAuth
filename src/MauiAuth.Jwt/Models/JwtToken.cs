using System.Security.Claims;

namespace MauiAuth.Jwt.Models;

/// <summary>
/// Represents a parsed JSON Web Token.
/// </summary>
public sealed record JwtToken(
    IReadOnlyDictionary<string, object?> Header,
    IReadOnlyDictionary<string, object?> Payload,
    IReadOnlyList<Claim> Claims,
    DateTimeOffset? ExpiresAtUtc);
