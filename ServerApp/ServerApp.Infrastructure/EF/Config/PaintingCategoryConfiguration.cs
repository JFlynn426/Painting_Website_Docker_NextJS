namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.PaintingCategory;

public class PaintingCategoryConfiguration : IEntityTypeConfiguration<PaintingCategory>
{
    public void Configure(EntityTypeBuilder<PaintingCategory> builder)
    {
        builder.ToTable("PaintingCategories");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        // Value object with a non-null backing primitive
        builder.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("nvarchar(50)")
            .IsRequired()
            .HasConversion(
                n => n.Value,
                value => new PaintingCategoryName(value));

        // Value object with a non-null backing primitive
        builder.Property(e => e.Slug)
            .HasColumnName("Slug")
            .HasColumnType("nvarchar(100)")
            .IsRequired()
            .HasConversion(
                s => s.Value,
                value => new PaintingCategorySlug(value));

        // Plain string property (nullable)
        builder.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.HasIndex(e => e.Slug).IsUnique();
    }
}