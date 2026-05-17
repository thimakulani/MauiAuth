namespace MauiAuth.Core.Options;

/// <summary>
/// Configures authentication state behavior.
/// </summary>
public sealed class AuthenticationOptions
{
    /// <summary>
    /// Gets or sets whether expired tokens should be removed when a session is restored.
    /// </summary>
    public bool ClearExpiredTokenOnRestore { get; set; } = true;

    /// <summary>
    /// Gets or sets the authentication type used for identities created from tokens.
    /// </summary>
    public string AuthenticationType { get; set; } = "Bearer";

    /// <summary>
    /// Gets or sets the claim type used for role checks.
    /// </summary>
    public string RoleClaimType { get; set; } = System.Security.Claims.ClaimTypes.Role;

    /// <summary>
    /// Gets or sets the claim type used for user names.
    /// </summary>
    public string NameClaimType { get; set; } = System.Security.Claims.ClaimTypes.Name;
}
