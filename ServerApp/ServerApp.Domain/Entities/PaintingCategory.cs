namespace ServerApp.Domain.Entities;

using System.Collections.Generic;
using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingCategory : AggregateRoot<Guid>
{
    public PaintingCategoryName Name { get; private set; }
    public PaintingCategorySlug Slug { get; private set; }
    public string? Description { get; private set; }

    // Navigation property for Paintings in this category
    public ICollection<Painting> Paintings { get; private set; } = new List<Painting>();

    // Parameterless constructor for EF Core
    private PaintingCategory() { }

    // Constructor for creating a new category (domain creation path)
    internal PaintingCategory(PaintingCategoryID id, PaintingCategoryName name, PaintingCategorySlug slug, string? description = null)
    {
        Id = id.Value;
        Name = name;
        Slug = slug;
        Description = description;
    }

    // Method to add a painting to this category
    public void AddPainting(Painting painting)
    {
        if (!Paintings.Contains(painting))
        {
            Paintings.Add(painting);
        }
    }

    // Method to remove a painting from this category
    public void RemovePainting(Painting painting)
    {
        Paintings.Remove(painting);
    }
}