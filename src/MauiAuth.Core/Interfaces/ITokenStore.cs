using MauiAuth.Core.Models;

namespace MauiAuth.Core.Interfaces;

/// <summary>
/// Persists authentication token material.
/// </summary>
public interface ITokenStore
{
    /// <summary>
    /// Gets the persisted token, when one exists.
    /// </summary>
    Task<StoredToken?> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves token material.
    /// </summary>
    Task SaveAsync(StoredToken token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes persisted token material.
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);
}
