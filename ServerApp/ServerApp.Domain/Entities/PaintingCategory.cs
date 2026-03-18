namespace ServerApp.Domain.Entities;

using System.Collections.Generic;
using ServerApp.Shared.Abstractions.Domain;

public class PaintingCategory : AggregateRoot<string>
{
    public string Name { get; protected set; } = string.Empty;
    public string Slug { get; protected set; } = string.Empty;
    public string? Description { get; protected set; }

    // Navigation property for Paintings in this category
    public ICollection<Painting> Paintings { get; protected set; } = new List<Painting>();

    // Computed property for painting count
    public int PaintingCount => Paintings.Count;

    // Constructor for creating a new category
    public PaintingCategory(string id, string name, string slug, string? description = null)
    {
        Id = id;
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