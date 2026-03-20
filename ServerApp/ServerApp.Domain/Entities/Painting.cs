namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Abstractions.Domain;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.Exceptions;

public class Painting : AggregateRoot<Guid>
{
    public PaintingName Title { get; protected set; } = new PaintingName();
    public PaintingDescription Description { get; protected set; } = new PaintingDescription();
    public PaintingImageUrl ImageUrl { get; protected set; } = new PaintingImageUrl();
    public PaintingThumbnailUrl ThumbnailUrl { get; protected set; } = new PaintingThumbnailUrl();
    public PaintingCategorySlug CategorySlug { get; protected set; }
    public PaintingWidth? Width { get; protected set; }
    public PaintingHeight? Height { get; protected set; }
    public PaintingDepth? Depth { get; protected set; }
    public PaintingYear? Year { get; protected set; }
    public PaintingPrice Price { get; protected set; } = new PaintingPrice();
    public PaintingIsAvailable IsAvailable { get; protected set; } = true;

    // Navigation property for the category this painting belongs to
    public PaintingCategory? Category { get; protected set; }

    // Constructor for creating a new painting
    public Painting(PaintingID id, PaintingName title, PaintingDescription description, PaintingImageUrl imageUrl,
        PaintingThumbnailUrl thumbnailUrl, PaintingCategorySlug categorySlug, PaintingPrice price,
        PaintingWidth? width = null, PaintingHeight? height = null, PaintingDepth? depth = null,
        PaintingYear? year = null, PaintingIsAvailable isAvailable = default!)
    {
        Id = id.Value;
        Title = title;
        Description = description;
        ImageUrl = imageUrl;
        ThumbnailUrl = thumbnailUrl;
        CategorySlug = categorySlug;
        Price = price;
        Width = width;
        Height = height;
        Depth = depth;
        Year = year;
        IsAvailable = isAvailable;
    }

    // Parameterless constructor for ORM
    protected Painting() { }

    // Method to update availability
    public void SetAvailability(PaintingIsAvailable isAvailable)
    {
        IsAvailable = isAvailable;
    }

    // Method to update price
    public void UpdatePrice(PaintingPrice newPrice)
    {
        Price = newPrice;
    }

    // Method to update description
    public void UpdateDescription(PaintingDescription newDescription)
    {
        Description = newDescription;
    }

    // Method to update image URL
    public void UpdateImageUrl(PaintingImageUrl newImageUrl)
    {
        ImageUrl = newImageUrl;
    }

    // Method to update thumbnail URL
    public void UpdateThumbnailUrl(PaintingThumbnailUrl newThumbnailUrl)
    {
        ThumbnailUrl = newThumbnailUrl;
    }

    // Method to update year
    public void UpdateYear(PaintingYear newYear)
    {
        Year = newYear;
    }

    // Method to update dimensions
    public void UpdateDimensions(PaintingWidth? width, PaintingHeight? height, PaintingDepth? depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
    }

    // Method to associate with a category
    public void AssignCategory(PaintingCategory category)
    {
        Category = category;
        CategorySlug = category.Slug;
    }
}