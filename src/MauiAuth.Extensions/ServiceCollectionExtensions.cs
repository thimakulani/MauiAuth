using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Options;
using MauiAuth.Core.Services;
using MauiAuth.Http.Handlers;
using MauiAuth.Jwt.Services;
using MauiAuth.Storage.Abstractions;
using MauiAuth.Storage.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MauiAuth.Extensions;

/// <summary>
/// Dependency injection helpers for MauiAuth.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MauiAuth using JWT parsing and in-memory token storage.
    /// Replace the token store with <see cref="AddMauiAuthSecureStorage"/> in MAUI applications.
    /// </summary>
    public static IServiceCollection AddMauiAuth(
        this IServiceCollection services,
        Action<AuthenticationOptions>? configure = null)
    {
        var options = new AuthenticationOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<ITokenStore, InMemoryTokenStore>();
        services.AddSingleton<ITokenPrincipalFactory, JwtParser>();
        services.AddSingleton<AuthenticationStateProvider>();
        services.AddSingleton<IAuthenticationStateProvider>(provider => provider.GetRequiredService<AuthenticationStateProvider>());
        services.AddSingleton<IAccessTokenProvider>(provider => provider.GetRequiredService<AuthenticationStateProvider>());
        services.AddTransient<BearerTokenHandler>();

        return services;
    }

    /// <summary>
    /// Replaces the token store with secure storage backed by the supplied adapter.
    /// </summary>
    public static IServiceCollection AddMauiAuthSecureStorage(
        this IServiceCollection services,
        Func<IServiceProvider, ISecureStorage> secureStorageFactory)
    {
        ArgumentNullException.ThrowIfNull(secureStorageFactory);

        services.AddSingleton(secureStorageFactory);
        services.AddSingleton<ITokenStore, SecureStorageTokenStore>();
        return services;
    }

    /// <summary>
    /// Replaces the token store with the supplied implementation.
    /// </summary>
    public static IServiceCollection AddMauiAuthTokenStore<TTokenStore>(this IServiceCollection services)
        where TTokenStore : class, ITokenStore
    {
        services.AddSingleton<ITokenStore, TTokenStore>();
        return services;
    }
}
