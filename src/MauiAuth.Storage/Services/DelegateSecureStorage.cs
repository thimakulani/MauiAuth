using MauiAuth.Storage.Abstractions;

namespace MauiAuth.Storage.Services;

/// <summary>
/// Secure storage adapter backed by delegates, useful for binding to Microsoft.Maui.Storage.SecureStorage.Default.
/// </summary>
public sealed class DelegateSecureStorage : ISecureStorage
{
    private readonly Func<string, Task<string?>> _getAsync;
    private readonly Func<string, string, Task> _setAsync;
    private readonly Func<string, bool> _remove;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateSecureStorage"/> class.
    /// </summary>
    public DelegateSecureStorage(
        Func<string, Task<string?>> getAsync,
        Func<string, string, Task> setAsync,
        Func<string, bool> remove)
    {
        _getAsync = getAsync;
        _setAsync = setAsync;
        _remove = remove;
    }

    /// <inheritdoc />
    public Task<string?> GetAsync(string key) => _getAsync(key);

    /// <inheritdoc />
    public Task SetAsync(string key, string value) => _setAsync(key, value);

    /// <inheritdoc />
    public bool Remove(string key) => _remove(key);
}
