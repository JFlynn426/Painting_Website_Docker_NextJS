namespace ServerApp.Domain.Entities;

using ServerApp.Shared.Domain;
using ServerApp.Domain.ValueObjects.Admin;
using ServerApp.Domain.Events;

public class AdminUser : AggregateRoot<Guid>
{
    public AdminEmail Email { get; private set; } = default!;
    public AdminName DisplayName { get; private set; } = default!;
    public AdminPictureUrl? PictureUrl { get; private set; }
    public AdminGoogleSub GoogleSubjectId { get; private set; } = default!;
    public AdminLastLoginAt LastLoginAt { get; private set; } = default!;
    public AdminCreatedAt CreatedAt { get; private set; } = default!;
    public AdminIsActive IsActive { get; private set; } = default!;

    // Parameterless constructor for EF Core
    private AdminUser() { }

    // Internal constructor that accepts pre-computed ID (used by factory)
    internal AdminUser(AdminID id, AdminEmail email, AdminName displayName,
        AdminPictureUrl? pictureUrl, AdminGoogleSub googleSubjectId)
    {
        Id = id.Value;
        Email = email;
        DisplayName = displayName;
        PictureUrl = pictureUrl;
        GoogleSubjectId = googleSubjectId;
        CreatedAt = new AdminCreatedAt(DateTime.UtcNow);
        LastLoginAt = new AdminLastLoginAt(DateTime.UtcNow);
        IsActive = new AdminIsActive(true);

        AddEvent(new AdminCreatedEvent(Id, Email.Value));
    }

    public void UpdateLoginInfo(AdminName? displayName = null,
        AdminPictureUrl? pictureUrl = null)
    {
        if (displayName != null) DisplayName = displayName;
        if (pictureUrl != null) PictureUrl = pictureUrl;
        LastLoginAt = new AdminLastLoginAt(DateTime.UtcNow);
        AddEvent(new AdminLoggedInEvent(Id, Email.Value));
    }

    public void Deactivate()
    {
        IsActive = new AdminIsActive(false);
    }

    public void Reactivate()
    {
        IsActive = new AdminIsActive(true);
    }
}
