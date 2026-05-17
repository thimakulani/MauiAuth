using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Services;
using MauiAuth.Jwt.Services;
using MauiAuth.Storage.Services;

namespace MauiAuth.Tests;

public sealed class AuthenticationStateProviderTests
{
    [Fact]
    public async Task SignInAsync_persists_token_and_raises_change()
    {
        var store = new InMemoryTokenStore();
        var provider = new AuthenticationStateProvider(store, new JwtParser());
        var token = JwtTokenBuilder.Create(new { name = "Grace", exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds() });
        var raised = false;

        provider.AuthenticationStateChanged += (_, args) =>
        {
            raised = true;
            Assert.False(args.PreviousState.IsAuthenticated);
            Assert.True(args.CurrentState.IsAuthenticated);
        };

        var state = await provider.SignInAsync(token);
        var stored = await store.GetAsync();

        Assert.True(raised);
        Assert.True(state.IsAuthenticated);
        Assert.Equal(token, stored?.AccessToken);
    }

    [Fact]
    public async Task RestoreSessionAsync_clears_expired_token()
    {
        var store = new InMemoryTokenStore();
        var provider = new AuthenticationStateProvider(store, new JwtParser());
        var token = JwtTokenBuilder.Create(new { exp = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds() });
        await store.SaveAsync(new(token));

        var state = await provider.RestoreSessionAsync();

        Assert.False(state.IsAuthenticated);
        Assert.Null(await store.GetAsync());
    }

    private static class JwtTokenBuilder
    {
        public static string Create(object payload)
        {
            var header = Base64Url(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(new { alg = "none", typ = "JWT" }));
            var body = Base64Url(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(payload));
            return $"{header}.{body}.";
        }

        private static string Base64Url(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
