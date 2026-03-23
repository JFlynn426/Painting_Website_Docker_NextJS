namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerApp.Infrastructure.EF.Models;

public class PageContentReadModelConfiguration : IEntityTypeConfiguration<PageContentReadModel>
{
    public void Configure(EntityTypeBuilder<PageContentReadModel> builder)
    {
        builder.ToTable("PageContents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Address)
            .HasColumnName("Address")
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("nvarchar(200)")
            .IsRequired();

        builder.Property(e => e.Content)
            .HasColumnName("Content")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("UpdatedAt")
            .HasColumnType("datetime2");

        builder.HasIndex(e => e.Address)
            .IsUnique();
    }
}