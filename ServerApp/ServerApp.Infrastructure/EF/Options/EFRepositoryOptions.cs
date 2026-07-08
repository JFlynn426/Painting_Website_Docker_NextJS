namespace ServerApp.Infrastructure.EF.Options;

/// <summary>
/// Configuration options for EF Core repositories.
/// </summary>
public class EFRepositoryOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}
