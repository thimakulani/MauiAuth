using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MauiAuth.Core.Interfaces;
using MauiAuth.Core.Options;
using MauiAuth.Jwt.Models;

namespace MauiAuth.Jwt.Services;

/// <summary>
/// Parses JWT access tokens and creates claims principals.
/// </summary>
public sealed class JwtParser : ITokenPrincipalFactory
{
    private readonly AuthenticationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtParser"/> class.
    /// </summary>
    public JwtParser(AuthenticationOptions? options = null)
    {
        _options = options ?? new AuthenticationOptions();
    }

    /// <summary>
    /// Parses a JWT access token.
    /// </summary>
    public JwtToken Parse(string accessToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        var parts = accessToken.Split('.');
        if (parts.Length < 2)
        {
            throw new FormatException("The token is not a valid JWT.");
        }

        var header = ParseJsonObject(parts[0]);
        var payload = ParseJsonObject(parts[1]);
        var claims = CreateClaims(payload);
        var expiresAtUtc = GetExpiresAtUtc(payload);

        return new JwtToken(header, payload, claims, expiresAtUtc);
    }

    /// <inheritdoc />
    public ClaimsPrincipal CreatePrincipal(string accessToken)
    {
        var token = Parse(accessToken);
        var identity = new ClaimsIdentity(token.Claims, _options.AuthenticationType, _options.NameClaimType, _options.RoleClaimType);
        return new ClaimsPrincipal(identity);
    }

    /// <inheritdoc />
    public DateTimeOffset? GetExpiresAtUtc(string accessToken)
    {
        var token = Parse(accessToken);
        return token.ExpiresAtUtc;
    }

    private static IReadOnlyDictionary<string, object?> ParseJsonObject(string base64Url)
    {
        var json = Encoding.UTF8.GetString(Base64UrlDecode(base64Url));
        using var document = JsonDocument.Parse(json);
        return ReadObject(document.RootElement);
    }

    private IReadOnlyList<Claim> CreateClaims(IReadOnlyDictionary<string, object?> payload)
    {
        var claims = new List<Claim>();
        foreach (var (key, value) in payload)
        {
            AddClaim(claims, key, value);
        }

        MapWellKnownClaim(claims, "sub", ClaimTypes.NameIdentifier);
        MapWellKnownClaim(claims, "name", _options.NameClaimType);
        MapWellKnownClaim(claims, "email", ClaimTypes.Email);
        MapRoleClaims(claims);

        return claims;
    }

    private static void AddClaim(ICollection<Claim> claims, string type, object? value)
    {
        switch (value)
        {
            case null:
                break;
            case JsonElement element when element.ValueKind == JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    AddClaim(claims, type, ReadElement(item));
                }
                break;
            case IReadOnlyList<object?> items:
                foreach (var item in items)
                {
                    AddClaim(claims, type, item);
                }
                break;
            default:
                claims.Add(new Claim(type, Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty));
                break;
        }
    }

    private static void MapWellKnownClaim(ICollection<Claim> claims, string sourceType, string targetType)
    {
        if (sourceType == targetType)
        {
            return;
        }

        foreach (var claim in claims.Where(claim => claim.Type == sourceType).ToArray())
        {
            claims.Add(new Claim(targetType, claim.Value));
        }
    }

    private void MapRoleClaims(ICollection<Claim> claims)
    {
        foreach (var claim in claims.Where(claim => claim.Type is "role" or "roles").ToArray())
        {
            if (claim.Type != _options.RoleClaimType)
            {
                claims.Add(new Claim(_options.RoleClaimType, claim.Value));
            }
        }
    }

    private static DateTimeOffset? GetExpiresAtUtc(IReadOnlyDictionary<string, object?> payload)
    {
        if (!payload.TryGetValue("exp", out var value))
        {
            return null;
        }

        return value switch
        {
            long seconds => DateTimeOffset.FromUnixTimeSeconds(seconds),
            int seconds => DateTimeOffset.FromUnixTimeSeconds(seconds),
            double seconds => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(seconds)),
            string text when long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var seconds) =>
                DateTimeOffset.FromUnixTimeSeconds(seconds),
            _ => null
        };
    }

    private static IReadOnlyDictionary<string, object?> ReadObject(JsonElement element)
    {
        var result = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var property in element.EnumerateObject())
        {
            result[property.Name] = ReadElement(property.Value);
        }

        return result;
    }

    private static object? ReadElement(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var number) => number,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Array => element.EnumerateArray().Select(ReadElement).ToArray(),
            JsonValueKind.Object => ReadObject(element),
            _ => null
        };

    private static byte[] Base64UrlDecode(string value)
    {
        var base64 = value.Replace('-', '+').Replace('_', '/');
        var padding = base64.Length % 4;
        if (padding > 0)
        {
            base64 = base64.PadRight(base64.Length + 4 - padding, '=');
        }

        return Convert.FromBase64String(base64);
    }
}
