namespace ServerApp.Domain.Services;

/// <summary>
/// Service interface for HTML sanitization to prevent XSS attacks.
/// </summary>
public interface IHtmlSanitizer
{
    /// <summary>
    /// Sanitizes HTML content by removing potentially dangerous elements and attributes.
    /// </summary>
    /// <param name="html">The HTML content to sanitize.</param>
    /// <returns>The sanitized HTML content.</returns>
    string Sanitize(string html);
}