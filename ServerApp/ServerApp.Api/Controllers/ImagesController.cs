using Microsoft.AspNetCore.Mvc;
using ServerApp.Domain.Services;

namespace ServerApp.Api.Controllers;

/// <summary>
/// Controller for image upload and management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ServerApp.Api.Filters.AdminAuthorized]
public class ImagesController : BaseController
{
    private readonly IImageProcessingService _imageService;

    public ImagesController(IImageProcessingService imageService)
    {
        _imageService = imageService;
    }

    /// <summary>
    /// Uploads an image file and processes it into multiple sizes.
    /// </summary>
    /// <param name="file">The image file to upload (JPG/JPEG only).</param>
    /// <returns>200 OK with the URLs for original, high-res, and thumbnail images.</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _imageService.ProcessAndSaveAsync(stream, file.FileName);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an image file from all storage directories.
    /// </summary>
    /// <param name="fileName">The filename of the image to delete.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("{fileName}")]
    public async Task<IActionResult> DeleteImage(string fileName)
    {
        await _imageService.DeleteAsync(fileName);
        return NoContent();
    }
}
