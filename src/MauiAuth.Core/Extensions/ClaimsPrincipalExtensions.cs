using System.Security.Claims;

namespace MauiAuth.Core.Extensions;

/// <summary>
/// Authorization helpers for claims principals.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Determines whether the principal contains a claim.
    /// </summary>
    public static bool HasClaimValue(this ClaimsPrincipal principal, string type, string value) =>
        principal.HasClaim(claim => claim.Type == type && string.Equals(claim.Value, value, StringComparison.Ordinal));

    /// <summary>
    /// Determines whether the principal is in any of the supplied roles.
    /// </summary>
    public static bool IsInAnyRole(this ClaimsPrincipal principal, params string[] roles) =>
        roles.Any(principal.IsInRole);
}
