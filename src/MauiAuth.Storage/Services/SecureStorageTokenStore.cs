using System.Globalization;
using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Models;
using MauiAuth.Storage.Abstractions;

namespace MauiAuth.Storage.Services;

/// <summary>
/// Persists tokens using an <see cref="ISecureStorage"/> adapter.
/// </summary>
public sealed class SecureStorageTokenStore : ITokenStore
{
    private const string DefaultAccessTokenKey = "mauiauth.access_token";
    private const string DefaultExpiresAtKey = "mauiauth.expires_at_utc";
    private readonly ISecureStorage _secureStorage;
    private readonly string _accessTokenKey;
    private readonly string _expiresAtKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecureStorageTokenStore"/> class.
    /// </summary>
    public SecureStorageTokenStore(
        ISecureStorage secureStorage,
        string accessTokenKey = DefaultAccessTokenKey,
        string expiresAtKey = DefaultExpiresAtKey)
    {
        _secureStorage = secureStorage;
        _accessTokenKey = accessTokenKey;
        _expiresAtKey = expiresAtKey;
    }

    /// <inheritdoc />
    public async Task<StoredToken?> GetAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var accessToken = await _secureStorage.GetAsync(_accessTokenKey).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var expiresAtText = await _secureStorage.GetAsync(_expiresAtKey).ConfigureAwait(false);
        var expiresAtUtc = DateTimeOffset.TryParse(
            expiresAtText,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsed)
            ? parsed
            : (DateTimeOffset?)null;

        return new StoredToken(accessToken, expiresAtUtc);
    }

    /// <inheritdoc />
    public async Task SaveAsync(StoredToken token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        cancellationToken.ThrowIfCancellationRequested();
        await _secureStorage.SetAsync(_accessTokenKey, token.AccessToken).ConfigureAwait(false);

        if (token.ExpiresAtUtc.HasValue)
        {
            await _secureStorage.SetAsync(_expiresAtKey, token.ExpiresAtUtc.Value.UtcDateTime.ToString("O", CultureInfo.InvariantCulture))
                .ConfigureAwait(false);
        }
        else
        {
            _secureStorage.Remove(_expiresAtKey);
        }
    }

    /// <inheritdoc />
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _secureStorage.Remove(_accessTokenKey);
        _secureStorage.Remove(_expiresAtKey);
        return Task.CompletedTask;
    }
}
