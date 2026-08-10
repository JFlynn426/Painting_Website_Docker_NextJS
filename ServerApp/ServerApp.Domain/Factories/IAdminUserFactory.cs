namespace ServerApp.Domain.Factories;

using ServerApp.Domain.Entities;
using ServerApp.Domain.ValueObjects.Admin;

public interface IAdminUserFactory
{
    AdminUser Create(
        AdminEmail email,
        AdminName displayName,
        AdminPictureUrl? pictureUrl,
        AdminGoogleSub? googleSubjectId = null,
        AdminYahooGuid? yahooGuid = null);
}
