namespace ServerApp.Application.DTOs;

/// <summary>
/// Standardized response for mutation commands indicating completion status.
/// </summary>
public class CommandCompletionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public int? AffectedRecords { get; set; }
}
