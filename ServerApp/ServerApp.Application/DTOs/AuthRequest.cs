namespace ServerApp.Application.DTOs;

public class AuthRequest
{
    public string Code { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
}
