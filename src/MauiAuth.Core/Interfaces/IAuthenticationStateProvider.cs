using System.Security.Claims;
using MauiAuth.Core.Events;
using MauiAuth.Core.Models;

namespace MauiAuth.Core.Interfaces;

/// <summary>
/// Provides and updates the current authentication state.
/// </summary>
public interface IAuthenticationStateProvider
{
    /// <summary>
    /// Occurs when the authentication state changes.
    /// </summary>
    event EventHandler<AuthenticationStateChangedEventArgs>? AuthenticationStateChanged;

    /// <summary>
    /// Gets the current authentication state.
    /// </summary>
    AuthenticationState State { get; }

    /// <summary>
    /// Gets the current user principal.
    /// </summary>
    ClaimsPrincipal User { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the current authentication state.
    /// </summary>
    Task<AuthenticationState> GetAuthenticationStateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs in with the supplied access token.
    /// </summary>
    Task<AuthenticationState> SignInAsync(string accessToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs out and removes persisted token material.
    /// </summary>
    Task SignOutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a persisted authentication session.
    /// </summary>
    Task<AuthenticationState> RestoreSessionAsync(CancellationToken cancellationToken = default);
}
