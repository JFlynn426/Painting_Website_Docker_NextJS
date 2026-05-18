namespace ServerApp.Application.Services;

public interface IGoogleAuthService
{
    string GetAuthorizationUrl();
    Task<GoogleUserProfile?> ExchangeCodeForUserProfileAsync(string code);
}

public record GoogleUserProfile(
    string Email,
    string DisplayName,
    string? PictureUrl,
    string GoogleSubjectId);
