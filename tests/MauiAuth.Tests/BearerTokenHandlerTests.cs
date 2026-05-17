using System.Net;
using MauiAuth.Core.Interfaces;
using MauiAuth.Http.Handlers;

namespace MauiAuth.Tests;

public sealed class BearerTokenHandlerTests
{
    [Fact]
    public async Task SendAsync_adds_bearer_token_when_available()
    {
        var inner = new CaptureHandler();
        var handler = new BearerTokenHandler(new StaticAccessTokenProvider("abc123"))
        {
            InnerHandler = inner
        };

        using var client = new HttpClient(handler);
        await client.GetAsync("https://example.com");

        Assert.Equal("Bearer", inner.Request?.Headers.Authorization?.Scheme);
        Assert.Equal("abc123", inner.Request?.Headers.Authorization?.Parameter);
    }

    private sealed class StaticAccessTokenProvider : IAccessTokenProvider
    {
        private readonly string? _token;

        public StaticAccessTokenProvider(string? token)
        {
            _token = token;
        }

        public Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default) => Task.FromResult(_token);
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public HttpRequestMessage? Request { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
