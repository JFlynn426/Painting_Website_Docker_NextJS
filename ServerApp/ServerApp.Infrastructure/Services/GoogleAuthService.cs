namespace ServerApp.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using ServerApp.Application.Services;

// Implementation of IGoogleAuthService from Application layer

public class GoogleAuthService : IGoogleAuthService
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<GoogleAuthService> _logger;

    private const string GoogleAuthorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string GoogleTokenUrl = "https://oauth2.googleapis.com/token";
    private const string GoogleUserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

    public GoogleAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<GoogleAuthService> logger)
    {
        _clientId = configuration["GoogleAuth:ClientId"]
            ?? throw new ArgumentNullException("GoogleAuth:ClientId is not configured");
        _clientSecret = configuration["GoogleAuth:ClientSecret"]
            ?? throw new ArgumentNullException("GoogleAuth:ClientSecret is not configured");
        _redirectUri = configuration["GoogleAuth:RedirectUri"]
            ?? throw new ArgumentNullException("GoogleAuth:RedirectUri is not configured");
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public string GetAuthorizationUrl()
    {
        var scope = Uri.EscapeDataString("email profile openid");
        var state = Uri.EscapeDataString(Guid.NewGuid().ToString());
        var redirectUri = Uri.EscapeDataString(_redirectUri);

        return $"{GoogleAuthorizationUrl}" +
               $"?response_type=code" +
               $"&client_id={Uri.EscapeDataString(_clientId)}" +
               $"&redirect_uri={redirectUri}" +
               $"&scope={scope}" +
               $"&access_type=offline" +
               $"&prompt=select_account" +
               $"&state={state}";
    }

    public async Task<GoogleUserProfile?> ExchangeCodeForUserProfileAsync(string code)
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
        var tokenResponse = await tokenHttpClient.PostAsync(GoogleTokenUrl,
            new FormUrlEncodedContent(tokenRequest));

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorContent = await tokenResponse.Content.ReadAsStringAsync();
            _logger.LogError("Google token exchange failed. Status: {StatusCode}, Response: {Response}",
                tokenResponse.StatusCode, errorContent);
            return null;
        }

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
        _logger.LogInformation("Google token response received");
        var tokenData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(tokenJson);

        if (tokenData.ValueKind == System.Text.Json.JsonValueKind.Undefined)
        {
            _logger.LogError("Google token response could not be parsed as JSON");
            return null;
        }

        var accessToken = tokenData.TryGetProperty("access_token", out var accessTokenElement)
            ? accessTokenElement.GetString()
            : null;

        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogError("Google token response does not contain access_token");
            return null;
        }

        // Step 2: Get user profile from Google OAuth2 userinfo endpoint
        var userInfoHttpClient = _httpClientFactory.CreateClient();
        userInfoHttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var userInfoResponse = await userInfoHttpClient.GetAsync(GoogleUserInfoUrl);

        if (!userInfoResponse.IsSuccessStatusCode)
        {
            var errorContent = await userInfoResponse.Content.ReadAsStringAsync();
            _logger.LogError("Google userinfo API failed. Status: {StatusCode}, Response: {Response}",
                userInfoResponse.StatusCode, errorContent);
            return null;
        }

        var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
        _logger.LogInformation("Google userinfo API response received");
        var userInfoData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(userInfoJson);

        if (userInfoData.ValueKind == System.Text.Json.JsonValueKind.Undefined)
        {
            _logger.LogError("Google userinfo response could not be parsed as JSON");
            return null;
        }

        // Extract email
        var email = userInfoData.TryGetProperty("email", out var emailElement)
            ? emailElement.GetString()
            : null;
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogError("Could not extract email from Google userinfo response");
            return null;
        }

        // Extract display name
        var displayName = userInfoData.TryGetProperty("name", out var nameElement)
            ? nameElement.GetString() ?? email
            : email;

        // Extract picture URL
        var pictureUrl = userInfoData.TryGetProperty("picture", out var pictureElement)
            ? pictureElement.GetString()
            : null;

        // Extract Google subject ID
        var googleSubjectId = userInfoData.TryGetProperty("id", out var idElement)
            ? idElement.GetString() ?? string.Empty
            : string.Empty;

        return new GoogleUserProfile(email, displayName, pictureUrl, googleSubjectId);
    }

}
