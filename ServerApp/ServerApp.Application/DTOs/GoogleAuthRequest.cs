namespace ServerApp.Application.DTOs;

public class GoogleAuthRequest
{
    public string Code { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
}
