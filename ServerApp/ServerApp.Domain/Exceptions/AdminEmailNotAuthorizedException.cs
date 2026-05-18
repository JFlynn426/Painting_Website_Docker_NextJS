namespace ServerApp.Domain.Exceptions;

using ServerApp.Shared.Exceptions;

public class AdminEmailNotAuthorizedException : ServerAppException
{
    public AdminEmailNotAuthorizedException(string email)
        : base($"The email address '{email}' is not authorized to access the admin panel.")
    {
    }
}
