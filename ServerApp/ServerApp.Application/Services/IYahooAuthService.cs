namespace ServerApp.Application.Services;

public interface IYahooAuthService
{
    (string Url, string State) GetAuthorizationUrl();
    Task<YahooUserProfile?> ExchangeCodeForUserProfileAsync(string code);
}

public record YahooUserProfile(
    string Email,
    string DisplayName,
    string? PictureUrl,
    string? YahooGuid);
