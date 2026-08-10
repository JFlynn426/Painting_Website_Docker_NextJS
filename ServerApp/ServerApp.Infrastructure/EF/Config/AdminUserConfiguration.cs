namespace ServerApp.Infrastructure.EF.Config;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Admin;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable("AdminUsers");

        // Primary key
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        // Email - required, unique
        builder.Property(e => e.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(256)")
            .IsRequired()
            .HasConversion(
                e => e.Value,
                value => new AdminEmail(value));
        builder.HasIndex(e => e.Email).IsUnique();

        // DisplayName - required
        builder.Property(e => e.DisplayName)
            .HasColumnName("DisplayName")
            .HasColumnType("varchar(100)")
            .IsRequired()
            .HasConversion(
                n => n.Value,
                value => new AdminName(value));

        // PictureUrl - nullable
        builder.Property(e => e.PictureUrl)
            .HasColumnName("PictureUrl")
            .HasColumnType("varchar(500)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<AdminPictureUrl?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new AdminPictureUrl(v)));

        // GoogleSubjectId - nullable, unique for non-null values (partial index)
        builder.Property(e => e.GoogleSubjectId)
            .HasColumnName("GoogleSubjectId")
            .HasColumnType("varchar(100)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<AdminGoogleSub?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new AdminGoogleSub(v)));
        // Partial unique index - allows multiple NULLs but unique for non-NULL values
        builder.HasIndex(e => e.GoogleSubjectId)
            .HasDatabaseName("IX_AdminUsers_GoogleSubjectId")
            .IsUnique()
            .HasFilter("GoogleSubjectId IS NOT NULL");

        // YahooGuid - nullable, unique for non-null values (partial index)
        builder.Property(e => e.YahooGuid)
            .HasColumnName("YahooGuid")
            .HasColumnType("varchar(100)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<AdminYahooGuid?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new AdminYahooGuid(v)));
        // Partial unique index - allows multiple NULLs but unique for non-NULL values
        builder.HasIndex(e => e.YahooGuid)
            .HasDatabaseName("IX_AdminUsers_YahooGuid")
            .IsUnique()
            .HasFilter("YahooGuid IS NOT NULL");

        // LastLoginAt - required
        builder.Property(e => e.LastLoginAt)
            .HasColumnName("LastLoginAt")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasConversion(
                t => t.Value,
                value => new AdminLastLoginAt(value));

        // CreatedAt - required
        builder.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasConversion(
                t => t.Value,
                value => new AdminCreatedAt(value));

        // IsActive - required
        builder.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("boolean")
            .IsRequired()
            .HasConversion(
                a => a.Value,
                value => new AdminIsActive(value));
    }
}
