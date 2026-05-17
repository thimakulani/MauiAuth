using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Models;

namespace MauiAuth.Storage.Services;

/// <summary>
/// In-memory token store intended for tests and transient desktop scenarios.
/// </summary>
public sealed class InMemoryTokenStore : ITokenStore
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private StoredToken? _token;

    /// <inheritdoc />
    public async Task<StoredToken?> GetAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return _token;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(StoredToken token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _token = token;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <inheritdoc />
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            _token = null;
        }
        finally
        {
            _gate.Release();
        }
    }
}
