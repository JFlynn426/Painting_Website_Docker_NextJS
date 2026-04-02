namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        // Nullable value object - conversion must handle null both ways
        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("nvarchar(200)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<PageTitle?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PageTitle(v)));

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