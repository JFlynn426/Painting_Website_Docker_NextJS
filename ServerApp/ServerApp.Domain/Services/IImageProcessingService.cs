namespace ServerApp.Domain.Services;

public interface IImageProcessingService
{
    Task<ImageProcessingResult> ProcessAndSaveAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
}
