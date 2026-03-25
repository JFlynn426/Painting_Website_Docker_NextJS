namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerApp.Infrastructure.EF.Models;

public class PaintingReadModelConfiguration : IEntityTypeConfiguration<PaintingReadModel>
{
    public void Configure(EntityTypeBuilder<PaintingReadModel> builder)
    {
        builder.ToTable("Paintings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(e => e.Slug)
            .HasColumnName("Slug")
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.ImageUrl)
            .HasColumnName("ImageUrl")
            .HasColumnType("nvarchar(500)")
            .IsRequired();

        builder.Property(e => e.ThumbnailUrl)
            .HasColumnName("ThumbnailUrl")
            .HasColumnType("nvarchar(500)");

        builder.Property(e => e.CategorySlug)
            .HasColumnName("CategorySlug")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        builder.Property(e => e.Width)
            .HasColumnName("Width")
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Height)
            .HasColumnName("Height")
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Depth)
            .HasColumnName("Depth")
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Year)
            .HasColumnName("Year")
            .HasColumnType("int");

        builder.Property(e => e.Price)
            .HasColumnName("Price")
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.IsAvailable)
            .HasColumnName("IsAvailable")
            .HasColumnType("bit")
            .IsRequired();

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