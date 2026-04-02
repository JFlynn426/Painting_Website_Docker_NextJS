namespace ServerApp.Shared.Validation;

using System.Text.RegularExpressions;

/// <summary>
/// Utility class for URL validation.
/// </summary>
public static class UrlValidator
{
    // URL pattern for http/https URLs or relative paths
    // Accepts: http://example.com/path, https://example.com/path, /path/to/image.jpg
    // Also allows spaces and parentheses in paths (for filenames like "Cloud Creatures2.jpg" or "Mine (1).jpg")
    private static readonly Regex UrlPattern = new(
        @"^(https?:\/\/)?(([\w\-]+\.)+[\w\-]+)?(\/[\w\s\(\)\-./?%&=#_]*)?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Validates if the given string is a valid URL.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid, false otherwise.</returns>
    public static bool IsValid(string url)
    {
        return UrlPattern.IsMatch(url);
    }
}