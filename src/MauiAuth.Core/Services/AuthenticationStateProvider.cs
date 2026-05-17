using System.Security.Claims;
using MauiAuth.Core.Events;
using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Models;
using MauiAuth.Core.Options;

namespace MauiAuth.Core.Services;

/// <summary>
/// Default authentication state provider implementation.
/// </summary>
public class AuthenticationStateProvider : IAuthenticationStateProvider, IAccessTokenProvider
{
    private readonly ITokenStore _tokenStore;
    private readonly ITokenPrincipalFactory _principalFactory;
    private readonly AuthenticationOptions _options;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private AuthenticationState _state = AuthenticationState.Anonymous;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationStateProvider"/> class.
    /// </summary>
    public AuthenticationStateProvider(
        ITokenStore tokenStore,
        ITokenPrincipalFactory principalFactory,
        AuthenticationOptions? options = null)
    {
        _tokenStore = tokenStore;
        _principalFactory = principalFactory;
        _options = options ?? new AuthenticationOptions();
    }

    /// <inheritdoc />
    public event EventHandler<AuthenticationStateChangedEventArgs>? AuthenticationStateChanged;

    /// <inheritdoc />
    public AuthenticationState State => _state;

    /// <inheritdoc />
    public ClaimsPrincipal User => _state.User;

    /// <inheritdoc />
    public bool IsAuthenticated => _state.IsAuthenticated && !_state.IsExpired;

    /// <inheritdoc />
    public Task<AuthenticationState> GetAuthenticationStateAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_state);

    /// <inheritdoc />
    public async Task<AuthenticationState> SignInAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        var expiresAtUtc = _principalFactory.GetExpiresAtUtc(accessToken);
        var principal = _principalFactory.CreatePrincipal(accessToken);
        var next = new AuthenticationState(principal, accessToken, expiresAtUtc);

        await _tokenStore.SaveAsync(new StoredToken(accessToken, expiresAtUtc), cancellationToken).ConfigureAwait(false);
        await SetStateAsync(next, cancellationToken).ConfigureAwait(false);
        return next;
    }

    /// <inheritdoc />
    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await _tokenStore.ClearAsync(cancellationToken).ConfigureAwait(false);
        await SetStateAsync(AuthenticationState.Anonymous, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<AuthenticationState> RestoreSessionAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenStore.GetAsync(cancellationToken).ConfigureAwait(false);
        if (token is null)
        {
            await SetStateAsync(AuthenticationState.Anonymous, cancellationToken).ConfigureAwait(false);
            return _state;
        }

        var expiresAtUtc = token.ExpiresAtUtc ?? _principalFactory.GetExpiresAtUtc(token.AccessToken);
        if (_options.ClearExpiredTokenOnRestore && expiresAtUtc.HasValue && expiresAtUtc.Value <= DateTimeOffset.UtcNow)
        {
            await SignOutAsync(cancellationToken).ConfigureAwait(false);
            return _state;
        }

        var principal = _principalFactory.CreatePrincipal(token.AccessToken);
        var next = new AuthenticationState(principal, token.AccessToken, expiresAtUtc);
        await SetStateAsync(next, cancellationToken).ConfigureAwait(false);
        return next;
    }

    /// <inheritdoc />
    public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var token = IsAuthenticated ? _state.AccessToken : null;
        return Task.FromResult(token);
    }

    private async Task SetStateAsync(AuthenticationState next, CancellationToken cancellationToken)
    {
        AuthenticationState previous;
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            previous = _state;
            _state = next;
        }
        finally
        {
            _gate.Release();
        }

        AuthenticationStateChanged?.Invoke(this, new AuthenticationStateChangedEventArgs(previous, next));
    }
}
