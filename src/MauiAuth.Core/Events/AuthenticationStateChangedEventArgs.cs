using MauiAuth.Core.Models;

namespace MauiAuth.Core.Events;

/// <summary>
/// Provides data for authentication state changes.
/// </summary>
public sealed class AuthenticationStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationStateChangedEventArgs"/> class.
    /// </summary>
    public AuthenticationStateChangedEventArgs(AuthenticationState previousState, AuthenticationState currentState)
    {
        PreviousState = previousState;
        CurrentState = currentState;
    }

    /// <summary>
    /// Gets the previous authentication state.
    /// </summary>
    public AuthenticationState PreviousState { get; }

    /// <summary>
    /// Gets the current authentication state.
    /// </summary>
    public AuthenticationState CurrentState { get; }
}
