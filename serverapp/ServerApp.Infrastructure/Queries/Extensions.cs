namespace ServerApp.Infrastructure.Queries;

using ServerApp.Application.DTOs;
using ServerApp.Infrastructure.EF.Models;

internal static class Extensions
{
    public static PaintingCategoryDto AsDTO(this PaintingCategoryReadModel readModel)
    {
        return new PaintingCategoryDto
        {
            Id = readModel.Id,
            Name = readModel.Name,
            Slug = readModel.Slug,
            Description = readModel.Description
        };
    }

    public static PaintingDto AsDTO(this PaintingReadModel readModel)
    {
        return new PaintingDto
        {
            Title = readModel.Title,
            Description = readModel.Description,
            ImageUrl = readModel.ImageUrl,
            ThumbnailUrl = readModel.ThumbnailUrl,
            CategorySlug = readModel.CategorySlug,
            Width = readModel.Width,
            Height = readModel.Height,
            Depth = readModel.Depth,
            Year = readModel.Year,
            Price = readModel.Price,
            IsAvailable = readModel.IsAvailable
        };
    }

    public static PageContentDto AsDTO(this PageContentReadModel readModel)
    {
        return new PageContentDto
        {
            Address = readModel.Address,
            Title = readModel.Title,
            Content = readModel.Content
        };
    }

    public static PaintingCategoryWithPaintingsDto AsDTOWithPaintings(this PaintingCategoryReadModel readModel, List<PaintingReadModel> paintings)
    {
        return new PaintingCategoryWithPaintingsDto
        {
            Id = readModel.Id,
            Name = readModel.Name,
            Slug = readModel.Slug,
            Description = readModel.Description,
            Paintings = paintings.Select(p => p.AsDTO()).ToList()
        };
    }
}