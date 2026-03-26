namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Page;

public class PageContentConfiguration : IEntityTypeConfiguration<PageContent>
{
    public void Configure(EntityTypeBuilder<PageContent> builder)
    {
        builder.ToTable("PageContents");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        // Value object with a non-null backing primitive
        builder.Property(e => e.Address)
            .HasColumnName("Address")
            .HasColumnType("nvarchar(200)")
            .IsRequired()
            .HasConversion(
                a => a.Value,
                value => new PageAddress(value));

        // Value object with a non-null backing primitive
        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("nvarchar(200)")
            .IsRequired()
            .HasConversion(
                t => t.Value,
                value => new PageTitle(value));

        // Value object with a non-null backing primitive
        builder.Property(e => e.Content)
            .HasColumnName("Content")
            .HasColumnType("nvarchar(max)")
            .IsRequired()
            .HasConversion(
                c => c.Value,
                value => new PageContentText(value));

        builder.HasIndex(e => e.Address).IsUnique();
    }
}