namespace ServerApp.Domain.ValueObjects.Painting;

using ServerApp.Shared.Domain;

public record PaintingSlug : StringValueObject
{
    public const int MaxLength = 200;

    public PaintingSlug() : base()
    {
    }

    public PaintingSlug(string value) : base(value, MaxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Slug cannot be empty or whitespace.", nameof(value));
        }

        // Validate slug format (lowercase, alphanumeric with hyphens only)
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-z0-9]+(-[a-z0-9]+)*$"))
        {
            throw new ArgumentException("Slug must contain only lowercase letters, numbers, and hyphens.", nameof(value));
        }
    }

    public static implicit operator PaintingSlug(string slug) => new(slug);

    /// <summary>
    /// Generates a valid slug from a painting title
    /// </summary>
    public static PaintingSlug FromTitle(PaintingName title)
    {
        if (title == null || string.IsNullOrWhiteSpace(title.Value))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        }

        // Convert to lowercase
        var slug = title.Value.ToLowerInvariant();

        // Replace common characters
        slug = slug.Replace("&", "-and-");
        slug = slug.Replace("'", "");
        slug = slug.Replace(",", "");
        slug = slug.Replace(".", "");
        slug = slug.Replace("?", "");
        slug = slug.Replace("!", "");

        // Replace spaces and multiple spaces with single hyphen
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");

        // Remove any characters that are not lowercase letters, numbers, or hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9-]", "");

        // Remove leading/trailing hyphens
        slug = slug.Trim('-');

        // Collapse multiple consecutive hyphens
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

        return new PaintingSlug(slug);
    }
}