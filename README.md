# MauiAuth

MauiAuth is a lightweight authentication-state framework for .NET MAUI inspired by Blazor's `AuthenticationStateProvider`.

## Architecture

- `MauiAuth.Core`: platform-neutral state provider, events, token store contracts, options, and authorization helpers.
- `MauiAuth.Jwt`: JWT parsing and `ClaimsPrincipal` creation.
- `MauiAuth.Storage`: token store implementations and secure-storage adapter contracts.
- `MauiAuth.Http`: `DelegatingHandler` for bearer token injection.
- `MauiAuth.Extensions`: dependency injection setup for app developers.

`MauiAuth.Core` targets `net8.0` and does not reference MAUI.

## Basic Usage

```csharp
using MauiAuth.Core.Interfaces;
using MauiAuth.Extensions.Extensions;

builder.Services.AddMauiAuth();
```

```csharp
public sealed class LoginViewModel
{
    private readonly IAuthenticationStateProvider _auth;

    public LoginViewModel(IAuthenticationStateProvider auth)
    {
        _auth = auth;
        _auth.AuthenticationStateChanged += (_, args) =>
        {
            var isAuthenticated = args.CurrentState.IsAuthenticated;
        };
    }

    public Task SignInAsync(string jwt) => _auth.SignInAsync(jwt);

    public Task SignOutAsync() => _auth.SignOutAsync();
}
```

```csharp
if (auth.IsAuthenticated)
{
    var user = auth.User;
}
```

## MAUI SecureStorage Adapter

Bind `MauiAuth.Storage` to MAUI SecureStorage from your app project:

```csharp
using MauiAuth.Extensions.Extensions;
using MauiAuth.Storage.Services;
using Microsoft.Maui.Storage;

builder.Services
    .AddMauiAuth()
    .AddMauiAuthSecureStorage(_ => new DelegateSecureStorage(
        SecureStorage.Default.GetAsync,
        SecureStorage.Default.SetAsync,
        SecureStorage.Default.Remove));
```

## HttpClient Integration

```csharp
using MauiAuth.Http.Handlers;

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
})
.AddHttpMessageHandler<BearerTokenHandler>();
```

## Session Restoration

Call session restore when the app starts:

```csharp
var auth = services.GetRequiredService<IAuthenticationStateProvider>();
await auth.RestoreSessionAsync();
```

## Sample App

See [samples/MauiAuth.Sample](samples/MauiAuth.Sample) for a runnable MAUI app that demonstrates:

- dependency injection setup
- MAUI `SecureStorage` token persistence
- session restoration
- sign in with a demo JWT
- sign out
- `AuthenticationStateChanged` UI updates
- claims and role display

Build the Windows target:

```powershell
dotnet build samples/MauiAuth.Sample/MauiAuth.Sample.csproj -f net9.0-windows10.0.19041.0
```

## Packaging

Each project includes NuGet metadata. Before publishing, replace `RepositoryUrl`, add icon/readme metadata, and sign packages if your organization requires it.

```powershell
dotnet pack -c Release
```
