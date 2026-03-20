namespace ServerApp.Domain.Entities;

using System.Collections.Generic;
using ServerApp.Shared.Abstractions.Domain;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingCategory : AggregateRoot<Guid>
{
    public PaintingCategoryName Name { get; protected set; }
    public PaintingCategorySlug Slug { get; protected set; }
    public PaintingCategoryDescription? Description { get; protected set; }

    // Navigation property for Paintings in this category
    public ICollection<Painting> Paintings { get; protected set; } = new List<Painting>();

    // Computed property for painting count
    public int PaintingCount => Paintings.Count;

    // Constructor for creating a new category
    internal PaintingCategory(PaintingCategoryID id, PaintingCategoryName name, PaintingCategorySlug slug, PaintingCategoryDescription? description = null)
    {
        Id = id.Value;
        Name = name;
        Slug = slug;
        Description = description;
    }

    // Parameterless constructor for ORM
    protected PaintingCategory() { }

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