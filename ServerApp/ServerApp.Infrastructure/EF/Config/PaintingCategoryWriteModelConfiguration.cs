namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerApp.Infrastructure.EF.Models;

public class PaintingCategoryWriteModelConfiguration : IEntityTypeConfiguration<PaintingCategoryWriteModel>
{
    public void Configure(EntityTypeBuilder<PaintingCategoryWriteModel> builder)
    {
        builder.ToTable("PaintingCategories");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("Name")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        builder.Property(e => e.Slug)
            .HasColumnName("Slug")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("Description")
            .HasColumnType("nvarchar(max)");

        builder.HasIndex(e => e.Slug)
            .IsUnique();
    }
}