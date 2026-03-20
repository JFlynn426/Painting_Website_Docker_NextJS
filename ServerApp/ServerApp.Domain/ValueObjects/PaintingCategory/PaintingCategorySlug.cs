namespace ServerApp.Domain.ValueObjects.PaintingCategory;

using ServerApp.Shared.Abstractions.Domain;

public record PaintingCategorySlug : StringValueObject
{
    public const int MaxLength = 200;

    public PaintingCategorySlug() : base()
    {
    }

    public PaintingCategorySlug(string value) : base(value, MaxLength)
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

    public static implicit operator PaintingCategorySlug(string slug) => new(slug);

    /// <summary>
    /// Generates a valid slug from a painting category name
    /// </summary>
    public static PaintingCategorySlug FromName(PaintingCategoryName name)
    {
        if (name == null || string.IsNullOrWhiteSpace(name.Value))
        {
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));
        }

        // Convert to lowercase
        var slug = name.Value.ToLowerInvariant();

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

        return new PaintingCategorySlug(slug);
    }
}