namespace ServerApp.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using ServerApp.Application.Services;

// Implementation of IYahooAuthService from Application layer

public class YahooAuthService : IYahooAuthService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<YahooAuthService> _logger;
    private readonly IStateStore _stateStore;

    private const string YahooAuthorizationUrl = "https://api.login.yahoo.com/oauth2/request_auth";
    private const string YahooTokenUrl = "https://api.login.yahoo.com/oauth2/get_token";
    private const string YahooProfileUrl = "https://api.login.yahoo.com/openid/v1/userinfo";

    public YahooAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<YahooAuthService> logger, IStateStore stateStore)
    {
        _clientId = configuration["YahooAuth:ClientId"]
            ?? throw new ArgumentNullException("YahooAuth:ClientId is not configured");
        _clientSecret = configuration["YahooAuth:ClientSecret"]
            ?? throw new ArgumentNullException("YahooAuth:ClientSecret is not configured");
        _redirectUri = configuration["YahooAuth:RedirectUri"]
            ?? throw new ArgumentNullException("YahooAuth:RedirectUri is not configured");
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _stateStore = stateStore;
    }

    public (string Url, string State) GetAuthorizationUrl()
    {
        var scope = Uri.EscapeDataString("openid profile email");
        var state = Guid.NewGuid().ToString();
        var redirectUri = Uri.EscapeDataString(_redirectUri);

        // Store state for CSRF validation
        _stateStore.StoreState(state);

        var url = $"{YahooAuthorizationUrl}" +
               $"?response_type=code" +
               $"&client_id={Uri.EscapeDataString(_clientId)}" +
               $"&redirect_uri={redirectUri}" +
               $"&scope={scope}" +
               $"&state={Uri.EscapeDataString(state)}";

        return (url, state);
    }

    public async Task<YahooUserProfile?> ExchangeCodeForUserProfileAsync(string code)
    {
        // Step 1: Exchange authorization code for access token
        var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", _clientId },
            { "client_secret", _clientSecret },
            { "code", code },
            { "grant_type", "authorization_code" },
            { "redirect_uri", _redirectUri }
        };

        var tokenHttpClient = _httpClientFactory.CreateClient("YahooAuth");
        var tokenResponse = await tokenHttpClient.PostAsync(YahooTokenUrl,
            new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            _logger.LogError("Yahoo token exchange failed. Status: {StatusCode}, Response: {Response}",
                tokenResponse.StatusCode, errorContent);
            return null;
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        _logger.LogInformation("Yahoo token response received");
        var tokenData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(tokenJson);

        if (tokenData.ValueKind == System.Text.Json.JsonValueKind.Undefined)
        {
            _logger.LogError("Yahoo token response could not be parsed as JSON");
            return null;
        }

        var accessToken = tokenData.TryGetProperty("access_token", out var accessTokenElement)
            ? accessTokenElement.GetString()
            : null;

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogError("Yahoo token response does not contain access_token");
            return null;
        }

        // Step 2: Get user profile from Yahoo OIDC userinfo endpoint
        var profileHttpClient = _httpClientFactory.CreateClient("YahooAuth");
        profileHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var profileResponse = await profileHttpClient.GetAsync(YahooProfileUrl);

        if (!profileResponse.IsSuccessStatusCode)
        {
            var errorContent = await profileResponse.Content.ReadAsStringAsync();
            _logger.LogError("Yahoo profile API failed. Status: {StatusCode}, Response: {Response}",
                profileResponse.StatusCode, errorContent);
            return null;
        }

        var profileJson = await profileResponse.Content.ReadAsStringAsync();
        _logger.LogInformation("Yahoo profile API response received");
        var profileData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(profileJson);

        if (profileData.ValueKind == System.Text.Json.JsonValueKind.Undefined)
        {
            _logger.LogError("Yahoo profile response could not be parsed as JSON");
            return null;
        }

        // OIDC userinfo returns flat JSON: { sub, email, name, picture }
        // Extract email
        var email = profileData.TryGetProperty("email", out var emailElement)
            ? emailElement.GetString()
            : null;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogError("Could not extract email from Yahoo profile response");
            return null;
        }

        // Extract display name (OIDC uses "name")
        var displayName = profileData.TryGetProperty("name", out var nameElement)
            ? nameElement.GetString()
            : email;

        // Extract picture URL (OIDC uses "picture")
        var pictureUrl = profileData.TryGetProperty("picture", out var pictureElement)
            ? pictureElement.GetString()
            : null;

        // Use sub from userinfo as the Yahoo GUID (unique user identifier)
        var yahooGuid = profileData.TryGetProperty("sub", out var subElement)
            ? subElement.GetString()
            : null;

        if (string.IsNullOrEmpty(yahooGuid))
        {
            _logger.LogWarning("Yahoo userinfo response does not contain 'sub' (user identifier). " +
                "User will not be linked to a Yahoo GUID.");
        }

        return new YahooUserProfile(email!, displayName!, pictureUrl, yahooGuid);
    }
}
