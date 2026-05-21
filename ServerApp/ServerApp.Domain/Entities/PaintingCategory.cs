namespace ServerApp.Domain.Entities;

using System.Collections.Generic;
using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.Events;

public class PaintingCategory : AggregateRoot<Guid>
{
    public PaintingCategoryName Name { get; private set; } = default!;
    public PaintingCategorySlug Slug { get; private set; } = default!;
    public PaintingCategoryDescription? Description { get; private set; }

    // Navigation property for Paintings in this category
    public ICollection<Painting> Paintings { get; private set; } = new List<Painting>();

    // Parameterless constructor for EF Core
    private PaintingCategory() { }

    // Constructor for creating a new category (domain creation path)
    internal PaintingCategory(PaintingCategoryID id, PaintingCategoryName name, PaintingCategorySlug slug, PaintingCategoryDescription? description = null)
    {
        Id = id.Value;
        Name = name;
        Slug = slug;
        Description = description;

        AddEvent(new PaintingCategoryCreatedEvent(Id, name.Value, slug.Value));
    }

    // Consolidated update method - applies only non-null parameters
    public void Update(
        PaintingCategoryName? name = null,
        PaintingCategoryDescription? description = null)
    {
        if (name != null)
        {
            Name = name;
            Slug = PaintingCategorySlug.FromName(name);
        }
        if (description != null) Description = description;

        AddEvent(new PaintingCategoryUpdatedEvent(Id, Name.Value, Slug.Value));
    }

    // Method to remove a painting from this category
    public void RemovePainting(Painting painting)
    {
        Paintings.Remove(painting);
    }

    // Method to mark category for deletion
    public void MarkAsDeleted()
    {
        AddEvent(new PaintingCategoryDeletedEvent(Id, Name.Value));
    }
}