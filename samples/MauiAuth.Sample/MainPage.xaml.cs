using System.Security.Claims;
using System.Text.Json;
using MauiAuth.Core.Events;
using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Models;

namespace MauiAuth.Sample;

public partial class MainPage : ContentPage
{
    private readonly IAuthenticationStateProvider _auth;
    private bool _restored;

    public MainPage(IAuthenticationStateProvider auth)
    {
        _auth = auth;
        InitializeComponent();
        _auth.AuthenticationStateChanged += OnAuthenticationStateChanged;
        Render(_auth.State, "Ready");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_restored)
        {
            return;
        }

        _restored = true;
        var state = await _auth.RestoreSessionAsync();
        Render(state, state.IsAuthenticated ? "Session restored" : "No saved session");
    }

    private void OnUseDemoTokenClicked(object? sender, EventArgs e)
    {
        TokenEntry.Text = CreateDemoJwt();
    }

    private async void OnSignInClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TokenEntry.Text))
        {
            EventLabel.Text = "Paste a JWT or use the demo token.";
            return;
        }

        try
        {
            await _auth.SignInAsync(TokenEntry.Text);
        }
        catch (Exception exception)
        {
            EventLabel.Text = $"Sign-in failed: {exception.Message}";
        }
    }

    private async void OnSignOutClicked(object? sender, EventArgs e)
    {
        await _auth.SignOutAsync();
    }

    private void OnAuthenticationStateChanged(object? sender, AuthenticationStateChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var message = e.CurrentState.IsAuthenticated ? "Signed in" : "Signed out";
            Render(e.CurrentState, message);
        });
    }

    private void Render(AuthenticationState state, string eventMessage)
    {
        StatusLabel.Text = state.IsAuthenticated ? "Signed in" : "Signed out";
        NameLabel.Text = state.User.Identity?.Name ?? "No user restored";
        UserIdLabel.Text = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-";
        RolesLabel.Text = string.Join(", ", state.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value));
        if (string.IsNullOrWhiteSpace(RolesLabel.Text))
        {
            RolesLabel.Text = "-";
        }

        ExpiresLabel.Text = state.ExpiresAtUtc?.LocalDateTime.ToString("g") ?? "-";
        EventLabel.Text = $"{eventMessage} at {DateTime.Now:T}";
    }

    private static string CreateDemoJwt()
    {
        var expiresAt = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
        var payload = new
        {
            sub = "demo-user-001",
            name = "Demo User",
            email = "demo@mauiauth.local",
            roles = new[] { "Admin", "User" },
            exp = expiresAt
        };

        return $"{Base64Url(new { alg = "none", typ = "JWT" })}.{Base64Url(payload)}.";
    }

    private static string Base64Url(object value)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
