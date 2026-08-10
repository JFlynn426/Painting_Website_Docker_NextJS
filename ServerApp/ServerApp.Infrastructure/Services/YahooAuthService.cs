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

    private const string YahooAuthorizationUrl = "https://api.login.yahoo.com/oauth2/request_auth";
    private const string YahooTokenUrl = "https://api.login.yahoo.com/oauth2/get_token";
    private const string YahooProfileUrl = "https://social.yahooapis.com/v1/user/{0}/profile?format=json";

    public YahooAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<YahooAuthService> logger)
    {
        _clientId = configuration["YahooAuth:ClientId"]
            ?? throw new ArgumentNullException("YahooAuth:ClientId is not configured");
        _clientSecret = configuration["YahooAuth:ClientSecret"]
            ?? throw new ArgumentNullException("YahooAuth:ClientSecret is not configured");
        _redirectUri = configuration["YahooAuth:RedirectUri"]
            ?? throw new ArgumentNullException("YahooAuth:RedirectUri is not configured");
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string GetAuthorizationUrl()
    {
        var scope = Uri.EscapeDataString("profile email");
        var state = Uri.EscapeDataString(Guid.NewGuid().ToString());
        var redirectUri = Uri.EscapeDataString(_redirectUri);

        return $"{YahooAuthorizationUrl}" +
               $"?response_type=code" +
               $"&client_id={Uri.EscapeDataString(_clientId)}" +
               $"&redirect_uri={redirectUri}" +
               $"&scope={scope}" +
               $"&state={state}";
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

        var tokenHttpClient = _httpClientFactory.CreateClient();
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

        // Extract guid from token response (Yahoo returns guid in the token response)
        var guid = tokenData.TryGetProperty("guid", out var guidElement)
            ? guidElement.GetString()
            : null;

        if (string.IsNullOrEmpty(guid))
        {
            _logger.LogError("Yahoo token response does not contain guid");
            return null;
        }

        // Step 2: Get user profile from Yahoo using the guid
        var profileUrl = string.Format(YahooProfileUrl, Uri.EscapeDataString(guid));
        var profileHttpClient = _httpClientFactory.CreateClient();
        profileHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var profileResponse = await profileHttpClient.GetAsync(profileUrl);

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

        // Navigate to profile nested object
        if (!profileData.TryGetProperty("profile", out var profileObject))
        {
            _logger.LogError("Yahoo profile response does not contain profile object");
            return null;
        }

        // Extract email
        var email = profileObject.TryGetProperty("email", out var emailElement)
            ? emailElement.GetString()
            : null;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogError("Could not extract email from Yahoo profile response");
            return null;
        }

        // Extract display name (Yahoo uses givenName and familyName)
        var givenName = profileObject.TryGetProperty("givenName", out var givenNameElement)
            ? givenNameElement.GetString()
            : null;
        var familyName = profileObject.TryGetProperty("familyName", out var familyNameElement)
            ? familyNameElement.GetString()
            : null;

        var displayName = string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(familyName)
            ? email
            : $"{givenName ?? string.Empty} {familyName ?? string.Empty}".Trim();

        // Extract picture URL (Yahoo uses image -> imageSize -> imageUrl or profileImage)
        var pictureUrl = profileObject.TryGetProperty("image", out var imageElement) && imageElement.ValueKind == JsonValueKind.Object
            ? imageElement.TryGetProperty("imageUrl", out var imageUrlElement)
                ? imageUrlElement.GetString()
                : null
            : null;

        return new YahooUserProfile(email, displayName, pictureUrl, guid);
    }
}
