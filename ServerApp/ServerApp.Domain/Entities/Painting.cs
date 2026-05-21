namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.Exceptions;
using ServerApp.Domain.Events;

public class Painting : AggregateRoot<Guid>
{
    public PaintingName Title { get; private set; } = default!;
    public PaintingSlug Slug { get; private set; } = default!;
    public PaintingDescription? Description { get; private set; }
    public PaintingImageUrl ImageUrl { get; private set; } = default!;
    public PaintingThumbnailUrl? ThumbnailUrl { get; private set; }
    public PaintingCategorySlug CategorySlug { get; private set; } = default!;
    public PaintingWidth? Width { get; private set; }
    public PaintingHeight? Height { get; private set; }
    public PaintingDepth? Depth { get; private set; }
    public PaintingYear? Year { get; private set; }
    public PaintingPrice? Price { get; private set; }
    public PaintingIsAvailable IsAvailable { get; private set; } = default!;
    public PaintingIsNew IsNew { get; private set; } = default!;
    public PaintingIsCarouselPainting IsCarouselPainting { get; private set; } = default!;

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
        PaintingYear? year = null, PaintingIsAvailable isAvailable = default!, PaintingIsNew isNew = default!, PaintingIsCarouselPainting isCarouselPainting = default!)
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
        IsNew = isNew;
        IsCarouselPainting = isCarouselPainting;

        AddEvent(new PaintingCreatedEvent(Id, title.Value, categorySlug.Value));
    }

    // Consolidated update method - applies only non-null parameters
    public void Update(
        PaintingDescription? description = null,
        PaintingImageUrl? imageUrl = null,
        PaintingThumbnailUrl? thumbnailUrl = null,
        PaintingPrice? price = null,
        PaintingWidth? width = null,
        PaintingHeight? height = null,
        PaintingDepth? depth = null,
        PaintingYear? year = null,
        PaintingIsAvailable? isAvailable = null,
        PaintingIsNew? isNew = null,
        PaintingIsCarouselPainting? isCarouselPainting = null)
    {
        if (description != null) Description = description;
        if (imageUrl != null) ImageUrl = imageUrl;
        if (thumbnailUrl != null) ThumbnailUrl = thumbnailUrl;
        if (price != null) Price = price;
        if (width != null) Width = width;
        if (height != null) Height = height;
        if (depth != null) Depth = depth;
        if (year != null) Year = year;
        if (isAvailable != null) IsAvailable = isAvailable;
        if (isNew != null) IsNew = isNew;
        if (isCarouselPainting != null) IsCarouselPainting = isCarouselPainting;

        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to associate with a category (separate operation)
    public void AssignCategory(PaintingCategory category)
    {
        Category = category;
        CategorySlug = category.Slug;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to update category slug (called when category name/slug changes)
    public void UpdateCategorySlug(PaintingCategorySlug newCategorySlug)
    {
        CategorySlug = newCategorySlug;
        AddEvent(new PaintingUpdatedEvent(Id, Title.Value, CategorySlug.Value));
    }

    // Method to mark painting for deletion
    public void MarkAsDeleted()
    {
        AddEvent(new PaintingDeletedEvent(Id, Title.Value));
    }
}