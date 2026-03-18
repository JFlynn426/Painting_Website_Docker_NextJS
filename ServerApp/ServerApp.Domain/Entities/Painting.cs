namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Abstractions.Domain;

public class Painting : AggregateRoot<string>
{
    public string Title { get; protected set; } = string.Empty;
    public string Description { get; protected set; } = string.Empty;
    public string ImageUrl { get; protected set; } = string.Empty;
    public string? ThumbnailUrl { get; protected set; }
    public string CategorySlug { get; protected set; } = string.Empty;
    public string? Dimensions { get; protected set; }
    public int? Year { get; protected set; }
    public decimal Price { get; protected set; }
    public bool IsAvailable { get; protected set; } = true;

    // Navigation property for the category this painting belongs to
    public PaintingCategory? Category { get; protected set; }

    // Constructor for creating a new painting
    public Painting(string id, string title, string description, string imageUrl,
        string categorySlug, decimal price, string? thumbnailUrl = null,
        string? dimensions = null, int? year = null, bool isAvailable = true)
    {
        Id = id;
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
        CategorySlug = categorySlug;
        Price = price;
        ThumbnailUrl = thumbnailUrl;
        Dimensions = dimensions;
        Year = year;
        IsAvailable = isAvailable;
    }

    // Parameterless constructor for ORM
    protected Painting() { }

    // Method to update availability
    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
    }

    // Method to update price
    public void UpdatePrice(decimal newPrice)
    {
        Price = newPrice;
    }

    // Method to associate with a category
    public void AssignCategory(PaintingCategory category)
    {
        Category = category;
        CategorySlug = category.Slug;
    }
}