namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Painting;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingConfiguration : IEntityTypeConfiguration<Painting>
{
    public void Configure(EntityTypeBuilder<Painting> builder)
    {
        builder.ToTable("Paintings");

        // Primary key - unwrap the value object to its primitive
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        // Value object with a non-null backing primitive
        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("nvarchar(200)")
            .IsRequired()
            .HasConversion(
                n => n.Value,
                value => new PaintingName(value));

        // Value object with a non-null backing primitive
        builder.Property(e => e.Slug)
            .HasColumnName("Slug")
            .HasColumnType("nvarchar(200)")
            .IsRequired()
            .HasConversion(
                s => s.Value,
                value => new PaintingSlug(value));

        // Nullable value object - conversion must handle null both ways
        builder.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<PaintingDescription?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingDescription(v)));

        // Value object with a non-null backing primitive
        builder.Property(e => e.ImageUrl)
            .HasColumnName("ImageUrl")
            .HasColumnType("nvarchar(500)")
            .IsRequired()
            .HasConversion(
                url => url.Value,
                value => new PaintingImageUrl(value));

        // Nullable value object
        builder.Property(e => e.ThumbnailUrl)
            .HasColumnName("ThumbnailUrl")
            .HasColumnType("nvarchar(500)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<PaintingThumbnailUrl?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingThumbnailUrl(v)));

        // Value object with a non-null backing primitive
        builder.Property(e => e.CategorySlug)
            .HasColumnName("CategorySlug")
            .HasColumnType("nvarchar(100)")
            .IsRequired()
            .HasConversion(
                s => s.Value,
                value => new PaintingCategorySlug(value));

        // Nullable value objects for dimensions
        builder.Property(e => e.Width)
            .HasColumnName("Width")
            .HasColumnType("decimal(18, 2)")
            .HasConversion(
                new ValueConverter<PaintingWidth?, decimal?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingWidth((decimal)v)));

        builder.Property(e => e.Height)
            .HasColumnName("Height")
            .HasColumnType("decimal(18, 2)")
            .HasConversion(
                new ValueConverter<PaintingHeight?, decimal?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingHeight((decimal)v)));

        builder.Property(e => e.Depth)
            .HasColumnName("Depth")
            .HasColumnType("decimal(18, 2)")
            .HasConversion(
                new ValueConverter<PaintingDepth?, decimal?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingDepth((decimal)v)));

        // Nullable value object for year
        builder.Property(e => e.Year)
            .HasColumnName("Year")
            .HasColumnType("int")
            .HasConversion(
                new ValueConverter<PaintingYear?, int?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingYear((int)v)));

        // Nullable value object for price
        builder.Property(e => e.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(18, 2)")
            .HasConversion(
                new ValueConverter<PaintingPrice?, decimal?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PaintingPrice((decimal)v)));

        // Value object for boolean
        builder.Property(e => e.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasColumnType("bit")
            .IsRequired()
            .HasConversion(
                a => a.Value,
                value => new PaintingIsAvailable(value));

        // Value object for boolean
        builder.Property(e => e.IsNew)
            .HasColumnName("IsNew")
            .HasColumnType("bit")
            .IsRequired()
            .HasConversion(
                a => a.Value,
                value => new PaintingIsNew(value));

        // Navigation property
        builder.Property(e => e.CategoryId)
            .HasColumnName("CategoryId")
            .HasColumnType("uniqueidentifier");

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Paintings)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.CategorySlug);
        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.Slug).IsUnique();
    }
}