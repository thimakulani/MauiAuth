namespace MauiAuth.Core.Interfaces;

/// <summary>
/// Provides the current bearer access token.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Gets the current access token, or null when unavailable.
    /// </summary>
    Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
