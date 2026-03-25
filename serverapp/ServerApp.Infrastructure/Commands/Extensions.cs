namespace ServerApp.Infrastructure.Commands;

using ServerApp.Application.Commands;
using ServerApp.Infrastructure.EF.Models;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;
using ServerApp.Domain.ValueObjects.Page;

internal static class Extensions
{
    public static PaintingWriteModel ToWriteModel(this AddPainting command)
    {
        var id = new PaintingID();
        var title = new PaintingName(command.Title);
        var slug = PaintingSlug.FromTitle(title);
        var description = PaintingDescription.FromNullable(command.Description);
        var imageUrl = new PaintingImageUrl(command.ImageUrl);
        var thumbnailUrl = PaintingThumbnailUrl.FromNullable(command.ThumbnailUrl);
        var categorySlug = new PaintingCategorySlug(command.CategorySlug);
        var price = PaintingPrice.FromNullable(command.Price);
        var width = PaintingWidth.FromNullable(command.Width);
        var height = PaintingHeight.FromNullable(command.Height);
        var depth = PaintingDepth.FromNullable(command.Depth);
        var year = PaintingYear.FromNullable(command.Year);
        var isAvailable = new PaintingIsAvailable(command.IsAvailable);

        return new PaintingWriteModel
        {
            Id = id.Value,
            Title = title.Value,
            Slug = slug.Value,
            Description = description?.Value,
            ImageUrl = imageUrl.Value,
            ThumbnailUrl = thumbnailUrl?.Value,
            CategorySlug = categorySlug.Value,
            CategoryId = null,
            Price = price?.Value,
            Width = width?.Value,
            Height = height?.Value,
            Depth = depth?.Value,
            Year = year?.Value,
            IsAvailable = isAvailable.Value
        };
    }

    public static PageContentWriteModel ToWriteModel(this AddPageContent command)
    {
        var address = new PageAddress(command.Address);
        var title = new PageTitle(command.Title);
        var content = new PageContentText(command.Content);

        return new PageContentWriteModel
        {
            Id = Guid.NewGuid(),
            Address = address.Value,
            Title = title.Value,
            Content = content.Value
        };
    }

    public static PaintingCategoryWriteModel ToWriteModel(this AddPaintingCategory command)
    {
        var name = new PaintingCategoryName(command.Name);
        var slug = PaintingCategorySlug.FromName(name);
        var id = new PaintingCategoryID();

        return new PaintingCategoryWriteModel
        {
            Id = id.Value,
            Name = command.Name,
            Slug = slug.Value,
            Description = command.Description
        };
    }
}