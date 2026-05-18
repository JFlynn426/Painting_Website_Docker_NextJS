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
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedOnAdd();

        // Email - required, unique
        builder.Property(e => e.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(256)")
            .IsRequired()
            .HasConversion(
                e => e.Value,
                value => new AdminEmail(value));
        builder.HasIndex(e => e.Email).IsUnique();

        // DisplayName - required
        builder.Property(e => e.DisplayName)
            .HasColumnName("DisplayName")
            .HasColumnType("nvarchar(100)")
            .IsRequired()
            .HasConversion(
                n => n.Value,
                value => new AdminName(value));

        // PictureUrl - nullable
        builder.Property(e => e.PictureUrl)
            .HasColumnName("PictureUrl")
            .HasColumnType("nvarchar(500)")
            .IsRequired(false)
            .HasConversion(
                new ValueConverter<AdminPictureUrl?, string?>(
                    v => v == null ? null : v.Value,
                    v => v == null ? null : new AdminPictureUrl(v)));

        // GoogleSubjectId - required, unique
        builder.Property(e => e.GoogleSubjectId)
            .HasColumnName("GoogleSubjectId")
            .HasColumnType("nvarchar(100)")
            .IsRequired()
            .HasConversion(
                g => g.Value,
                value => new AdminGoogleSub(value));
        builder.HasIndex(e => e.GoogleSubjectId).IsUnique();

        // LastLoginAt - required
        builder.Property(e => e.LastLoginAt)
            .HasColumnName("LastLoginAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasConversion(
                t => t.Value,
                value => new AdminLastLoginAt(value));

        // CreatedAt - required
        builder.Property(e => e.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasColumnType("datetime2")
            .IsRequired()
            .HasConversion(
                t => t.Value,
                value => new AdminCreatedAt(value));

        // IsActive - required
        builder.Property(e => e.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired()
            .HasConversion(
                a => a.Value,
                value => new AdminIsActive(value));
    }
}
