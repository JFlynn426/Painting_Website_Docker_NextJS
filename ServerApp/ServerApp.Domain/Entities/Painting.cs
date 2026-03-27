namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.Exceptions;
using ServerApp.Domain.Events;

public class Painting : AggregateRoot<Guid>
{
    public PaintingName Title { get; private set; } = new PaintingName();
    public PaintingSlug Slug { get; private set; } = new PaintingSlug();
    public PaintingDescription? Description { get; private set; }
    public PaintingImageUrl ImageUrl { get; private set; } = new PaintingImageUrl();
    public PaintingThumbnailUrl? ThumbnailUrl { get; private set; }
    public PaintingCategorySlug CategorySlug { get; private set; }
    public PaintingWidth? Width { get; private set; }
    public PaintingHeight? Height { get; private set; }
    public PaintingDepth? Depth { get; private set; }
    public PaintingYear? Year { get; private set; }
    public PaintingPrice? Price { get; private set; }
    public PaintingIsAvailable IsAvailable { get; private set; } = true;

    // Navigation property for the category this painting belongs to
    public PaintingCategory? Category { get; private set; }

    // Foreign key property for EF Core
    public Guid? CategoryId { get; private set; }

    // Parameterless constructor for EF Core
    private Painting() { }

    // Constructor for creating a new painting (domain creation path)
    // Internal constructor that accepts pre-computed slug (used by factory)
    internal Painting(PaintingID id, PaintingName title, PaintingSlug slug, PaintingDescription? description, PaintingImageUrl imageUrl,
        PaintingThumbnailUrl? thumbnailUrl, PaintingCategorySlug categorySlug, PaintingPrice? price,
        PaintingWidth? width = null, PaintingHeight? height = null, PaintingDepth? depth = null,
        PaintingYear? year = null, PaintingIsAvailable isAvailable = default!)
    {
        Id = id.Value;
        Title = title;
        Slug = slug;
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

        AddEvent(new PaintingCreatedEvent(Id, title.Value, categorySlug.Value));
    }

    // Method to update availability
    public void SetAvailability(PaintingIsAvailable isAvailable)
    {
        IsAvailable = isAvailable;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update price
    public void UpdatePrice(PaintingPrice newPrice)
    {
        Price = newPrice;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update description
    public void UpdateDescription(PaintingDescription newDescription)
    {
        Description = newDescription;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update image URL
    public void UpdateImageUrl(PaintingImageUrl newImageUrl)
    {
        ImageUrl = newImageUrl;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update thumbnail URL
    public void UpdateThumbnailUrl(PaintingThumbnailUrl newThumbnailUrl)
    {
        ThumbnailUrl = newThumbnailUrl;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update year
    public void UpdateYear(PaintingYear newYear)
    {
        Year = newYear;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update dimensions
    public void UpdateDimensions(PaintingWidth? width, PaintingHeight? height, PaintingDepth? depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to associate with a category
    public void AssignCategory(PaintingCategory category)
    {
        Category = category;
        CategorySlug = category.Slug;
    }

    // Method to mark painting for deletion
    public void MarkAsDeleted()
    {
        AddEvent(new PaintingDeletedEvent(Id, Title.Value));
    }
}