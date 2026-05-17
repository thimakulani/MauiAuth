# MauiAuth.Sample

Runnable .NET MAUI sample showing:

- `builder.Services.AddMauiAuth()`
- MAUI `SecureStorage` integration through `DelegateSecureStorage`
- session restoration on startup
- `SignInAsync`
- `SignOutAsync`
- `AuthenticationStateChanged`
- JWT claims and role display

Run on Windows:

```powershell
dotnet build samples/MauiAuth.Sample/MauiAuth.Sample.csproj -f net9.0-windows10.0.19041.0
```

Build mobile targets when the corresponding workloads are installed:

```powershell
dotnet build samples/MauiAuth.Sample/MauiAuth.Sample.csproj -p:IncludeMobileTargets=true
```
