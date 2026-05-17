namespace MauiAuth.Storage.Abstractions;

/// <summary>
/// Minimal adapter over platform secure storage.
/// </summary>
public interface ISecureStorage
{
    /// <summary>
    /// Gets a value from secure storage.
    /// </summary>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Saves a value to secure storage.
    /// </summary>
    Task SetAsync(string key, string value);

    /// <summary>
    /// Removes a value from secure storage.
    /// </summary>
    bool Remove(string key);
}
