namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Admin;

public class AdminUserFactory : IAdminUserFactory
{
    public AdminUser Create(
        AdminEmail email,
        AdminName displayName,
        AdminPictureUrl? pictureUrl,
        AdminGoogleSub googleSubjectId)
    {
        // Auto-generate ID (single source of truth for ID generation)
        var id = new AdminID();

        var adminUser = new AdminUser(id, email, displayName, pictureUrl, googleSubjectId);
        return adminUser;
    }
}
