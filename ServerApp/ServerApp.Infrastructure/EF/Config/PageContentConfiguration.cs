namespace ServerApp.Infrastructure.EF.Config;

using System.Text.Json;
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

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Address)
            .HasColumnName("Address")
            .HasColumnType("varchar(200)")
            .IsRequired()
            .HasConversion(
                a => a.Value,
                value => new PageAddress(value));

        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasColumnType("varchar(200)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<PageTitle?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new PageTitle(v)));

        builder.Property(e => e.Content)
            .HasColumnName("Content")
            .HasColumnType("text")
            .IsRequired()
            .HasConversion(
                c => c.Value,
                value => new PageContentText(value));

        builder.Property(e => e.PhotoUrls)
            .HasColumnName("PhotoUrls")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasConversion(
                new ValueConverter<PagePhotoUrls, string>(
                    v => JsonSerializer.Serialize(v.ToArray()),
                    v => new PagePhotoUrls(JsonSerializer.Deserialize<string[]>(v) ?? Array.Empty<string>())));

        builder.HasIndex(e => e.Address).IsUnique();
    }
}
